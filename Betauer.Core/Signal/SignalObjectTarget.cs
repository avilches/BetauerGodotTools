using System;
using Betauer.Memory;
using Object = Godot.Object;

namespace Betauer.Signal {
    internal abstract class SignalObjectTarget : Object, IObjectConsumer {
        public readonly Object Origin;
        public readonly bool OneShot;

        protected SignalObjectTarget(Object origin, bool oneShot) {
            Origin = origin;
            OneShot = oneShot;
            Watch();
        }

        protected void AfterCall() {
            if (OneShot) CallDeferred("free");
        }

        public bool Consume(bool force) {
            if (force || !IsInstanceValid(Origin)) {
                if (IsInstanceValid(this)) CallDeferred("free");
                return true;
            }
            return false;
        }

        public void Unwatch() {
            DefaultObjectWatcherRunner.Instance.Remove(this);
        }

        public void Watch() {
            DefaultObjectWatcherRunner.Instance.Add(this);
        }
    }
        internal abstract class SignalObjectTarget<T> : SignalObjectTarget {
        protected readonly T Action;

        protected SignalObjectTarget(Object origin, T action, bool oneShot): base(origin, oneShot) {
            Action = action;
        }
    }

    internal class SignalObjectTargetAction : SignalObjectTarget<Action> {
        public SignalObjectTargetAction(Object origin, Action action, bool oneShot) : base(origin, action, oneShot) {
        }

        public void GodotCall() {
            try {
                Action();
            } catch (Exception e) {
                throw e;
            } finally {
                AfterCall();
            }
        }
    }

    internal class SignalObjectTargetAction<T1> : SignalObjectTarget<Action<T1>> {
        public SignalObjectTargetAction(Object origin, Action<T1> action, bool oneShot) : base(origin, action, oneShot) {
        }

        public void GodotCall(T1 v1) {
            try {
                Action(v1);
            } catch (Exception e) {
                throw e;
            } finally {
                AfterCall();
            }
        }
    }

    internal class SignalObjectTargetAction<T1, T2> : SignalObjectTarget<Action<T1, T2>> {
        public SignalObjectTargetAction(Object origin, Action<T1, T2> action, bool oneShot) : base(origin, action, oneShot) {
        }

        public void GodotCall(T1 v1, T2 v2) {
            try {
                Action(v1, v2);
            } catch (Exception e) {
                throw e;
            } finally {
                AfterCall();
            }
        }
    }

    internal class SignalObjectTargetAction<T1, T2, T3> : SignalObjectTarget<Action<T1, T2, T3>> {
        public SignalObjectTargetAction(Object origin, Action<T1, T2, T3> action, bool oneShot) : base(origin, action, oneShot) {
        }

        public void GodotCall(T1 v1, T2 v2, T3 v3) {
            try {
                Action(v1, v2, v3);
            } catch (Exception e) {
                throw e;
            } finally {
                AfterCall();
            }
        }
    }

    internal class SignalObjectTargetAction<T1, T2, T3, T4> : SignalObjectTarget<Action<T1, T2, T3, T4>> {
        public SignalObjectTargetAction(Object origin, Action<T1, T2, T3, T4> action, bool oneShot) : base(origin, action, oneShot) {
        }

        public void GodotCall(T1 v1, T2 v2, T3 v3, T4 v4) {
            try {
                Action(v1, v2, v3, v4);
            } catch (Exception e) {
                throw e;
            } finally {
                AfterCall();
            }
        }
    }

    internal class SignalObjectTargetAction<T1, T2, T3, T4, T5> : SignalObjectTarget<Action<T1, T2, T3, T4, T5>> {
        public SignalObjectTargetAction(Object origin, Action<T1, T2, T3, T4, T5> action, bool oneShot) : base(origin, action, oneShot) {
        }

        public void GodotCall(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5) {
            try {
                Action(v1, v2, v3, v4, v5);
            } catch (Exception e) {
                throw e;
            } finally {
                AfterCall();
            }
        }
    }

    internal class SignalObjectTargetAction<T1, T2, T3, T4, T5, T6> : SignalObjectTarget<Action<T1, T2, T3, T4, T5, T6>> {
        public SignalObjectTargetAction(Object origin, Action<T1, T2, T3, T4, T5, T6> action, bool oneShot) : base(origin, action, oneShot) {
        }

        public void GodotCall(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6) {
            try {
                Action(v1, v2, v3, v4, v5, v6);
            } catch (Exception e) {
                throw e;
            } finally {
                AfterCall();
            }
        }
    }

    internal class SignalObjectTargetAction<T1, T2, T3, T4, T5, T6, T7> : SignalObjectTarget<Action<T1, T2, T3, T4, T5, T6, T7>> {
        public SignalObjectTargetAction(Object origin, Action<T1, T2, T3, T4, T5, T6, T7> action, bool oneShot) : base(origin, action, oneShot) {
        }

        public void GodotCall(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7) {
            try {
                Action(v1, v2, v3, v4, v5, v6, v7);
            } catch (Exception e) {
                throw e;
            } finally {
                AfterCall();
            }
        }
    }

    internal class SignalObjectTargetAction<T1, T2, T3, T4, T5, T6, T7, T8> : SignalObjectTarget<Action<T1, T2, T3, T4, T5, T6, T7, T8>> {
        public SignalObjectTargetAction(Object origin, Action<T1, T2, T3, T4, T5, T6, T7, T8> action, bool oneShot) : base(origin, action, oneShot) {
        }

        public void GodotCall(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8) {
            try {
                Action(v1, v2, v3, v4, v5, v6, v7, v8);
            } catch (Exception e) {
                throw e;
            } finally {
                AfterCall();
            }
        }
    }

}