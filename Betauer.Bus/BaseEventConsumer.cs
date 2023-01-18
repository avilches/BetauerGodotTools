using System;

namespace Betauer.Bus {
    public abstract class BaseEventConsumer<TThis, TPublisher, TArgs> where TThis : class {
        protected Action<TPublisher, TArgs>? Action { get; set; }
        protected Func<bool>? UnsubscribeIfFunc { get; set; }
        
        public TThis Do(Action<TPublisher, TArgs> action) {
            Action = action;
            return this as TThis;
        }

        public TThis UnsubscribeIf(Func<bool> func) {
            UnsubscribeIfFunc = func;
            return this as TThis;
        }

        public void Execute(TPublisher publisher, TArgs args) {
            Action?.Invoke(publisher, args);
        }

        public virtual void Unsubscribe() {
            Action = null;
            UnsubscribeIfFunc = null;
        }

        public virtual bool IsValid() {
            return Action != null && (UnsubscribeIfFunc == null || !UnsubscribeIfFunc.Invoke());
        }
    }
}