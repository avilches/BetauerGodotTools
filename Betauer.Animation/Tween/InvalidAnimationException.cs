using System;

namespace Betauer.Animation.Tween {
    public class InvalidAnimationException : Exception {
        public InvalidAnimationException(string message) : base(message) {
        }
    }
}