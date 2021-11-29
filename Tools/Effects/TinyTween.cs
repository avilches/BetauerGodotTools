// TinyTween.cs
//
// Copyright (c) 2013 Nick Gravelyn
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software
// and associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial
// portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

// For Unity, we can check all the individual platform defines to infer the environment.
// For XNA, you can either go into the project and add 'XNA' to the Build defines list or you can uncomment the following line.

using System;
using System.Collections.Generic;
using Godot;

namespace Tools.Effects {
    public enum TweenReturn {
        pause,
        next,
        restart
    }

    public delegate TweenReturn OnFinish();

    public interface IStep<T> {
        float GetCurrentValue();
        void Start();
        void Reset();
        void Pause();
        void Update(float time);
        TweenState GetState();
        TweenReturn OnComplete();
        IStep<float> CreateReverse();
        public delegate void OnUpdate(T value);
    }

    public abstract class TweenStep<T> : IStep<T> {
        private static readonly OnFinish OnCompleteNext = () => TweenReturn.next;

        private readonly OnFinish _onComplete;
        private IStep<T> _stepImplementation;

        protected TweenStep(OnFinish onComplete = null) {
            _onComplete = onComplete ?? OnCompleteNext;
        }

        public TweenReturn OnComplete() {
            return _onComplete();
        }

        public abstract IStep<float> CreateReverse();

        public abstract float GetCurrentValue();

        public abstract void Start();

        public abstract void Reset();

        public abstract void Pause();

        public abstract void Update(float time);

        public abstract TweenState GetState();
    }

    public class DelayTweenStep : TweenStep<float> {
        public float CurrentValue;
        public readonly float Delay;
        public override float GetCurrentValue() => CurrentValue;
        private float _consumed;
        public TweenState State { get; private set; } = TweenState.Running;

        public DelayTweenStep(float delay, float value, OnFinish onComplete = null) : base(onComplete) {
            Delay = delay;
            CurrentValue = value;
        }

        public override void Start() {
            State = TweenState.Running;
        }

        public override void Reset() {
            _consumed = 0;
            Start();
        }

        public override void Pause() {
            if (State == TweenState.Running) {
                State = TweenState.Paused;
            }
        }

        public override void Update(float time) {
            if (State != TweenState.Running) {
                return;
            }

            _consumed += time;
            if (_consumed >= Delay) {
                _consumed = Delay;
                State = TweenState.End;
            }
        }

        public override TweenState GetState() => State;

        public override IStep<float> CreateReverse() => new DelayTweenStep(Delay, CurrentValue, OnComplete);
    }

    public class TweenSequence : Node {
        private List<IStep<float>> steps = new List<IStep<float>>();

        private int _pos = 0;
        private bool _disposed = false;
        public bool Loop;
        private IStep<float>.OnUpdate _onUpdate;

        public TweenSequence(bool loop = false) {
            Loop = loop;
        }

        public TweenSequence Add(float start, float end, int duration, ScaleFunc scaleFunc,
            OnFinish onComplete = null) {
            if (!_disposed) {
                steps.Add(new FloatTweenStep(new FloatTween(start, end, duration, scaleFunc), onComplete));
            }
            return this;
        }

        public TweenSequence AutoUpdate(Node owner, IStep<float>.OnUpdate onUpdate) {
            if (!_disposed && onUpdate != null && _onUpdate == null) {
                _onUpdate = onUpdate;
                owner.AddChild(this);
            }
            return this;
        }

        public override void _PhysicsProcess(float delta) {
            if (!_disposed && _onUpdate != null) {
                _onUpdate(Update(delta));
            }
        }


        public TweenSequence AddDelay(float delay, float value, OnFinish onComplete = null) {
            if (!_disposed) {
                steps.Add(new DelayTweenStep(delay, value, onComplete));
            }
            return this;
        }

        public TweenSequence AddReverseAll() {
            return AddReverse(steps.Count);
        }

        public TweenSequence AddReverse(int elements = 1) {
            if (!_disposed) {
                elements = Godot.Mathf.Clamp(elements, 0, steps.Count);
                var part = steps.GetRange(steps.Count - elements, elements);
                part.Reverse();
                part.ForEach(input => steps.Add(input.CreateReverse()));
            }
            return this;
        }

        private IStep<float> CurrentStep => steps[_pos];

        private bool _end = false;

        public void Start() {
            if (_disposed) return;
            if (_end) {
                _end = false;
                _pos = 0;
            }

            CurrentStep.Start();
        }

        public void Restart() {
            if (_disposed) return;
            _pos = 0;
            _end = false;
            CurrentStep.Reset();
            CurrentStep.Start();
        }

        public void Pause() {
            if (_disposed) return;
            CurrentStep.Pause();
        }

        public float Update(float time) {
            if (!_disposed) {
                CurrentStep.Update(time);
                if (CurrentStep.GetState() == TweenState.End) {
                    NextStep();
                }
            }
            return CurrentStep.GetCurrentValue();
        }

        public void Dispose() {
            if (_disposed) return;
            _disposed = true;
            Pause();
            QueueFree();
        }

        public void NextStep() {
            if (_disposed) return;
            var ret = CurrentStep.OnComplete();
            switch (ret) {
                case TweenReturn.next:
                    _pos++;
                    if (!Loop && _pos == steps.Count) {
                        _pos--;
                        CurrentStep.Pause();
                        _end = true;
                        // Si no hace loop se queda pausado en el ultimo paso y se marca como end
                    } else {
                        // Hay loop o no estamos en la última vuelta -> next
                        if (_pos == steps.Count) {
                            _pos = 0;
                        }
                        CurrentStep.Reset();
                    }
                    break;
                case TweenReturn.restart:
                    _pos = 0;
                    CurrentStep.Reset();
                    break;
                case TweenReturn.pause:
                    _pos++;
                    if (_pos == steps.Count) {
                        _pos = 0;
                    }

                    CurrentStep.Reset();
                    CurrentStep.Pause();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public class FloatTweenStep : TweenStep<float> {
        private readonly FloatTween _tween;

        public FloatTweenStep(FloatTween tween, OnFinish onComplete = null) : base(onComplete) {
            _tween = tween;
        }

        public override float GetCurrentValue() => _tween.CurrentValue;

        public override void Start() => _tween.Start();

        public override void Reset() => _tween.Reset();

        public override void Pause() => _tween.Pause();

        public override void Update(float time) => _tween.Update(time);

        public override TweenState GetState() => _tween.State;

        public override IStep<float> CreateReverse() {
            return new FloatTweenStep(_tween.CreateReverse(), OnComplete);
        }
    }


    /// <summary>
    /// Takes in progress which is the percentage of the tween complete and returns
    /// the interpolation value that is fed into the lerp function for the tween.
    /// </summary>
    /// <remarks>
    /// Scale functions are used to define how the tween should occur. Examples would be linear,
    /// easing in quadratic, or easing out circular. You can implement your own scale function
    /// or use one of the many defined in the ScaleFuncs static class.
    /// </remarks>
    /// <param name="progress">The percentage of the tween complete in the range [0, 1].</param>
    /// <returns>The scale value used to lerp between the tween's start and end values</returns>
    public delegate float ScaleFunc(float progress);

    /// <summary>
    /// Standard linear interpolation function: "start + (end - start) * progress"
    /// </summary>
    /// <remarks>
    /// In a language like C++ we wouldn't need this delegate at all. Templates in C++ would allow us
    /// to simply write "start + (end - start) * progress" in the tween class and the compiler would
    /// take care of enforcing that the type supported those operators. Unfortunately C#'s generics
    /// are not so powerful so instead we must have the user provide the interpolation function.
    ///
    /// Thankfully frameworks like XNA and Unity provide lerp functions on their primitive math types
    /// which means that for most users there is nothing specific to do here. Additionally this file
    /// provides concrete implementations of tweens for vectors, colors, and more for XNA and Unity
    /// users, lessening the burden even more.
    /// </remarks>
    /// <typeparam name="T">The type to interpolate.</typeparam>
    /// <param name="start">The starting value.</param>
    /// <param name="end">The ending value.</param>
    /// <param name="progress">The interpolation progress.</param>
    /// <returns>The interpolated value, generally using "start + (end - start) * progress"</returns>
    public delegate T LerpFunc<T>(T start, T end, float progress);

    /// <summary>
    /// State of an ITween object
    /// </summary>
    public enum TweenState {
        Running,

        /// <summary>
        /// The tween is paused.
        /// </summary>
        Paused,

        /// <summary>
        /// The tween is stopped.
        /// </summary>
        End
    }

    /// <summary>
    /// The behavior to use when manually stopping a tween.
    /// </summary>
    public enum StopBehavior {
        /// <summary>
        /// Does not change the current value.
        /// </summary>
        AsIs,

        /// <summary>
        /// Forces the tween progress to the end value.
        /// </summary>
        ForceComplete
    }

    /// <summary>
    /// Interface for a tween object.
    /// </summary>
    public interface ITween {
        /// <summary>
        /// Gets the current state of the tween.
        /// </summary>
        TweenState State { get; }

        /// <summary>
        /// Pauses the tween.
        /// </summary>
        void Start();

        /// <summary>
        /// Pauses the tween.
        /// </summary>
        void Pause();

        /// <summary>
        /// Resumes the paused tween.
        /// </summary>
        // void Resume();

        /// <summary>
        /// Stops the tween.
        /// </summary>
        /// <param name="stopBehavior">The behavior to use to handle the stop.</param>
        void End(StopBehavior stopBehavior);

        /// <summary>
        /// Updates the tween.
        /// </summary>
        /// <param name="elapsedTime">The elapsed time to add to the tween.</param>
        void Update(float elapsedTime);
    }

    /// <summary>
    /// Interface for a tween object that handles a specific type.
    /// </summary>
    /// <typeparam name="T">The type to tween.</typeparam>
    public interface ITween<T> : ITween
        where T : struct {
        /// <summary>
        /// Gets the current value of the tween.
        /// </summary>
        T CurrentValue { get; }

        /// <summary>
        /// Config a tween.
        /// </summary>
        /// <param name="start">The start value.</param>
        /// <param name="end">The end value.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="scaleFunc">A function used to scale progress over time.</param>
        // void Config(T start, T end, float duration, ScaleFunc scaleFunc);
    }

    /// <summary>
    /// A concrete implementation of a tween object.
    /// </summary>
    /// <typeparam name="T">The type to tween.</typeparam>
    public class Tween<T> : ITween<T>
        where T : struct {
        protected readonly LerpFunc<T> lerpFunc;
        protected readonly T start;
        protected readonly T end;
        protected readonly float duration;
        protected readonly ScaleFunc scaleFunc;

        private float currentTime;
        private TweenState state;
        private T value;

        /// <summary>
        /// Gets the current time of the tween.
        /// </summary>
        public float CurrentTime {
            get { return currentTime; }
        }

        /// <summary>
        /// Gets the duration of the tween.
        /// </summary>
        public float Duration => duration;

        /// <summary>
        /// Gets the current state of the tween.
        /// </summary>
        public TweenState State => state;

        /// <summary>
        /// Gets the starting value of the tween.
        /// </summary>
        public T StartValue => start;

        /// <summary>
        /// Gets the ending value of the tween.
        /// </summary>
        public T EndValue => end;

        /// <summary>
        /// Gets the current value of the tween.
        /// </summary>
        public T CurrentValue => value;

        /// <summary>
        /// Initializes a new Tween with a given lerp function.
        /// </summary>
        /// <remarks>
        /// C# generics are good but not good enough. We need a delegate to know how to
        /// interpolate between the start and end values for the given type.
        /// </remarks>
        /// <param name="lerpFunc">The interpolation function for the tween type.</param>
        /// <param name="start">The start value.</param>
        /// <param name="end">The end value.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="scaleFunc">A function used to scale progress over time.</param>
        public Tween(LerpFunc<T> lerpFunc, T start, T end, float duration, ScaleFunc scaleFunc) {
            if (duration <= 0) {
                throw new ArgumentException("duration must be greater than 0");
            }

            this.scaleFunc = scaleFunc ?? throw new ArgumentNullException(nameof(scaleFunc));
            this.start = start;
            this.end = end;
            this.lerpFunc = lerpFunc;
            this.duration = duration;
            Reset();
        }

        public void Reset() {
            currentTime = 0;
            Start();
        }


        public void Start() {
            state = TweenState.Running;
            UpdateValue();
        }

        /// <summary>
        /// Pauses the tween.
        /// </summary>
        public void Pause() {
            if (state == TweenState.Running) {
                state = TweenState.Paused;
            }
        }

        /// <summary>
        /// Stops the tween.
        /// </summary>
        /// <param name="stopBehavior">The behavior to use to handle the stop.</param>
        public void End(StopBehavior stopBehavior) {
            state = TweenState.End;

            if (stopBehavior == StopBehavior.ForceComplete) {
                currentTime = duration;
                UpdateValue();
            }
        }

        /// <summary>
        /// Updates the tween.
        /// </summary>
        /// <param name="elapsedTime">The elapsed time to add to the tween.</param>
        public void Update(float elapsedTime) {
            if (state != TweenState.Running) {
                return;
            }

            currentTime += elapsedTime;
            if (currentTime >= duration) {
                currentTime = duration;
                state = TweenState.End;
            }

            UpdateValue();
        }

        /// <summary>
        /// Helper that uses the current time, duration, and delegates to update the current value.
        /// </summary>
        private void UpdateValue() {
            value = lerpFunc(start, end, scaleFunc(currentTime / duration));
        }
    }

    /// <summary>
    /// Object used to tween float values.
    /// </summary>
    public class FloatTween : Tween<float> {
        private static float LerpFloat(float start, float end, float progress) {
            return start + (end - start) * progress;
        }

        // Static readonly delegate to avoid multiple delegate allocations
        private static readonly LerpFunc<float> LerpFunc = LerpFloat;

        /// <summary>
        /// Initializes a new FloatTween instance.
        /// </summary>
        public FloatTween(float start, float end, float duration, ScaleFunc scaleFunc) : base(LerpFunc, start, end,
            duration, scaleFunc) {
        }

        public FloatTween CreateReverse() {
            return new FloatTween(end, start, duration, scaleFunc);
        }
    }

// If XNA or Unity we can leverage their types to create more tween types to simplify usage.
#if XNA
    /// <summary>
    /// Object used to tween Vector2 values.
    /// </summary>
    public class Vector2Tween : Tween<Vector2>
    {
        // Static readonly delegate to avoid multiple delegate allocations
        private static readonly LerpFunc<Vector2> LerpFunc = Mathf.Lerp();

        /// <summary>
        /// Initializes a new Vector2Tween instance.
        /// </summary>
        public Vector2Tween() : base(LerpFunc) { }
    }

    /// <summary>
    /// Object used to tween Vector3 values.
    /// </summary>
    public class Vector3Tween : Tween<Vector3>
    {
        // Static readonly delegate to avoid multiple delegate allocations
        private static readonly LerpFunc<Vector3> LerpFunc = Vector3.Lerp;

        /// <summary>
        /// Initializes a new Vector3Tween instance.
        /// </summary>
        public Vector3Tween() : base(LerpFunc) { }
    }

    /// <summary>
    /// Object used to tween Vector4 values.
    /// </summary>
    public class Vector4Tween : Tween<Vector4>
    {
        // Static readonly delegate to avoid multiple delegate allocations
        private static readonly LerpFunc<Vector4> LerpFunc = Vector4.Lerp;

        /// <summary>
        /// Initializes a new Vector4Tween instance.
        /// </summary>
        public Vector4Tween() : base(LerpFunc) { }
    }

    /// <summary>
    /// Object used to tween Color values.
    /// </summary>
    public class ColorTween : Tween<Color>
    {
        // Static readonly delegate to avoid multiple delegate allocations
        private static readonly LerpFunc<Color> LerpFunc = Color.Lerp;

        /// <summary>
        /// Initializes a new ColorTween instance.
        /// </summary>
        public ColorTween() : base(LerpFunc) { }
    }

    /// <summary>
    /// Object used to tween Quaternion values.
    /// </summary>
    public class QuaternionTween : Tween<Quaternion>
    {
        // Static readonly delegate to avoid multiple delegate allocations
        private static readonly LerpFunc<Quaternion> LerpFunc = Quaternion.Lerp;

        /// <summary>
        /// Initializes a new QuaternionTween instance.
        /// </summary>
        public QuaternionTween() : base(LerpFunc) { }
    }
#endif

    /// <summary>
    /// Defines a set of premade scale functions for use with tweens.
    /// </summary>
    /// <remarks>
    /// To avoid excess allocations of delegates, the public members of ScaleFuncs are already
    /// delegates that reference private methods.
    ///
    /// Implementations based on http://theinstructionlimit.com/flash-style-tweeneasing-functions-in-c
    /// which are based on http://www.robertpenner.com/easing/
    /// </remarks>
    public static class ScaleFuncs {
        /// <summary>
        /// A linear progress scale function.
        /// </summary>
        public static readonly ScaleFunc Linear = LinearImpl;

        /// <summary>
        /// A quadratic (x^2) progress scale function that eases in.
        /// </summary>
        public static readonly ScaleFunc QuadraticEaseIn = QuadraticEaseInImpl;

        /// <summary>
        /// A quadratic (x^2) progress scale function that eases out.
        /// </summary>
        public static readonly ScaleFunc QuadraticEaseOut = QuadraticEaseOutImpl;

        /// <summary>
        /// A quadratic (x^2) progress scale function that eases in and out.
        /// </summary>
        public static readonly ScaleFunc QuadraticEaseInOut = QuadraticEaseInOutImpl;

        /// <summary>
        /// A cubic (x^3) progress scale function that eases in.
        /// </summary>
        public static readonly ScaleFunc CubicEaseIn = CubicEaseInImpl;

        /// <summary>
        /// A cubic (x^3) progress scale function that eases out.
        /// </summary>
        public static readonly ScaleFunc CubicEaseOut = CubicEaseOutImpl;

        /// <summary>
        /// A cubic (x^3) progress scale function that eases in and out.
        /// </summary>
        public static readonly ScaleFunc CubicEaseInOut = CubicEaseInOutImpl;

        /// <summary>
        /// A quartic (x^4) progress scale function that eases in.
        /// </summary>
        public static readonly ScaleFunc QuarticEaseIn = QuarticEaseInImpl;

        /// <summary>
        /// A quartic (x^4) progress scale function that eases out.
        /// </summary>
        public static readonly ScaleFunc QuarticEaseOut = QuarticEaseOutImpl;

        /// <summary>
        /// A quartic (x^4) progress scale function that eases in and out.
        /// </summary>
        public static readonly ScaleFunc QuarticEaseInOut = QuarticEaseInOutImpl;

        /// <summary>
        /// A quintic (x^5) progress scale function that eases in.
        /// </summary>
        public static readonly ScaleFunc QuinticEaseIn = QuinticEaseInImpl;

        /// <summary>
        /// A quintic (x^5) progress scale function that eases out.
        /// </summary>
        public static readonly ScaleFunc QuinticEaseOut = QuinticEaseOutImpl;

        /// <summary>
        /// A quintic (x^5) progress scale function that eases in and out.
        /// </summary>
        public static readonly ScaleFunc QuinticEaseInOut = QuinticEaseInOutImpl;

        /// <summary>
        /// A sinusoidal progress scale function that eases in.
        /// </summary>
        public static readonly ScaleFunc SineEaseIn = SineEaseInImpl;

        /// <summary>
        /// A sinusoidal progress scale function that eases out.
        /// </summary>
        public static readonly ScaleFunc SineEaseOut = SineEaseOutImpl;

        /// <summary>
        /// A sinusoidal progress scale function that eases in and out.
        /// </summary>
        public static readonly ScaleFunc SineEaseInOut = SineEaseInOutImpl;

        private const float Pi = (float) Math.PI;
        private const float HalfPi = Pi / 2f;

        private static float LinearImpl(float progress) {
            return progress;
        }

        private static float QuadraticEaseInImpl(float progress) {
            return EaseInPower(progress, 2);
        }

        private static float QuadraticEaseOutImpl(float progress) {
            return EaseOutPower(progress, 2);
        }

        private static float QuadraticEaseInOutImpl(float progress) {
            return EaseInOutPower(progress, 2);
        }

        private static float CubicEaseInImpl(float progress) {
            return EaseInPower(progress, 3);
        }

        private static float CubicEaseOutImpl(float progress) {
            return EaseOutPower(progress, 3);
        }

        private static float CubicEaseInOutImpl(float progress) {
            return EaseInOutPower(progress, 3);
        }

        private static float QuarticEaseInImpl(float progress) {
            return EaseInPower(progress, 4);
        }

        private static float QuarticEaseOutImpl(float progress) {
            return EaseOutPower(progress, 4);
        }

        private static float QuarticEaseInOutImpl(float progress) {
            return EaseInOutPower(progress, 4);
        }

        private static float QuinticEaseInImpl(float progress) {
            return EaseInPower(progress, 5);
        }

        private static float QuinticEaseOutImpl(float progress) {
            return EaseOutPower(progress, 5);
        }

        private static float QuinticEaseInOutImpl(float progress) {
            return EaseInOutPower(progress, 5);
        }

        private static float EaseInPower(float progress, int power) {
            return (float) Math.Pow(progress, power);
        }

        private static float EaseOutPower(float progress, int power) {
            int sign = power % 2 == 0 ? -1 : 1;
            return (float) (sign * (Math.Pow(progress - 1, power) + sign));
        }

        private static float EaseInOutPower(float progress, int power) {
            progress *= 2;
            if (progress < 1) {
                return (float) Math.Pow(progress, power) / 2f;
            } else {
                int sign = power % 2 == 0 ? -1 : 1;
                return (float) (sign / 2.0 * (Math.Pow(progress - 2, power) + sign * 2));
            }
        }

        private static float SineEaseInImpl(float progress) {
            return (float) Math.Sin(progress * HalfPi - HalfPi) + 1;
        }

        private static float SineEaseOutImpl(float progress) {
            return (float) Math.Sin(progress * HalfPi);
        }

        private static float SineEaseInOutImpl(float progress) {
            return (float) (Math.Sin(progress * Pi - HalfPi) + 1) / 2;
        }
    }
}