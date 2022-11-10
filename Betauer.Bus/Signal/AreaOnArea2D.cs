using Betauer.Core.Signal;
using Godot;

namespace Betauer.Bus.Signal {
    public static class AreaOnArea2D {
        public class Collection : SignalCollection<Area2D, Area2D, Area2D> {

            public Collection(string? name = null) : base(name) {
            }

            public bool IsOverlapping => Size() > 0;

            protected override Area2D Extract(Area2D signalArgs) {
                return signalArgs;
            }

            public Collection Connect(Area2D area2D) {
                area2D.OnAreaEntered(target => OnEnter(area2D, target));
                area2D.OnAreaExited(target => OnExit(area2D, target));
                return this;
            }
        }

        public class Status : SignalStatus<Area2D, Area2D, Area2D> {
            protected override Area2D Extract(Area2D signalArgs) {
                return signalArgs;
            }
            
            public Status Connect(Area2D area2D) {
                area2D.OnAreaEntered(target => OnEnter(area2D, target));
                area2D.OnAreaExited(target => OnExit(area2D, target));
                return this;
            }
        }
    }

    public static class AreaOnArea2DEntered {
        public class Multicast : SignalMulticast<Area2D, Area2D, Area2D> {
            public Multicast(string name) : base(name) {
            }

            public override SignalHandler Connect(Area2D area2D) {
                return area2D.OnAreaEntered(target => Publish(area2D, target));
            }

            protected override bool Matches(Area2D e, Area2D detect) {
                return e == detect;
            }
        }

        public class Unicast : SignalUnicast<Area2D, Area2D, Area2D> {
            public Unicast(string name) : base(name) {
            }

            public override SignalHandler Connect(Area2D area2D) {
                return area2D.OnAreaEntered(target => Publish(area2D, target));
            }

            protected override bool Matches(Area2D e, Area2D detect) {
                return e == detect;
            }
        }
    }

    public static class AreaOnArea2DExited {
        public class Multicast : SignalMulticast<Area2D, Area2D, Area2D> {
            public Multicast(string name) : base(name) {
            }

            public override SignalHandler Connect(Area2D area2D) {
                return area2D.OnAreaExited(target => Publish(area2D, target));
            }

            protected override bool Matches(Area2D e, Area2D detect) {
                return e == detect;
            }
        }

        public class Unicast : SignalUnicast<Area2D, Area2D, Area2D> {
            public Unicast(string name) : base(name) {
            }

            public override SignalHandler Connect(Area2D area2D) {
                return area2D.OnAreaExited(target => Publish(area2D, target));
            }

            protected override bool Matches(Area2D e, Area2D detect) {
                return e == detect;
            }
        }
    }
}