using Godot;

namespace Betauer.Restorer {
    public class FocusRestorer : Restorer {
        private Control? _focused;
        private readonly Control _control;

        public FocusRestorer(Control control) {
            _control = control;
        }

        protected override void DoSave() {
            _focused = _control.GetFocusOwner();
        }

        protected override void DoRestore() {
            _focused?.GrabFocus();
        }
    }
}