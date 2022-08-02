using Betauer.DI;
using Betauer.Memory;
using Betauer.OnReady;
using Betauer.Signal;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer {
    /**
     * A Container that listen for nodes added to the tree and inject services inside of them + process the OnReady tag
     */
    public class AutoConfiguration : Node {
        private Container _container;

        [Service(typeof(ObjectWatcherNode))] public ObjectWatcherNode ObjectWatcherNode => new ObjectWatcherNode();
        [Service(typeof(SceneTree))] public SceneTree SceneTree => GetTree();
        
        public override void _EnterTree() {
            PauseMode = PauseModeEnum.Process;
            _container = new Container(this);
            var builder = _container.CreateBuilder();
            builder.Scan();
            builder.ScanConfiguration(this);
            builder.Build();
            GetTree().OnNodeAdded(_GodotSignalNodeAdded);
        }

        private const string MetaInjected = "__injected";

        // Method called by Godot
        private void _GodotSignalNodeAdded(Node node) {
            if (node.GetScript() is CSharpScript) {
                OnReadyScanner.ScanAndInject(node);
                if (!node.HasMeta(MetaInjected)) {
                    _container.InjectAllFields(node);
                    node.SetMeta(MetaInjected, true);
                }
            }
        }
    }
}