using System;
using Godot;
using Tools;
using Veronenger.Game.Managers.Autoload;

namespace Veronenger.Game.Controller.Stage {
    /**
     * Add this script to a Comera2D in your Player.
     * The Player should have an Area2D called 'StageDetector' children
     * (So, this script and StageDetector should be siblings)
     *
     * Player (KB2d)
     *   +- StageDetector : Area2D
     *   +- StageCameraController (this class)
     */
    public class StageCameraController : Camera2D {

        public override void _EnterTree() {
            var stageDetector = GetNode<Area2D>("../StageDetector");
            if (stageDetector == null) {
                throw new Exception("Missing parent node Area2D 'StageDetector'");
            }
            GameManager.Instance.StageManager.ConfigureStageCamera(this, stageDetector);
        }

        public void ChangeStage(Rect2 rect2) {
            Debug.Stage("Camera",rect2.Position + " " + rect2.End);
            LimitLeft = (int)rect2.Position.x;
            LimitTop = (int)rect2.Position.y;
            LimitRight = (int)rect2.End.x;
            LimitBottom = (int)rect2.End.y;
        }
    }

}