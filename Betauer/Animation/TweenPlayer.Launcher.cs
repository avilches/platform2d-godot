using System;
using System.Threading.Tasks;
using Godot;
using Object = Godot.Object;

namespace Betauer.Animation {
    public class Launcher : ITweenPlayer<Launcher> {
        public class SequenceWithLauncher : RegularSequenceBuilder<SequenceWithLauncher> {
            private readonly Launcher _launcher;

            internal SequenceWithLauncher(Launcher launcher,
                bool createEmptyTweenList) : base(createEmptyTweenList) {
                _launcher = launcher;
            }

            public Launcher EndSequence() {
                return _launcher;
            }

            public Task<LoopStatus> Play(float initialDelay = 0, float duration = -1) {
                return _launcher.Play(this, DefaultTarget, initialDelay, duration);
            }
        }
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(Launcher));

        public Tween Tween { get; private set; }

        public Launcher() {
        }

        public Launcher(Tween tween) {
            WithTween(tween);
        }

        public Launcher CreateNewTween(Node node) {
            var tween = new Tween();
            node.AddChild(tween);
            return WithTween(tween);
        }

        public Launcher WithTween(Tween tween) {
            Tween = tween;
            return this;
        }

        public Launcher RemoveTween() {
            Tween = null;
            return this;
        }

        public bool IsRunning() => Tween.IsActive();

        /*
         * Flow:
         * 1 Start <-> Stop
         * 2 Reset keeps the current status. So, Start -> Reset will continue playing (but from beginning)
         */

        public Launcher Start() {
            if (!Object.IsInstanceValid(Tween)) {
                Logger.Warning("Can't Start in a freed Tween instance");
                return this;
            }
            Tween.Start();
            Tween.ResumeAll();
            return this;
        }

        public Launcher Stop() {
            if (!Object.IsInstanceValid(Tween)) {
                Logger.Warning("Can't Stop AnimationTweenPlayer in a freed Tween instance");
                return this;
            }
            Logger.Info("Tween.StopAll()");
            Tween.StopAll();
            return this;
        }

        public Launcher Reset() {
            if (!Object.IsInstanceValid(Tween)) {
                Logger.Warning("Can't Reset AnimationTweenPlayer in a freed Tween instance");
                return this;
            }
            Logger.Info("Tween.StopAll()");
            Tween.StopAll();
            Logger.Info("Tween.RemoveAll()");
            Tween.RemoveAll();
            Logger.Info("Tween.Start()");
            Tween.Start();
            return this;
        }

        public Launcher Kill() {
            if (!Object.IsInstanceValid(Tween)) return this;
            Logger.Info("Tween.StopAll()");
            Tween.StopAll();
            Logger.Info("Tween.QueueFree()");
            Tween.QueueFree();
            return this;
        }


        public SequenceWithLauncher CreateSequence(Node defaultTarget = null) {
            var sequence = new SequenceWithLauncher(this, true).SetDefaultTarget(defaultTarget);
            return sequence;
        }

        public Task<LoopStatus> Play(ISequence sequence, Node defaultTarget = null, float initialDelay = 0,
            float duration = -1) {
            var loops = sequence is ILoopedSequence loopedSequence ? loopedSequence.Loops: 1;
            return Play(loops, sequence, defaultTarget, initialDelay, duration);
        }

        public Task<LoopStatus> Play(int loops, ISequence sequence, Node defaultTarget = null, float initialDelay = 0,
            float duration = -1) {
            return _Execute(loops, sequence, defaultTarget, initialDelay, duration);
        }

        private Task<LoopStatus> _Execute(int loops, ISequence sequence, Node defaultTarget = null, float initialDelay = 0,
            float duration = -1) {
            LoopStatus loopStatus = new LoopStatus(Tween, loops, sequence, defaultTarget, duration);
            return loopStatus.Start(initialDelay).Await();
        }
    }
}