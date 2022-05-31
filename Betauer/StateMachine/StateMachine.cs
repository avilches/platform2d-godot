using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Collections;
using Godot;

namespace Betauer.StateMachine {

    public interface IStateMachine<TStateKey, TTransitionKey> {
        public void AddState(IState<TStateKey, TTransitionKey> state);
        public void AddListener(IStateListener<TStateKey> listener);
        public void On(TTransitionKey transitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>> transition);
        public void On(TStateKey stateKey, TTransitionKey transitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>> transition);
        public IState<TStateKey, TTransitionKey> State { get; }
        public void Trigger(TTransitionKey name);
        public Task Execute(float delta);
    }

    public class StateMachineBuilder<T, TStateKey, TTransitionKey> where T : IStateMachine<TStateKey, TTransitionKey> {
        private readonly T _stateMachine;
        private readonly Queue<StateBuilder<T, TStateKey, TTransitionKey, StateMachineBuilder<T, TStateKey, TTransitionKey>>> _pendingStateBuilders =
            new Queue<StateBuilder<T, TStateKey, TTransitionKey, StateMachineBuilder<T, TStateKey, TTransitionKey>>>();

        public StateMachineBuilder(T stateMachine) {
            _stateMachine = stateMachine;
        }

        public StateMachineBuilder<T, TStateKey, TTransitionKey> On(TStateKey stateKey, TTransitionKey transitionKey, 
            Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>> transition) {
            _stateMachine.On(stateKey, transitionKey, transition);
            return this;
        }

        public StateMachineBuilder<T, TStateKey, TTransitionKey> On(TTransitionKey transitionKey, 
            Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>> transition) {
            _stateMachine.On(transitionKey, transition);
            return this;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, StateMachineBuilder<T, TStateKey, TTransitionKey>> State(TStateKey stateKey) {
            var stateBuilder = new StateBuilder<T, TStateKey, TTransitionKey, StateMachineBuilder<T, TStateKey, TTransitionKey>>(_stateMachine, stateKey, this);
            _pendingStateBuilders.Enqueue(stateBuilder);
            return stateBuilder;
        }

        public StateMachineBuilder<T, TStateKey, TTransitionKey> AddListener(IStateListener<TStateKey> listener) {
            _stateMachine.AddListener(listener);
            return this;
        }

        public T Build() {
            while (_pendingStateBuilders.Count > 0) _pendingStateBuilders.Dequeue().Build();
            return _stateMachine;
        }
    }
    
    public interface IStateListener<in TStateKey> {
        public void OnEnter(TStateKey state, TStateKey from);
        public void OnAwake(TStateKey state, TStateKey from);
        public void OnSuspend(TStateKey state, TStateKey to);
        public void OnExit(TStateKey state, TStateKey to);
        public void OnTransition(TStateKey from, TStateKey to);
    }

    public class StateListenerBase<TStateKey> : IStateListener<TStateKey> {
        public virtual void OnEnter(TStateKey state, TStateKey from) {
        }

        public virtual void OnAwake(TStateKey state, TStateKey from) {
        }

        public virtual void OnSuspend(TStateKey state, TStateKey to) {
        }

        public virtual void OnExit(TStateKey state, TStateKey to) {
        }

        public virtual void OnTransition(TStateKey from, TStateKey to) {
        }
    }

    public class StateMachine<TStateKey, TTransitionKey> : IStateMachine<TStateKey, TTransitionKey> {
        internal readonly struct Change {
            internal readonly IState<TStateKey, TTransitionKey>? State;
            internal readonly TransitionType Type;
            internal Change(IState<TStateKey, TTransitionKey>? state, TransitionType type) {
                State = state;
                if (type == TransitionType.Trigger)
                    throw new ArgumentException("Change type can't be a trigger");
                Type = type;
            }
            internal bool IsPop() => Type == TransitionType.Pop;
            internal bool IsPopPush() => Type == TransitionType.PopPush;
            internal bool IsPush() => Type == TransitionType.Push;
            internal bool IsChange() => Type == TransitionType.Change;
            internal bool IsChange(TStateKey key) {
                return Type == TransitionType.Change && State != null && EqualityComparer<TStateKey>.Default.Equals(State.Key, key);
            }
            internal bool IsNone() => Type == TransitionType.None;
        }

        private readonly Stack<IState<TStateKey, TTransitionKey>> _stack = new Stack<IState<TStateKey, TTransitionKey>>();
        private readonly ExecuteContext<TStateKey, TTransitionKey> _executeContext = new ExecuteContext<TStateKey, TTransitionKey>();
        private readonly TriggerContext<TStateKey> _triggerContext = new TriggerContext<TStateKey>();
        private Dictionary<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>>>? _events;
        private Dictionary<Tuple<TStateKey, TTransitionKey>, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>>>? _stateEvents;
        private SimpleLinkedList<IStateListener<TStateKey>>? _listeners;
        private Change _nextChange;
        private readonly TStateKey _initialState;
        private bool _disposed = false;

        public readonly Logger Logger;
        public readonly string? Name;
        public readonly Dictionary<TStateKey, IState<TStateKey, TTransitionKey>> States = new Dictionary<TStateKey, IState<TStateKey, TTransitionKey>>();
        public TStateKey[] GetStack() => _stack.Reverse().Select(e => e.Key).ToArray();
        public IState<TStateKey, TTransitionKey> State { get; private set; }

        public StateMachine(TStateKey initialState, string? name = null) {
            _initialState = initialState;
            Name = name;
            Logger = name != null ?
                LoggerFactory.GetLogger(name, "StateMachine") : 
                LoggerFactory.GetLogger("StateMachine");
        }

        public StateMachineBuilder<StateMachine<TStateKey, TTransitionKey>, TStateKey, TTransitionKey> CreateBuilder() {
            return new StateMachineBuilder<StateMachine<TStateKey, TTransitionKey>, TStateKey, TTransitionKey>(this);
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

        public void AddListener(IStateListener<TStateKey> listener) {
            _listeners ??= new SimpleLinkedList<IStateListener<TStateKey>>();
            _listeners.Add(listener);
        }

        public void AddState(IState<TStateKey, TTransitionKey> state) {
            if (States.ContainsKey(state.Key)) throw new DuplicateNameException();
            States[state.Key] = state;
        }

        public void Trigger(TTransitionKey name) {
            var transition = GetTransitionFromTrigger(name);
            _nextChange = CreateChange(transition);
        }

        public async Task Execute(float delta) {
            if (_disposed) return;
            var change = State == null && _nextChange.State == null
                ? new Change(States[_initialState], TransitionType.Change)
                : _nextChange;
            await _ExitCurrentState(change);
            await _EnterNewState(change);
            _executeContext.Delta = delta;
            var transition = await State.Execute(_executeContext);
            _nextChange = CreateChange(transition);
        }

        private Change CreateChange(ExecuteTransition<TStateKey, TTransitionKey> candidate) {
            if (candidate.IsTrigger() ) {
                candidate = GetTransitionFromTrigger(candidate.TransitionKey);
            }
            if (State != null && candidate.IsChange(State.Key)) {
                return new Change(State, TransitionType.None);
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
                return new Change(State, TransitionType.None);
            }
            IState<TStateKey, TTransitionKey> newState = States[candidate.StateKey];
            return new Change(newState, candidate.Type);
        }

        private ExecuteTransition<TStateKey, TTransitionKey> GetTransitionFromTrigger(TTransitionKey name) {
            TriggerTransition<TStateKey> triggerTransition = default;
            var found = false;
            if (State != null && _stateEvents != null) {
                var key = new Tuple<TStateKey, TTransitionKey>(State.Key, name);
                if (_stateEvents != null && _stateEvents.ContainsKey(key)) {
                    triggerTransition = _stateEvents[key](_triggerContext);
                    found = true;
                }
            }
            if (!found && _events != null && _events.ContainsKey(name)) {
                triggerTransition = _events[name](_triggerContext);
                found = true;
            }
            if (!found) {
                throw new KeyNotFoundException("Transition " + name + " not found");
            }
            var transition = triggerTransition.ToTransition<TTransitionKey>();
            return transition;
        }

        private async Task _ExitCurrentState(Change change) {
            if (_disposed || State == null || change.IsNone()) {
                return;
            }
            if (change.IsPush()) {
                await Suspend(State, change.State!.Key);
            } else if (change.IsChange() && _stack.Count > 1) {
                // Spacial case: 
                // Exit from all the states in stack, in order, until the next change
                while (_stack.Count > 0) {
                    var exitingState = _stack.Pop();
                    var to = _stack.Count > 0 ? _stack.Peek().Key : change.State.Key;
                    await Exit(exitingState, to);
                }
            } else {
                // Pop, PopPush or Change with no stack: just exit the current state
                var currentState = _stack.Pop();
                await Exit(currentState, change.State.Key);
            }
        }

        private async Task _EnterNewState(Change change) {
            if (_disposed || change.IsNone()) return;
            var newState = change.State;
            var oldState = State ?? newState;
            State = newState;

            Transition(change, oldState, newState);

            if (change.IsPop()) {
                await Awake(newState, oldState.Key);
            } else {
                // Push, PopPush or Change: enter the new state
                _stack.Push(newState);
                await Enter(State, oldState.Key);
            }
        }

        private void Transition(Change change, IState<TStateKey, TTransitionKey> from, IState<TStateKey, TTransitionKey> to) {
            _listeners?.ForEach(listener => listener.OnTransition(from.Key, to.Key));
            Logger.Debug($"> {change.Type} State: \"{to.Key}\"(from:{from.Key}");
        }

        private async Task Exit(IState<TStateKey, TTransitionKey> state, TStateKey to) {
            _listeners?.ForEach(listener => listener.OnExit(state.Key, to));
            Logger.Debug($"Exit: \"{state.Key}\"(to:{to})\"");
            await state.Exit(to);
        }

        private async Task Suspend(IState<TStateKey, TTransitionKey> state, TStateKey to) {
            _listeners?.ForEach(listener => listener.OnSuspend(state.Key, to));
            Logger.Debug($"Suspend: \"{state.Key}\"(to:{to})");
            await state.Suspend(to);
        }

        private async Task Awake(IState<TStateKey, TTransitionKey> state, TStateKey from) {
            _listeners?.ForEach(listener => listener.OnAwake(state.Key, from));
            Logger.Debug($"Awake: \"{state.Key}\"(from:{from})");
            await state.Awake(from);
        }

        private async Task Enter(IState<TStateKey, TTransitionKey> state, TStateKey from) {
            _listeners?.ForEach(listener => listener.OnEnter(state.Key, from));
            Logger.Debug($"Enter: \"{state.Key}\"(from:{from})");
            await state.Enter(from);
        }

        public void Dispose() {
            _disposed = true;
        }
    }
}