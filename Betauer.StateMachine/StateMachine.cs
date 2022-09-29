using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Betauer.StateMachine {

    public interface IStateMachine<TStateKey, TTransitionKey> where TStateKey : Enum where TTransitionKey : Enum {
        public void AddState(IState<TStateKey, TTransitionKey> state);
        public StateBuilder<IStateMachine<TStateKey, TTransitionKey>, TStateKey, TTransitionKey> CreateState(TStateKey stateKey);
        public void AddListener(IStateMachineListener<TStateKey> machineListener);
        public void On(TTransitionKey transitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>> transition);
        public void On(TStateKey stateKey, TTransitionKey transitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>> transition);
        public bool IsState(TStateKey state);
        public IState<TStateKey, TTransitionKey> CurrentState { get; }
        public void Enqueue(TTransitionKey name);
        public Task Execute(float delta);
        public bool Available { get; }
        public string? Name { get; }
    }

    public abstract class StateMachine {
        protected static readonly Logger StaticLogger = LoggerFactory.GetLogger<StateMachine>();
    }

    public class StateMachine<TStateKey, TTransitionKey> : StateMachine, IStateMachine<TStateKey, TTransitionKey> where TStateKey : Enum where TTransitionKey : Enum {
        private readonly struct Change {
            internal readonly IState<TStateKey, TTransitionKey>? State;
            internal readonly TransitionType Type;
            internal Change(IState<TStateKey, TTransitionKey>? state, TransitionType type) {
                State = state;
                if (type == TransitionType.Trigger)
                    throw new ArgumentException("Change type can't be a trigger");
                Type = type;
            }
        }
        private static readonly Change NoChange = new(null, TransitionType.None);

        private readonly Stack<IState<TStateKey, TTransitionKey>> _stack = new();
        private readonly ExecuteContext<TStateKey, TTransitionKey> _executeContext = new();
        private readonly TriggerContext<TStateKey> _triggerContext = new();
        private Dictionary<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>>>? _events;
        private Dictionary<Tuple<TStateKey, TTransitionKey>, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>>>? _stateEvents;
        private List<IStateMachineListener<TStateKey>>? _listeners;
        private Func<Exception, ExecuteContext<TStateKey, TTransitionKey>, ExecuteTransition<TStateKey, TTransitionKey>> _onError; 
        private Change _nextChange;
        private readonly TStateKey _initialState;
        private bool _initialized = false;
        private bool _disposed = false;
        private TTransitionKey _nextTransition;
        private bool _nextTransitionDefined = false;
        private readonly object _lockObject = new();

        public readonly Logger Logger;
        public readonly Dictionary<TStateKey, IState<TStateKey, TTransitionKey>> States = new();
        public TStateKey[] GetStack() => _stack.Reverse().Select(e => e.Key).ToArray();
        public IState<TStateKey, TTransitionKey> CurrentState { get; private set; }
        public bool IsState(TStateKey state) => state.Equals(CurrentState.Key);
        public bool Available { get; private set; } = true;
        public string? Name { get; }

        public StateMachine(TStateKey initialState, string? name = null) {
            _initialState = initialState;
            Name = name;
            Logger = name == null ? StaticLogger : LoggerFactory.GetLogger(name);
        }

        public void On(TTransitionKey transitionKey, 
            Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>> transition) {
            _events ??= new Dictionary<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>>>();
            _events[transitionKey] = transition;
        }

        public void On(TStateKey stateKey, TTransitionKey transitionKey,
            Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>> transition) {
            _stateEvents ??= new Dictionary<Tuple<TStateKey, TTransitionKey>, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>>>();
            _stateEvents[new Tuple<TStateKey, TTransitionKey>(stateKey, transitionKey)] = transition;
        }

        public void AddListener(IStateMachineListener<TStateKey> machineListener) {
            _listeners ??= new List<IStateMachineListener<TStateKey>>();
            _listeners.Add(machineListener);
        }

        public StateBuilder<IStateMachine<TStateKey, TTransitionKey>, TStateKey, TTransitionKey> CreateState(
            TStateKey stateKey) {
            return new StateBuilder<IStateMachine<TStateKey, TTransitionKey>, TStateKey, TTransitionKey>(this, stateKey);
        }

        public void AddState(IState<TStateKey, TTransitionKey> state) {
            if (States.ContainsKey(state.Key)) throw new DuplicateNameException();
            States[state.Key] = state;
            if (state.Key.Equals(_initialState)) CurrentState = state;
        }

        public void Enqueue(TTransitionKey name) {
            lock (this) {
                _nextTransition = name;
                _nextTransitionDefined = true;
            }
        }
        
        public async Task Execute(float delta) {
            if (_disposed) return;
            if (!Available) return;
            lock (_lockObject) {
                if (!Available) return;
                Available = false;
            }
            var currentStateBackup = CurrentState;
            try {
                var change = NoChange;
                if (_nextTransitionDefined) {
                    _nextTransitionDefined = false;
                    var triggerTransition = GetTransitionFromTrigger(_nextTransition);
                    change = CreateChange(triggerTransition);
                } else if (!_initialized) {
                    var state = FindState(_initialState); // Call to ensure initial state exists
                    change = new Change(state, TransitionType.Set);
                } else {
                    change = _nextChange;
                }
                if (change.Type == TransitionType.Pop) {
                    await Exit(_stack.Pop(), change.State.Key);
                    CurrentState = TransitionTo(change, out var oldState);
                    await Awake(CurrentState, oldState.Key);
                } else if (change.Type == TransitionType.Push) {
                    await Suspend(CurrentState, change.State!.Key);
                    CurrentState = TransitionTo(change, out var oldState);
                    _stack.Push(CurrentState);
                    await Enter(CurrentState, oldState.Key);
                } else if (change.Type == TransitionType.PopPush) {
                    await Exit(_stack.Pop(), change.State.Key);
                    CurrentState = TransitionTo(change, out var oldState);
                    _stack.Push(CurrentState);
                    await Enter(CurrentState, oldState.Key);
                } else if (change.Type == TransitionType.Set) {
                    if (_stack.Count == 1) {
                        await Exit(_stack.Pop(), change.State.Key);
                    } else {
                        // Special case: 
                        // Exit from all the states from the stack, in order
                        while (_stack.Count > 0) {
                            var exitingState = _stack.Pop();
                            var to = _stack.Count > 0 ? _stack.Peek().Key : change.State.Key;
                            await Exit(exitingState, to);
                        }
                    }
                    CurrentState = TransitionTo(change, out var oldState);
                    _stack.Push(CurrentState);
                    await Enter(CurrentState, oldState.Key);
                }
                _listeners?.ForEach(listener => listener.OnExecuteStart(delta, CurrentState.Key));
                _executeContext.Delta = delta;
                var transition = await CurrentState.Execute(_executeContext);
                _listeners?.ForEach(listener => listener.OnExecuteEnd(CurrentState.Key));
                _nextChange = CreateChange(transition);
                _initialized = true;
            } catch (Exception e) {
                _nextChange = NoChange;
                CurrentState = currentStateBackup;
                throw;
            } finally {
                lock (_lockObject) {
                    Available = true;
                }
            }
        }

        private IState<TStateKey, TTransitionKey> FindState(TStateKey stateKey) {
            return States.TryGetValue(stateKey, out var state) ? state : 
                throw new KeyNotFoundException($"State {stateKey} not found. Please add it to the StateMachine");
        }

        private IState<TStateKey, TTransitionKey> TransitionTo(Change change, out IState<TStateKey, TTransitionKey> oldState) {
            var newState = change.State;
            oldState = CurrentState ?? newState;
            Transition(change, oldState, newState);
            return newState;
        }

        private Change CreateChange(ExecuteTransition<TStateKey, TTransitionKey> candidate) {
            if (candidate.IsTrigger() ) {
                candidate = GetTransitionFromTrigger(candidate.TransitionKey);
            }
            if (CurrentState != null && candidate.IsSet(CurrentState.Key)) {
                return NoChange;
            }
            if (candidate.IsPop()) {
                if (_stack.Count <= 1) {
                    throw new InvalidOperationException("Pop");
                }
                var o = _stack.Pop();
                var transition = new Change(_stack.Peek(), TransitionType.Pop);
                _stack.Push(o);
                return transition;
            }
            if (candidate.IsNone()) {
                return NoChange;
            }
            IState<TStateKey, TTransitionKey> newState = FindState(candidate.StateKey);
            return new Change(newState, candidate.Type);
        }


        private ExecuteTransition<TStateKey, TTransitionKey> GetTransitionFromTrigger(TTransitionKey name) {
            TriggerTransition<TStateKey> triggerTransition = default;
            var found = false;
            if (CurrentState != null && _stateEvents != null) {
                var key = new Tuple<TStateKey, TTransitionKey>(CurrentState.Key, name);
                if (_stateEvents != null && _stateEvents.ContainsKey(key)) {
                    triggerTransition = _stateEvents[key].Invoke(_triggerContext);
                    found = true;
                }
            }
            if (!found && _events != null && _events.ContainsKey(name)) {
                triggerTransition = _events[name].Invoke(_triggerContext);
                found = true;
            }
            if (!found) {
                throw new KeyNotFoundException($"Transition {name} not found. Please add it to the StateMachine");
            }
            var transition = triggerTransition.ToTransition<TTransitionKey>();
            return transition;
        }

        private void Transition(Change change, IState<TStateKey, TTransitionKey> from, IState<TStateKey, TTransitionKey> to) {
            _listeners?.ForEach(listener => listener.OnTransition(from.Key, to.Key));
            #if DEBUG
                Logger.Debug($"> {change.Type} State: \"{to.Key}\"(from:{from.Key}");
            #endif
        }

        private Task Exit(IState<TStateKey, TTransitionKey> state, TStateKey to) {
            _listeners?.ForEach(listener => listener.OnExit(state.Key, to));
            #if DEBUG
                Logger.Debug($"Exit: \"{state.Key}\"(to:{to})\"");
            #endif
            return state.Exit(to);
        }

        private Task Suspend(IState<TStateKey, TTransitionKey> state, TStateKey to) {
            _listeners?.ForEach(listener => listener.OnSuspend(state.Key, to));
            #if DEBUG
                Logger.Debug($"Suspend: \"{state.Key}\"(to:{to})");
            #endif
            return state.Suspend(to);
        }

        private Task Awake(IState<TStateKey, TTransitionKey> state, TStateKey from) {
            _listeners?.ForEach(listener => listener.OnAwake(state.Key, from));
            #if DEBUG
                Logger.Debug($"Awake: \"{state.Key}\"(from:{from})");
            #endif
            return state.Awake(from);
        }

        private Task Enter(IState<TStateKey, TTransitionKey> state, TStateKey from) {
            _listeners?.ForEach(listener => listener.OnEnter(state.Key, from));
            #if DEBUG
                Logger.Debug($"Enter: \"{state.Key}\"(from:{from})");
            #endif
            return state.Enter(from);
        }

        public void Dispose() {
            _disposed = true;
        }
    }
}