using System;

namespace Betauer.Animation {
    public class InvalidAnimationException : Exception {
        public InvalidAnimationException(string message) : base(message) {
        }
    }
}