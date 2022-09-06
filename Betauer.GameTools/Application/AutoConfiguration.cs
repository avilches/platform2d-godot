using System;
using Betauer.DI;
using Betauer.Memory;
using Betauer.Nodes;
using Betauer.OnReady;
using Betauer.Signal;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Application {
    /**
     * A Container that listen for nodes added to the tree and inject services inside of them + process the OnReady tag
     */
    public abstract class AutoConfiguration : Node {
        protected readonly Container Container = new Container();
        protected readonly MainLoopNotificationHandler MainLoopNotificationHandler = new MainLoopNotificationHandler();

        [Service] public Consumer Consumer => DefaultObjectWatcherRunner.Instance;
        [Service] public NodeHandler NodeHandler => DefaultNodeHandler.Instance;
        [Service] public SceneTree SceneTree => SceneTreeHolder.SceneTree;
        [Service] public MainLoopNotificationHandler MainLoopNotificationHandlerFactory => MainLoopNotificationHandler;
        
        private bool _addSingletonNodesToTree = true;
        private float _watchTimer = 10f;
        private bool _isReady = false;

        public void EnableAddSingletonNodesToTree(bool enabled) => _addSingletonNodesToTree = enabled;
        public void SetWatchTimer(float watchTimer) => _watchTimer = watchTimer;

        public override void _EnterTree() {
            // It can't be called before _EnterTree because the SceneTree is exposed as a service using SceneTreeHolder
            // It needs to be called in _EnterTree because the _Ready() is called after the the main scene _Ready(), so the main
            // scene will not have services injected.
            SceneTreeHolder.SceneTree = GetTree();
            StartContainer();

            MainLoopNotificationHandler.OnWmQuitRequest += () => {
                LoggerFactory.EnableAutoFlush();
                DefaultObjectWatcherRunner.Instance.Consume(true);
            };
            DefaultObjectWatcherRunner.Instance.Start(_watchTimer);
            PauseMode = PauseModeEnum.Process;
            this.OnReady(() => _isReady = true, true);
        }

        private void StartContainer() {
            if (_addSingletonNodesToTree) {
                Container.OnCreate += (lifetime, o) => {
                    if (lifetime == Lifetime.Singleton && o is Node node) {
                        if (_isReady) GetTree().Root.AddChild(node);
                        else GetTree().Root.CallDeferred("add_child", node);
                    }
                };
            }
            Container.CreateBuilder()
                .Scan(GetType().Assembly)
                .ScanConfiguration(this)
                .Build()
                .InjectServices(this);
            GetTree().OnNodeAdded(_GodotSignalNodeAdded);
        }

        private const string MetaInjected = "__injected";

        // Method called by Godot
        private void _GodotSignalNodeAdded(Node node) {
            if (node.GetScript() is CSharpScript) {
                OnReadyScanner.ScanAndInject(node);
                if (!node.HasMeta(MetaInjected)) {
                    Container.InjectServices(node);
                    node.SetMeta(MetaInjected, true);
                }
            }
        }

        public override void _Notification(int what) {
            MainLoopNotificationHandler.Execute(what);
        }
    }
}