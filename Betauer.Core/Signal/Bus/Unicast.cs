using System;
using Object = Godot.Object;

namespace Betauer.Signal.Bus {
    public abstract class Unicast<TEmitter, TSignalParams, TFilter>
        where TEmitter : Object
        where TFilter : Object {
        public readonly string? Name;
        public Action<TEmitter, TSignalParams>? EventHandler;

        protected Unicast(string name) {
            Name = name;
        }

        public abstract SignalHandler Connect(TEmitter area2D);

        public void Emit(TEmitter origin, TSignalParams signalParams) {
            EventHandler?.Invoke(origin, signalParams);
        }

        public void RemoveEventFilter() => EventHandler = null;

        public void OnEventFilter(TFilter filter, Action<TEmitter> action) {
            OnEventFilter(filter, (origin, signalParams) => action(origin));
        }

        public void OnEventFilter(TFilter filter, Action<TEmitter, TSignalParams> action) {
            EventHandler = (origin, signalParams) => {
                if (Matches(signalParams, filter)) action(origin, signalParams);
            };
        }

        public void OnEvent(Action<TEmitter, TSignalParams> action) {
            Console.WriteLine(Name + ":Subscribe enter *");
            EventHandler = action;
        }

        protected abstract bool Matches(TSignalParams e, TFilter detect);

    }
}