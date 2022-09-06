using System;
using System.Collections.Generic;

namespace Betauer.Memory {
    public interface IObjectConsumer {
        public bool Consume(bool force);
    }

    public class Consumer {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(Consumer));
        private readonly HashSet<IObjectConsumer> _objects = new HashSet<IObjectConsumer>();

        public int Count => _objects.Count;

        public List<IObjectConsumer> ToList() {
            lock (_objects) return new List<IObjectConsumer>(_objects);
        }

        public int Consume(bool force = false) {
            lock (_objects) {
                return _objects.RemoveWhere(o => {
                    var consumed = o.Consume(force);
#if DEBUG
                    if (consumed) {
                        Logger.Debug(
                            $"Count: {_objects.Count - 1} | - {o.GetType().Name}@{o.GetHashCode():x8} Consumed");
                    }
#endif
                    return consumed;
                });
            }
        }

        public void Add(IObjectConsumer o) {
            lock (_objects) {
#if DEBUG
                Logger.Debug($"Count: {_objects.Count + 1} | + {o.GetType().Name}@{o.GetHashCode():x8} {o}");
#endif
                _objects.Add(o);
            }
        }

        public void Remove(IObjectConsumer o) {
            lock (_objects) {
                _objects.RemoveWhere(x => {
                    var matches = x == o;
#if DEBUG
                    if (matches) {
                        Logger.Debug($"Count: {_objects.Count - 1} | - {o.GetType().Name}@{o.GetHashCode():x8}");
                    }
#endif
                    return matches;
                });
            }
        }

        public virtual void Dispose() {
            Consume(true);
        }
    }
}