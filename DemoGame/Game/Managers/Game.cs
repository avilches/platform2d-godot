using System;
using System.Threading.Tasks;
using Betauer.Application.Monitor;
using Betauer.DI;
using Betauer.Nodes;
using Godot;

namespace Veronenger.Game.Managers {

    [Service]
    public class Game {

        [Inject] private SceneTree SceneTree { get; set; }
        [Inject] private StageManager StageManager { get; set; }
        [Inject] private MainResourceLoader MainResourceLoader { get; set; }
        [Inject] private DebugOverlay DefaultDebugOverlay { get; set; }
        
        private Node _currentGameScene;
        private Node2D _playerScene;

        public void StartNew() {
            _currentGameScene = MainResourceLoader.CreateWorld2Empty();
            var tileMap = _currentGameScene.GetNode<TileMap>("RealTileMap");
            new WorldGenerator().Generate(tileMap);
            AddPlayerToScene(_currentGameScene, Vector2.Zero);
            SceneTree.Root.AddChildDeferred(_currentGameScene);
        }

        public void Start() {
            _currentGameScene = MainResourceLoader.CreateWorld2();
            AddPlayerToScene(_currentGameScene);
            SceneTree.Root.AddChildDeferred(_currentGameScene);
        }

        public void QueueChangeSceneWithPlayer(string sceneName) {
            StageManager.ClearTransition();
            _currentGameScene.QueueFree();
            var nextScene = ResourceLoader.Load<PackedScene>(sceneName).Instance();
            AddPlayerToScene(nextScene);
            _currentGameScene = nextScene;
            SceneTree.Root.AddChildDeferred(nextScene);
        }
        
        private void AddPlayerToScene(Node nextScene) {
            var position2D = nextScene.GetNode<Node2D>("PositionPlayer");
            if (position2D == null) throw new Exception("Node PositionPlayer not found when loading scene " + nextScene.Filename);
            AddPlayerToScene(nextScene, position2D.GlobalPosition);
        }

        private void AddPlayerToScene(Node nextScene, Vector2 position) {
            _playerScene = MainResourceLoader.CreatePlayer();
            nextScene.AddChild(_playerScene);
            // TODO: this shows a warning "!is_inside_tree()" is true. Returned: get_transform()" but it still works 
            _playerScene.GlobalPosition = position;
        }

        public void End() {
            _currentGameScene.PrintStrayNodes();
            _currentGameScene.QueueFree();
            _currentGameScene = null;
        }
    }
}