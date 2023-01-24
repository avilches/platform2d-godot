using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.StateMachine.Async; 

public class StateNodeBuilderAsync<TStateKey, TEventKey> : 
    BaseStateBuilderAsync<StateNodeBuilderAsync<TStateKey, TEventKey>, TStateKey, TEventKey> 
    where TStateKey : Enum where TEventKey : Enum {
        
    public StateNodeBuilderAsync(TStateKey key, Action<IStateAsync<TStateKey, TEventKey>> build) : base(key, build) {
    }

    protected override IStateAsync<TStateKey, TEventKey> CreateState() {
        return new StateNodeAsync<TStateKey, TEventKey>(Key, EnterFunc, Conditions.ToArray(), ExecuteFunc, ExitFunc, SuspendFunc, AwakeFunc, Events, _input, _unhandledInput, _inputBatch, _unhandledInputBatch);
    }

    private event Action<InputEvent> _input;
    private event Action<InputEvent> _unhandledInput;
    private event Action<IEnumerable<InputEvent>>? _inputBatch;
    private event Action<IEnumerable<InputEvent>>? _unhandledInputBatch;
        
    /// <summary>
    /// This event will be only executed when the state machine is available (not executing enter, exit, execute,
    /// suspend or awake). That means if one of these methods uses await for a long time, the input event will not
    /// be executed until they finish.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public StateNodeBuilderAsync<TStateKey, TEventKey> OnInput(Action<InputEvent> input) {
        _input += input;
        return this;
    }

    public StateNodeBuilderAsync<TStateKey, TEventKey> OnUnhandledInput(Action<InputEvent> unhandledInput) {
        _unhandledInput += unhandledInput;
        return this;
    }

    public StateNodeBuilderAsync<TStateKey, TEventKey> OnInputBatch(Action<IEnumerable<InputEvent>> inputBatch) {
        _inputBatch += inputBatch;
        return this;
    }

    public StateNodeBuilderAsync<TStateKey, TEventKey> OnUnhandledInputBatch(Action<IEnumerable<InputEvent>> unhandledInputBatch) {
        _unhandledInputBatch += unhandledInputBatch;
        return this;
    }
}