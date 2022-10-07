using System;
using Object = Godot.Object;

namespace Betauer.Bus {
    public abstract class BaseEventConsumer<TThis, TPublisher, TArgs> where TThis : class {
        protected Action<TPublisher, TArgs>? Action { get; private set; }
        public Object? Watch { get; private set; }
        public Func<bool>? RemoveIfFunc { get; private set; }

        public TThis Do(Action<TPublisher, TArgs> action) {
            Action = action;
            return this as TThis;
        }

        public TThis RemoveIfInvalid(Object? watch) {
            Watch = watch;
            return this as TThis;
        }

        public TThis RemoveIf(Func<bool> func) {
            RemoveIfFunc = func;
            return this as TThis;
        }

        public void Execute(TPublisher publisher, TArgs args) {
            Action?.Invoke(publisher, args);
        }

        public virtual void Remove() {
            Action = null;
            RemoveIfFunc = null;
            Watch = null;
        }

        public virtual bool IsValid() {
            return Action != null &&
                   (RemoveIfFunc == null || !RemoveIfFunc.Invoke()) &&
                   (Watch == null || Object.IsInstanceValid(Watch));
        }
    }
}