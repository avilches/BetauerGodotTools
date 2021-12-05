using System;
using Tools.Effects;
using Object = Godot.Object;

namespace Tools {
    public class TweenStatus : Object {
        protected TweenStack _tweenStack;
        protected Logger _logger;
        public readonly string Name;
        public readonly TweenSequence Sequence;
        public bool Playing { get; private set; } = false;

        private protected Callback _onStart;
        private protected Callback _onEnd;

        protected TweenStatus(TweenStack tweenStack, Logger logger, string name, TweenSequence sequence) {
            _tweenStack = tweenStack;
            _logger = logger;
            Name = name;
            Sequence = sequence;
        }

        internal void ExecuteOnStart() {
            _logger.Debug("Start " + GetType().Name + " \"" + Name + "\"");
            Playing = true;
            _onStart?.Invoke();
            Sequence.Reset().Start();
        }

        internal void ExecuteOnEnd() {
            _logger.Debug("End " + GetType().Name + " \"" + Name + "\"");
            _onEnd?.Invoke();
            Playing = false;
            Sequence.Stop();
        }
    }

    public class LoopTween : TweenStatus {
        protected internal LoopTween(TweenStack tweenStack, Logger logger, string name, TweenSequence sequence) : base(
            tweenStack, logger, name, sequence) {
        }

        public LoopTween OnStart(Callback callback) {
            _onStart = callback;
            return this;
        }

        public LoopTween OnEnd(Callback callback) {
            _onEnd = callback;
            return this;
        }

        public void Stop() {
            if (_tweenStack.IsPlayingLoop(Name)) {
                _tweenStack.StopLoopTween();
            }
        }

        public void PlayLoop() {
            _tweenStack.PlayLoopTween(this);
        }
    }

    public class OnceTween : TweenStatus {
        public bool CanBeInterrupted;
        public bool KillPreviousTween;

        protected internal OnceTween(TweenStack tweenStack, Logger logger, string name, TweenSequence sequence,
            bool canBeInterrupted, bool killPreviousTween) :
            base(tweenStack, logger, name, sequence) {
            CanBeInterrupted = canBeInterrupted;
            KillPreviousTween = killPreviousTween;
        }

        public OnceTween OnStart(Callback callback) {
            _onStart = callback;
            return this;
        }

        public OnceTween OnEnd(Callback callback) {
            _onEnd = callback;
            return this;
        }

        public void Stop(bool killPreviousAnimation = false) {
            if (_tweenStack.IsPlayingOnce(Name)) {
                _tweenStack.StopOnceTween(killPreviousAnimation);
            }
        }

        public void PlayOnce(bool killPreviousTween = false) {
            _tweenStack.PlayOnceTween(this, killPreviousTween);
        }
    }


    public class TweenStack : Object /* needed to listen signals */ {
        private readonly System.Collections.Generic.Dictionary<string, LoopTween> _loopTweens =
            new System.Collections.Generic.Dictionary<string, LoopTween>();

        private readonly System.Collections.Generic.Dictionary<string, OnceTween> _onceTweens =
            new System.Collections.Generic.Dictionary<string, OnceTween>();

        private LoopTween _currentLoopTween;
        private OnceTween _currentOnceTween;
        private readonly Logger _logger;

        public TweenStack(string name) {
            _logger = LoggerFactory.GetLogger(name, "TweenStack");
        }

        public LoopTween AddLoopTween(string name, TweenSequence tweenSequence) {
            tweenSequence.SetInfiniteLoops();
            var loopTweenStatus = new LoopTween(this, _logger, name, tweenSequence);
            _loopTweens.Add(name, loopTweenStatus);
            return loopTweenStatus;
        }

        public OnceTween AddOnceTween(string name, TweenSequence tweenSequence, bool canBeInterrupted = false,
            bool killPreviousTween = false) {
            if (tweenSequence.IsInfiniteLoop()) {
                tweenSequence.SetLoops(1);
            } else {
                // It has a finite amount of loops, which is correct.
            }

            tweenSequence.SetAutokill(false);
            tweenSequence.AddOnFinishTween(OnceTweenFinished);
            var onceTweenStatus =
                new OnceTween(this, _logger, name, tweenSequence, canBeInterrupted, killPreviousTween);
            _onceTweens.Add(name, onceTweenStatus);
            return onceTweenStatus;
        }

        public LoopTween GetLoopTween(string name) => _loopTweens[name];
        public OnceTween GetOnceTween(string name) => _onceTweens[name];
        public LoopTween GetPlayingLoop() => _currentLoopTween;
        public OnceTween GetPlayingOnce() => _currentOnceTween;
        public bool IsPlayingLoop(string name) => _currentOnceTween == null && _currentLoopTween?.Name == name;
        public bool IsPlayingLoop() => _currentOnceTween == null && _currentLoopTween != null;
        public bool IsPlayingOnce(string name) => _currentOnceTween?.Name == name;
        public bool IsPlayingOnce() => _currentOnceTween != null;
        public bool IsPlaying(string name) => _currentLoopTween?.Name == name || _currentLoopTween?.Name == name;
        public void PlayLoopTween(string name) => PlayLoopTween(_loopTweens[name]);

        public OnceTween PlayOnceTween(string name, bool killPreviousTween = false) =>
            PlayOnceTween(_onceTweens[name], killPreviousTween);


        /*
         * currentLoop currentOnce newLoop
         * null        null        null       currentLoop = null (nothing to do)
         * p(a)        null        null       stop(a), currentLoop = null
         * null        p(x)        null       currentLoop = null (nothing to do)
         * a           p(x)        null       currentLoop = null
         */
        public void StopLoopTween() {
            // Stop and remove the currentLoopAnimation
            if (_currentOnceTween == null) {
                _currentLoopTween.ExecuteOnEnd();
            }
            _currentLoopTween = null;
        }

        /*
        * null        null        a          currentLoop = p(a)
        * p(a)        null        a          - nothing to do -
        * null        p(x)        a          currentLoop = a
        * a           p(x)        a          currentLoop = a (nothing to do)
        *
        * null        null        b          currentLoop = p(b)
        * p(a)        null        b          stop(a), currentLoop = p(b)
        * null        p(x)        b          currentLoop = b
        * a           p(x)        b          currentLoop = b
        */
        protected internal void PlayLoopTween(LoopTween loopTween) {
            if (_currentLoopTween == null || _currentLoopTween.Name != loopTween.Name) {
                _logger.Debug("PlayLoop \"" + loopTween.Name + "\"");
                if (_currentOnceTween == null) {
                    _currentLoopTween?.ExecuteOnEnd();
                    _currentLoopTween = loopTween;
                    _currentLoopTween.ExecuteOnStart();
                } else {
                    _currentLoopTween = loopTween;
                }
            }
        }

        /*
        * currentLoop currentOnce newOnce
        * null        null        null       * currentOnce = null (nothing to do)
        * p(a)        null        null       * currentOnce = null (nothing to do)
        * null        p(x)        null       stop(x), currentOnce = null
        * a           p(x)        null       stop(x), play(a), currentOnce = null
        */
        public void StopOnceTween(bool killPreviousTween = false) {
            if (_currentOnceTween == null) return;
            if (_currentOnceTween.CanBeInterrupted || killPreviousTween) {
                _currentOnceTween.ExecuteOnEnd();
                _currentOnceTween = null;
                _currentLoopTween?.ExecuteOnStart();
            }
        }

        /*
        * null        null        a          currentOnce = p(a)
        * p(x)        null        a          stop(x), currentOnce = p(a)
        * null        p(a)        a          * currentOnce = p(a) (nothing to do)
        * x           p(a)        a          * currentOnce = p(a) (nothing to do)
        *
        * null        null        b          currentOnce = p(b)
        * p(x)        null        b          stop(x), currentOnce = p(b)
        * null        p(a)        b          stop(a), currentOnce = p(b)
        * x           p(a)        b          stop(a), currentOnce = p(b)
        */
        protected internal OnceTween PlayOnceTween(OnceTween newOnceTween,
            bool killPreviousTween) {
            if (_currentOnceTween == null || _currentOnceTween.CanBeInterrupted ||
                newOnceTween.KillPreviousTween || killPreviousTween) {
                if (_currentOnceTween != null) {
                    _logger.Debug("PlayOnce \"" + newOnceTween.Name + "\". Interrupting current once " +
                                  _currentOnceTween.Name + "\")");
                    _currentOnceTween.ExecuteOnEnd();
                } else {
                    _logger.Debug("PlayOnce \"" + newOnceTween.Name + "\"");
                }
                _currentLoopTween?.ExecuteOnEnd();
                _currentOnceTween = newOnceTween;
                _currentOnceTween.ExecuteOnStart();
            } else if (!_currentOnceTween.CanBeInterrupted) {
                _logger.Error("PlayOnce \"" + newOnceTween.Name +
                              "\" not played: current Once animation \"" +
                              _currentOnceTween.Name + "\" can't be interrupted)");
            }
            return newOnceTween;
        }

        public void OnceTweenFinished() {
            _logger.Debug("OnceTweenFinished: \"" + _currentOnceTween.Name + "\"");
            _currentOnceTween.ExecuteOnEnd();
            _currentOnceTween = null;
            _currentLoopTween?.ExecuteOnStart();
        }
    }
}