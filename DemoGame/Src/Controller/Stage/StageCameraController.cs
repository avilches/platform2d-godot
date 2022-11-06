using Godot;
using Betauer;
using Betauer.DI;
using Betauer.Tools.Logging;
using Betauer.OnReady;
using Veronenger.Managers;

namespace Veronenger.Controller.Stage {
    /**
     * Add this script to a Camera2D in your Player.
     * The Player should have an Area2D called 'StageDetector' children
     * (So, this script and StageDetector should be siblings)
     *
     * Player (KB2d)
     *   +- StageDetector : Area2D
     *   +- StageCameraController (this class)
     */
    public class StageCameraController : Camera2D {

        [Inject] public StageManager StageManager { get; set;}

        [OnReady("../Detector")] private Area2D stageDetector;

        private const bool EnableStageCamera = false;

        public override void _Ready() {
            if (EnableStageCamera) StageManager.ConfigureStageCamera(this, stageDetector);
        }

        public void ChangeStage(Rect2 rect2) {
            LoggerFactory.GetLogger(typeof(StageCameraController)).Debug($"Camera {rect2.Position} {rect2.End}");
            LimitLeft = (int)rect2.Position.x;
            LimitTop = (int)rect2.Position.y;
            LimitRight = (int)rect2.End.x;
            LimitBottom = (int)rect2.End.y;
        }
    }
}