using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.GodotAction.Proxy {
    public class ScrollContainerAction : ProxyNode {

        private List<Action>? _onDrawAction; 
        public ScrollContainerAction OnDraw(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onDrawAction, "draw", nameof(_GodotSignalDraw), action, oneShot, deferred);
            return this;
        }

        public ScrollContainerAction RemoveOnDraw(Action action) {
            RemoveSignal(_onDrawAction, "draw", nameof(_GodotSignalDraw), action);
            return this;
        }

        private ScrollContainerAction _GodotSignalDraw() {
            ExecuteSignal(_onDrawAction);
            return this;
        }

        private List<Action>? _onFocusEnteredAction; 
        public ScrollContainerAction OnFocusEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onFocusEnteredAction, "focus_entered", nameof(_GodotSignalFocusEntered), action, oneShot, deferred);
            return this;
        }

        public ScrollContainerAction RemoveOnFocusEntered(Action action) {
            RemoveSignal(_onFocusEnteredAction, "focus_entered", nameof(_GodotSignalFocusEntered), action);
            return this;
        }

        private ScrollContainerAction _GodotSignalFocusEntered() {
            ExecuteSignal(_onFocusEnteredAction);
            return this;
        }

        private List<Action>? _onFocusExitedAction; 
        public ScrollContainerAction OnFocusExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onFocusExitedAction, "focus_exited", nameof(_GodotSignalFocusExited), action, oneShot, deferred);
            return this;
        }

        public ScrollContainerAction RemoveOnFocusExited(Action action) {
            RemoveSignal(_onFocusExitedAction, "focus_exited", nameof(_GodotSignalFocusExited), action);
            return this;
        }

        private ScrollContainerAction _GodotSignalFocusExited() {
            ExecuteSignal(_onFocusExitedAction);
            return this;
        }

        private List<Action<InputEvent>>? _onGuiInputAction; 
        public ScrollContainerAction OnGuiInput(Action<InputEvent> action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onGuiInputAction, "gui_input", nameof(_GodotSignalGuiInput), action, oneShot, deferred);
            return this;
        }

        public ScrollContainerAction RemoveOnGuiInput(Action<InputEvent> action) {
            RemoveSignal(_onGuiInputAction, "gui_input", nameof(_GodotSignalGuiInput), action);
            return this;
        }

        private ScrollContainerAction _GodotSignalGuiInput(InputEvent @event) {
            ExecuteSignal(_onGuiInputAction, @event);
            return this;
        }

        private List<Action>? _onHideAction; 
        public ScrollContainerAction OnHide(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onHideAction, "hide", nameof(_GodotSignalHide), action, oneShot, deferred);
            return this;
        }

        public ScrollContainerAction RemoveOnHide(Action action) {
            RemoveSignal(_onHideAction, "hide", nameof(_GodotSignalHide), action);
            return this;
        }

        private ScrollContainerAction _GodotSignalHide() {
            ExecuteSignal(_onHideAction);
            return this;
        }

        private List<Action>? _onItemRectChangedAction; 
        public ScrollContainerAction OnItemRectChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onItemRectChangedAction, "item_rect_changed", nameof(_GodotSignalItemRectChanged), action, oneShot, deferred);
            return this;
        }

        public ScrollContainerAction RemoveOnItemRectChanged(Action action) {
            RemoveSignal(_onItemRectChangedAction, "item_rect_changed", nameof(_GodotSignalItemRectChanged), action);
            return this;
        }

        private ScrollContainerAction _GodotSignalItemRectChanged() {
            ExecuteSignal(_onItemRectChangedAction);
            return this;
        }

        private List<Action>? _onMinimumSizeChangedAction; 
        public ScrollContainerAction OnMinimumSizeChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMinimumSizeChangedAction, "minimum_size_changed", nameof(_GodotSignalMinimumSizeChanged), action, oneShot, deferred);
            return this;
        }

        public ScrollContainerAction RemoveOnMinimumSizeChanged(Action action) {
            RemoveSignal(_onMinimumSizeChangedAction, "minimum_size_changed", nameof(_GodotSignalMinimumSizeChanged), action);
            return this;
        }

        private ScrollContainerAction _GodotSignalMinimumSizeChanged() {
            ExecuteSignal(_onMinimumSizeChangedAction);
            return this;
        }

        private List<Action>? _onModalClosedAction; 
        public ScrollContainerAction OnModalClosed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onModalClosedAction, "modal_closed", nameof(_GodotSignalModalClosed), action, oneShot, deferred);
            return this;
        }

        public ScrollContainerAction RemoveOnModalClosed(Action action) {
            RemoveSignal(_onModalClosedAction, "modal_closed", nameof(_GodotSignalModalClosed), action);
            return this;
        }

        private ScrollContainerAction _GodotSignalModalClosed() {
            ExecuteSignal(_onModalClosedAction);
            return this;
        }

        private List<Action>? _onMouseEnteredAction; 
        public ScrollContainerAction OnMouseEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMouseEnteredAction, "mouse_entered", nameof(_GodotSignalMouseEntered), action, oneShot, deferred);
            return this;
        }

        public ScrollContainerAction RemoveOnMouseEntered(Action action) {
            RemoveSignal(_onMouseEnteredAction, "mouse_entered", nameof(_GodotSignalMouseEntered), action);
            return this;
        }

        private ScrollContainerAction _GodotSignalMouseEntered() {
            ExecuteSignal(_onMouseEnteredAction);
            return this;
        }

        private List<Action>? _onMouseExitedAction; 
        public ScrollContainerAction OnMouseExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onMouseExitedAction, "mouse_exited", nameof(_GodotSignalMouseExited), action, oneShot, deferred);
            return this;
        }

        public ScrollContainerAction RemoveOnMouseExited(Action action) {
            RemoveSignal(_onMouseExitedAction, "mouse_exited", nameof(_GodotSignalMouseExited), action);
            return this;
        }

        private ScrollContainerAction _GodotSignalMouseExited() {
            ExecuteSignal(_onMouseExitedAction);
            return this;
        }

        private List<Action>? _onReadyAction; 
        public ScrollContainerAction OnReady(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onReadyAction, "ready", nameof(_GodotSignalReady), action, oneShot, deferred);
            return this;
        }

        public ScrollContainerAction RemoveOnReady(Action action) {
            RemoveSignal(_onReadyAction, "ready", nameof(_GodotSignalReady), action);
            return this;
        }

        private ScrollContainerAction _GodotSignalReady() {
            ExecuteSignal(_onReadyAction);
            return this;
        }

        private List<Action>? _onRenamedAction; 
        public ScrollContainerAction OnRenamed(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action, oneShot, deferred);
            return this;
        }

        public ScrollContainerAction RemoveOnRenamed(Action action) {
            RemoveSignal(_onRenamedAction, "renamed", nameof(_GodotSignalRenamed), action);
            return this;
        }

        private ScrollContainerAction _GodotSignalRenamed() {
            ExecuteSignal(_onRenamedAction);
            return this;
        }

        private List<Action>? _onResizedAction; 
        public ScrollContainerAction OnResized(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onResizedAction, "resized", nameof(_GodotSignalResized), action, oneShot, deferred);
            return this;
        }

        public ScrollContainerAction RemoveOnResized(Action action) {
            RemoveSignal(_onResizedAction, "resized", nameof(_GodotSignalResized), action);
            return this;
        }

        private ScrollContainerAction _GodotSignalResized() {
            ExecuteSignal(_onResizedAction);
            return this;
        }

        private List<Action>? _onScriptChangedAction; 
        public ScrollContainerAction OnScriptChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action, oneShot, deferred);
            return this;
        }

        public ScrollContainerAction RemoveOnScriptChanged(Action action) {
            RemoveSignal(_onScriptChangedAction, "script_changed", nameof(_GodotSignalScriptChanged), action);
            return this;
        }

        private ScrollContainerAction _GodotSignalScriptChanged() {
            ExecuteSignal(_onScriptChangedAction);
            return this;
        }

        private List<Action>? _onScrollEndedAction; 
        public ScrollContainerAction OnScrollEnded(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScrollEndedAction, "scroll_ended", nameof(_GodotSignalScrollEnded), action, oneShot, deferred);
            return this;
        }

        public ScrollContainerAction RemoveOnScrollEnded(Action action) {
            RemoveSignal(_onScrollEndedAction, "scroll_ended", nameof(_GodotSignalScrollEnded), action);
            return this;
        }

        private ScrollContainerAction _GodotSignalScrollEnded() {
            ExecuteSignal(_onScrollEndedAction);
            return this;
        }

        private List<Action>? _onScrollStartedAction; 
        public ScrollContainerAction OnScrollStarted(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onScrollStartedAction, "scroll_started", nameof(_GodotSignalScrollStarted), action, oneShot, deferred);
            return this;
        }

        public ScrollContainerAction RemoveOnScrollStarted(Action action) {
            RemoveSignal(_onScrollStartedAction, "scroll_started", nameof(_GodotSignalScrollStarted), action);
            return this;
        }

        private ScrollContainerAction _GodotSignalScrollStarted() {
            ExecuteSignal(_onScrollStartedAction);
            return this;
        }

        private List<Action>? _onSizeFlagsChangedAction; 
        public ScrollContainerAction OnSizeFlagsChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onSizeFlagsChangedAction, "size_flags_changed", nameof(_GodotSignalSizeFlagsChanged), action, oneShot, deferred);
            return this;
        }

        public ScrollContainerAction RemoveOnSizeFlagsChanged(Action action) {
            RemoveSignal(_onSizeFlagsChangedAction, "size_flags_changed", nameof(_GodotSignalSizeFlagsChanged), action);
            return this;
        }

        private ScrollContainerAction _GodotSignalSizeFlagsChanged() {
            ExecuteSignal(_onSizeFlagsChangedAction);
            return this;
        }

        private List<Action>? _onSortChildrenAction; 
        public ScrollContainerAction OnSortChildren(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onSortChildrenAction, "sort_children", nameof(_GodotSignalSortChildren), action, oneShot, deferred);
            return this;
        }

        public ScrollContainerAction RemoveOnSortChildren(Action action) {
            RemoveSignal(_onSortChildrenAction, "sort_children", nameof(_GodotSignalSortChildren), action);
            return this;
        }

        private ScrollContainerAction _GodotSignalSortChildren() {
            ExecuteSignal(_onSortChildrenAction);
            return this;
        }

        private List<Action>? _onTreeEnteredAction; 
        public ScrollContainerAction OnTreeEntered(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action, oneShot, deferred);
            return this;
        }

        public ScrollContainerAction RemoveOnTreeEntered(Action action) {
            RemoveSignal(_onTreeEnteredAction, "tree_entered", nameof(_GodotSignalTreeEntered), action);
            return this;
        }

        private ScrollContainerAction _GodotSignalTreeEntered() {
            ExecuteSignal(_onTreeEnteredAction);
            return this;
        }

        private List<Action>? _onTreeExitedAction; 
        public ScrollContainerAction OnTreeExited(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action, oneShot, deferred);
            return this;
        }

        public ScrollContainerAction RemoveOnTreeExited(Action action) {
            RemoveSignal(_onTreeExitedAction, "tree_exited", nameof(_GodotSignalTreeExited), action);
            return this;
        }

        private ScrollContainerAction _GodotSignalTreeExited() {
            ExecuteSignal(_onTreeExitedAction);
            return this;
        }

        private List<Action>? _onTreeExitingAction; 
        public ScrollContainerAction OnTreeExiting(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action, oneShot, deferred);
            return this;
        }

        public ScrollContainerAction RemoveOnTreeExiting(Action action) {
            RemoveSignal(_onTreeExitingAction, "tree_exiting", nameof(_GodotSignalTreeExiting), action);
            return this;
        }

        private ScrollContainerAction _GodotSignalTreeExiting() {
            ExecuteSignal(_onTreeExitingAction);
            return this;
        }

        private List<Action>? _onVisibilityChangedAction; 
        public ScrollContainerAction OnVisibilityChanged(Action action, bool oneShot = false, bool deferred = false) {
            AddSignal(ref _onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action, oneShot, deferred);
            return this;
        }

        public ScrollContainerAction RemoveOnVisibilityChanged(Action action) {
            RemoveSignal(_onVisibilityChangedAction, "visibility_changed", nameof(_GodotSignalVisibilityChanged), action);
            return this;
        }

        private ScrollContainerAction _GodotSignalVisibilityChanged() {
            ExecuteSignal(_onVisibilityChangedAction);
            return this;
        }
    }
}