using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Godot;
using Object = Godot.Object;

namespace Betauer.Animation {
    /// <summary>
    /// A Tween compatible with C# lambdas, aka, Actions.
    /// Instead of Tween.InterpolateCallback(object, 0f, "method") you can use:
    /// ScheduleCallback(0f, () => {})
    ///
    /// Instead of Tween.InterpolateMethod(object, "method", 0, 12, ...) you can use:
    /// InterpolateAction(0, 12, ..., (val) => { })
    /// 
    /// </summary>
    public class TweenActionCallback : Tween {
        private readonly Dictionary<int, Action> _actions = new Dictionary<int, Action>();
        public List<TweenExecution> e = new List<TweenExecution>();

        public void ScheduleCallback(float delay, Action callback) {
            e.Add(new TweenExecutionCallback(delay, callback));
        }

        public void Add(TweenExecution runStep) {
            e.Add(runStep);
        }

        public override void _PhysicsProcess(float delta) {
            foreach (var tweenExecution in e) tweenExecution.Update(delta);
            e.RemoveAll(execution => execution.IsFinished());
        }

        public class TweenExecution {
            public enum TweenState {
                Play,
                Stop,
                End
            }

            protected float Time = 0;
            protected float Delay;

            public TweenState State = TweenState.Play;

            public virtual void Update(float delta) {
            }

            public bool IsFinished() => State == TweenState.End;
        }

        public class TweenExecutionCallback : TweenExecution {
            private Action _action;

            public TweenExecutionCallback(float delay, Action action) {
                Delay = delay;
                _action = action;
            }

            public override void Update(float delta) {
                if (State != TweenState.Play) return;
                Time += delta;
                if (Time < Delay) return;
                _action.Invoke();
                State = TweenState.End;
            }
        }

        public class InterpolateMethodAction<T> : TweenExecution {
            private T _from;
            private T _to;
            private float _duration;
            private float _endTime;
            private Easing _easing;
            private readonly Action<T> _action;

            public InterpolateMethodAction(T from, T to, float duration, Easing easing, float delay, Action<T> action) {
                _from = from;
                _to = to;
                _duration = duration;
                _easing = easing;
                Delay = delay;
                _action = action;
                _endTime = duration + delay;
            }

            public override void Update(float delta) {
                if (State != TweenState.Play) {
                    // Console.WriteLine($"{State} From/To: {_from}/{_to} | Delta:+{delta} From/To: {Delay}/{_endTime} Time:{Time} | "+State);             
                    return;
                }
                Time += delta;
                if (Time < Delay) {
                    // Console.WriteLine($"{State}  From/To: {_from}/{_to} | Delta:+{delta} From/To: {Delay}/{_endTime} Time:{Time} | Waiting until time > Delay {Delay}");             
                    return;
                }
                if (Time > _endTime) {
                    State = TweenState.End;
                    Time = _endTime;
                    // 0  20
                    // Time = 5,   0.25
                }
                var animationTime = Time - Delay;
                var weight = animationTime / _duration;
                var t = Mathf.Lerp(0, _duration, weight);
                var y = _easing.GetY(t);
                var value = (T)VariantHelper.LerpVariant(_from, _to, y);
                // Console.WriteLine($"Lerp(0, {_duration}, {weight}) = {t}. y = {y}. Value {value}");
                
                Console.WriteLine($"{State}  From/To: {_from}/{_to} | Delta:+{delta:0.00} From/To: {Delay:0.00}/{_endTime:0.00} (duration: {_duration:0.00} Time:{Time:0.00} | t:{t} y:{y} Value: {value}");             
                
                _action.Invoke(value);
            }
        }

        public class TweenExecutionChain : TweenExecution {
            private readonly List<TweenExecution> _tweenExecutions;
            public TweenExecutionChain(List<TweenExecution> tweenExecutions) {
                _tweenExecutions = tweenExecutions;
            }

            private int _pos = 0;
            private float _accumulated = 0;
            private float _nextAccumulated = 0;
            public override void Update(float delta) {
                if (_pos >= _tweenExecutions.Count) {
                    State = TweenState.End;
                    return;
                }
                _nextAccumulated += delta;
                var te = _tweenExecutions[_pos];
                te.Update(_accumulated + delta);
                if (te.IsFinished()) {
                    _pos++;
                    _accumulated = _nextAccumulated;
                    _nextAccumulated = 0;
                }
            }
        }
    }
}