using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Animation {
    public interface ITweenSequence {
        public ICollection<ICollection<ITweener>> TweenList { get; }
        public Node DefaultTarget { get; }
        public int Loops { get; }
        public float Speed { get; }
        public float Duration { get; }
        public Tween.TweenProcessMode ProcessMode { get; }
    }

    /**
     * A readonly ITweenSequence
     * Pay attention the internal _tweenList could be mutable.
     *
     * To protect this, when you use a template with ImportTemplate, all data is copied and the flag
     * _importedFromTemplate is set to true, so, any future call to the AddTweener() will make a new copy of the
     * internal collection
     */
    public class TweenSequenceTemplate : ITweenSequence {
        private readonly ICollection<ICollection<ITweener>> _tweenList;
        private readonly Node _defaultTarget;
        private readonly float _duration;
        private readonly int _loops;
        private readonly float _speed;
        private readonly Tween.TweenProcessMode _processMode;

        public ICollection<ICollection<ITweener>> TweenList => _tweenList;
        public Node DefaultTarget => _defaultTarget;
        public float Duration => _duration;
        public int Loops => _loops;

        public float Speed => _speed;

        // public bool Template => true;
        public Tween.TweenProcessMode ProcessMode => _processMode;

        public TweenSequenceTemplate(ICollection<ICollection<ITweener>> tweenList, Node defaultTarget,
            float duration, int loops, float speed, Tween.TweenProcessMode processMode) {
            _tweenList = tweenList;
            _defaultTarget = defaultTarget;
            _duration = duration;
            _loops = loops;
            _speed = speed;
            _processMode = processMode;
        }

        public static TweenSequenceTemplate Create(ITweenSequence from) {
            return new TweenSequenceTemplate(from.TweenList, from.DefaultTarget,
                from.Duration, from.Loops, from.Speed, from.ProcessMode);
        }
    }

    public class TweenSequence : ITweenSequence {
        public ICollection<ICollection<ITweener>> TweenList { get; protected set; }
        public Node DefaultTarget { get; private set; }
        public float Duration { get; protected set; } = -1.0f;
        public int Loops { get; protected set; } = 1;
        public float Speed { get; protected set; } = 1.0f;
        private protected bool _importedFromTemplate = false;
        public Tween.TweenProcessMode ProcessMode { get; protected set; } = Tween.TweenProcessMode.Idle;

        public void ImportTemplate(TweenSequenceTemplate tweenSequence, Node defaultTarget, float duration = -1) {
            TweenList = tweenSequence.TweenList;
            DefaultTarget = defaultTarget ?? tweenSequence.DefaultTarget;
            Loops = tweenSequence.Loops;
            Speed = tweenSequence.Speed;
            Duration = duration > 0 ? duration : tweenSequence.Duration;
            ProcessMode = tweenSequence.ProcessMode;
            _importedFromTemplate = true;
        }
    }

    public class TweenSequenceBuilder : AbstractTweenSequenceBuilder<TweenSequenceBuilder> {
        private TweenSequenceBuilder(ICollection<ICollection<ITweener>> tweenList) : base(tweenList) {
        }

        public static TweenSequenceBuilder Create() {
            var tweenSequenceBuilder = new TweenSequenceBuilder(new SimpleLinkedList<ICollection<ITweener>>());
            return tweenSequenceBuilder;
        }

        public TweenSequenceTemplate BuildTemplate() {
            return TweenSequenceTemplate.Create(this);
        }

        public SingleSequencePlayer CreatePlayer(Node node) {
            return SingleSequencePlayer.With(node, this);
        }

        public SingleSequencePlayer Play(Node node) {
            return CreatePlayer(node).Start();
        }
    }

    public abstract class AbstractTweenSequenceBuilder<TBuilder> : TweenSequence where TBuilder : class {
        private bool _parallel = false;

        internal AbstractTweenSequenceBuilder(ICollection<ICollection<ITweener>> tweenList) {
            TweenList = tweenList;
        }

        public TBuilder Parallel() {
            _parallel = true;
            return this as TBuilder;
        }

        public TBuilder Pause(float delay) {
            AddTweener(new PauseTweener(delay));
            return this as TBuilder;
        }

        public TBuilder Callback(Action callback, float delay = 0) {
            AddTweener(new CallbackTweener(delay, callback));
            return this as TBuilder;
        }

        public TBuilder SetSpeed(float speed) {
            Speed = speed;
            return this as TBuilder;
        }

        public TBuilder SetDuration(float duration) {
            // TODO: id duration is defined, every single duration should be fit on it
            Duration = duration;
            return this as TBuilder;
        }

        public TBuilder SetProcessMode(Tween.TweenProcessMode processMode) {
            ProcessMode = processMode;
            return this as TBuilder;
        }

        public TBuilder SetLoops(int maxLoops) {
            Loops = maxLoops;
            return this as TBuilder;
        }

        public TBuilder SetInfiniteLoops() {
            return SetLoops(-1);
        }

        public bool IsInfiniteLoops() {
            return Loops == -1;
        }

        public PropertyKeyStepToBuilder<TProperty, TBuilder> AnimateSteps<TProperty>(Node target = null,
            IProperty<TProperty> property = null, Easing easing = null) {
            var tweener = new PropertyKeyStepToBuilder<TProperty, TBuilder>(this, target, property, easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> AnimateStepsBy<TProperty>(Node target = null,
            IProperty<TProperty> property = null, Easing easing = null) {
            var tweener = new PropertyKeyStepOffsetBuilder<TProperty, TBuilder>(this, target, property, easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyStepOffsetBuilder<TProperty, TBuilder> AnimateRelativeSteps<TProperty>(Node target = null,
            IProperty<TProperty> property = null, Easing easing = null) {
            var tweener = new PropertyKeyStepOffsetBuilder<TProperty, TBuilder>(this, target, property, easing, true);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentToBuilder<TProperty, TBuilder> AnimateKeys<TProperty>(Node target = null,
            IProperty<TProperty> property = null, Easing easing = null) {
            var tweener = new PropertyKeyPercentToBuilder<TProperty, TBuilder>(this, target, property, easing);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> AnimateKeysBy<TProperty>(Node target = null,
            IProperty<TProperty> property = null, Easing easing = null) {
            var tweener = new PropertyKeyPercentOffsetBuilder<TProperty, TBuilder>(this, target, property, easing, false);
            AddTweener(tweener);
            return tweener;
        }

        public PropertyKeyPercentOffsetBuilder<TProperty, TBuilder> AnimateRelativeKeys<TProperty>(Node target = null,
            IProperty<TProperty> property = null, Easing easing = null) {
            var tweener = new PropertyKeyPercentOffsetBuilder<TProperty, TBuilder>(this, target, property, easing, true);
            AddTweener(tweener);
            return tweener;
        }

        private void AddTweener(ITweener tweener) {
            if (_importedFromTemplate) {
                var tweenListCloned = new SimpleLinkedList<ICollection<ITweener>>(TweenList);
                if (_parallel) {
                    var lastParallelCloned = new SimpleLinkedList<ITweener>(tweenListCloned.Last());
                    tweenListCloned.RemoveEnd();
                    tweenListCloned.Add(lastParallelCloned);
                }
                TweenList = tweenListCloned;
                _importedFromTemplate = false;
            }
            if (_parallel) {
                TweenList.Last().Add(tweener);
                _parallel = false;
            } else {
                TweenList.Add(new SimpleLinkedList<ITweener> { tweener });
            }
        }
    }
}