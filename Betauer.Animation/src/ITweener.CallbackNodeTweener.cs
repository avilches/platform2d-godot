using System;
using Betauer.Tools.Logging;
using Godot;

namespace Betauer.Animation {
    public class CallbackNodeTweener : ITweener {
        private static readonly Logger Logger = LoggerFactory.GetLogger<CallbackNodeTweener>();
        private readonly Action<Node> _callback;
        private readonly float _delay;

        public bool IsCompatibleWith(Node node) {
            return true;
        }

        internal CallbackNodeTweener(float delay, Action<Node> callback) {
            _delay = delay;
            _callback = callback;
        }

        public float Start(Tween sceneTreeTween, float initialDelay, Node target) {
            if (!GodotObject.IsInstanceValid(sceneTreeTween)) {
                Logger.Debug("Can't start a {0} from a freed tween instance", nameof(CallbackTweener));
                return 0;
            }
            var start = _delay + initialDelay;
            Logger.Debug("Adding anonymous callback with {0}s delay. Scheduled: {1:F}", _delay, start);
            var callbackTweener = sceneTreeTween
                .Parallel()
                .TweenCallback(Callable.From(() =>_callback(target)))
                .SetDelay(start);
            return _delay;
        }
    }
}