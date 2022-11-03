using System;

namespace Betauer.StateMachine {
    public class ConditionBuilder<TBuilder, TStateKey, TEventKey>
        where TStateKey : Enum
        where TEventKey : Enum 
        where TBuilder : class {
        
        private readonly TBuilder _builder;
        private readonly Action<ConditionBuilder<TBuilder, TStateKey, TEventKey>> _onBuild;

        internal readonly Func<bool> Predicate;
        internal Func<ConditionContext<TStateKey, TEventKey>, Command<TStateKey, TEventKey>>? Execute;
        internal Command<TStateKey, TEventKey> Result;

        internal ConditionBuilder(TBuilder builder, Func<bool> predicate, Action<ConditionBuilder<TBuilder, TStateKey, TEventKey>> onBuild) {
            _builder = builder;
            Predicate = predicate;
            _onBuild = onBuild;
        }

        public TBuilder Push(TStateKey state) {
            return Then(new Command<TStateKey, TEventKey>(CommandType.Push, state, default));
        }

        public TBuilder Set(TStateKey state) {
            return Then(new Command<TStateKey, TEventKey>(CommandType.Set, state, default));
        }

        public TBuilder PopPush(TStateKey state) {
            return Then(new Command<TStateKey, TEventKey>(CommandType.PopPush, state, default));
        }

        public TBuilder Pop() {
            return Then(new Command<TStateKey, TEventKey>(CommandType.Pop, default, default));
        }

        public TBuilder None() {
            return Then(new Command<TStateKey, TEventKey>(CommandType.None, default, default));
        }

        public TBuilder Trigger(TEventKey eventKey) {
            return Then(new Command<TStateKey, TEventKey>(CommandType.Trigger, default, eventKey));
        }

        private TBuilder Then(Command<TStateKey, TEventKey> command) {
            Result = command;
            _onBuild(this);
            return _builder;
        }

        public TBuilder Then(Func<ConditionContext<TStateKey, TEventKey>, Command<TStateKey, TEventKey>> execute) {
            Execute = execute;
            _onBuild(this);
            return _builder;
        }

    }
}