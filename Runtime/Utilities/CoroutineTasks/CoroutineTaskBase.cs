namespace TWILite.Utilities.CoroutineTasks
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class CoroutineTaskBase<T> where T : CoroutineTaskBase<T>
    {
        private Exception exception;
        private Delegate startCoroutine;
        private CoroutineTaskStates state;

        public Exception Exception => exception;
        public bool IsRunning => state == CoroutineTaskStates.Running || state == CoroutineTaskStates.Cancelling;
        public event Action<T> OnStateChanged;

        public CoroutineTaskStates State => state;

        public CoroutineTaskBase(Action<IEnumerable> coroutine) => Initialize(coroutine);
        public CoroutineTaskBase(Action<IEnumerable> coroutine, Action<T> onStateChanged) => Initialize(coroutine, onStateChanged);

        public CoroutineTaskBase(Func<IEnumerator, object> coroutine) => Initialize(coroutine);
        public CoroutineTaskBase(Func<IEnumerator, object> coroutine, Action<T> onStateChanged) => Initialize(coroutine, onStateChanged);

        private void Initialize(Delegate coroutine)
        {
            switch (coroutine)
            {
                case Action<IEnumerator> action: this.startCoroutine = action; break;
                case Func<IEnumerator, object> func: this.startCoroutine = func; break;
                case null: throw new ArgumentNullException("The argument 'startCoroutine' cannot be null");
                default: throw new ArgumentException("The argument 'startCoroutine' type is not supported");
            }
        }
        private void Initialize(Delegate coroutine, Action<T> onStateChanged)
        {
            if (onStateChanged == null) throw new ArgumentNullException();
            else OnStateChanged += onStateChanged;
            Initialize(coroutine);
        }

        protected virtual void OnReset() { }
        protected virtual void OnStart() { }
        protected virtual void OnStop() { }

        public void Cancel()
        {
            if (state != CoroutineTaskStates.Running) return;
            else state = CoroutineTaskStates.Cancelling;
            OnStateChanged?.Invoke(this as T);
        }

        public bool Update() => IsRunning ? CanInvokeUpdate() : throw new InvalidOperationException();

        public bool Update(Action action) => InvokeUpdate(action);
        public bool Update<T0>(Action<T0> action, T0 arg0) => InvokeUpdate(action, arg0);
        public bool Update<T0, T1>(Action<T0, T1> action, T0 arg0, T1 arg1) => InvokeUpdate(action, arg0, arg1);
        public bool Update<T0, T1, T2>(Action<T0, T1, T2> action, T0 arg0, T1 arg1, T2 arg2) => InvokeUpdate(action, arg0, arg1, arg2);
        public bool Update<T0, T1, T2, T3>(Action<T0, T1, T2, T3> action, T0 arg0, T1 arg1, T2 arg2, T3 arg3) => InvokeUpdate(action, arg0, arg1, arg2, arg3);
        public bool Update<T0, T1, T2, T3, T4>(Action<T0, T1, T2, T3, T4> action, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => InvokeUpdate(action, arg0, arg1, arg2, arg3, arg4);
        public bool Update<T0, T1, T2, T3, T4, T5>(Action<T0, T1, T2, T3, T4, T5> action, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => InvokeUpdate(action, arg0, arg1, arg2, arg3, arg4, arg5);
        public bool Update<T0, T1, T2, T3, T4, T5, T6>(Action<T0, T1, T2, T3, T4, T5, T6> action, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) => InvokeUpdate(action, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        public bool Update<T0, T1, T2, T3, T4, T5, T6, T7>(Action<T0, T1, T2, T3, T4, T5, T6, T7> action, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) => InvokeUpdate(action, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);

        protected virtual bool CanInvokeUpdate() => true;

        public void Start(IEnumerator task)
        {
            switch (state)
            {
                case CoroutineTaskStates.Running: throw new InvalidOperationException();
                case CoroutineTaskStates.Cancelling: throw new InvalidOperationException();
                default:
                    OnReset();
                    exception = null;
                    state = CoroutineTaskStates.Running;
                    OnStateChanged?.Invoke(this as T);
                    OnStart();

                    switch (startCoroutine)
                    {
                        case Action<IEnumerator> action: action.Invoke(StartCoroutine(task)); break;
                        case Func<IEnumerator, object> func: func.Invoke(StartCoroutine(task)); break;
                        case null: throw new NullReferenceException("The 'startCoroutine' cannot be null");
                        default: throw new InvalidCastException("The 'startCoroutine' type is not supported");
                    }
                    break;
            }
        }
        private IEnumerator StartCoroutine(IEnumerator task)
        {
            object obj;
            while (state == CoroutineTaskStates.Running)
            {
                try
                {
                    if (task.MoveNext()) obj = task.Current;
                    else break;
                }
                catch (Exception e)
                {
                    state = CoroutineTaskStates.Error;
                    exception = e;
                    break;
                }

                yield return obj;
            }
            if (task is IEnumerator<object> disposable) disposable.Dispose();

            switch (state)
            {
                case CoroutineTaskStates.Error: break;
                case CoroutineTaskStates.Running: state = CoroutineTaskStates.Completed; break;
                case CoroutineTaskStates.Cancelling: state = CoroutineTaskStates.Cancelled; break;
                default: throw new InvalidOperationException();
            }

            OnStop();
            OnStateChanged?.Invoke(this as T);
        }

        private bool InvokeUpdate(Delegate @delegate, params object[] args)
        {
            if (@delegate == null) throw new ArgumentNullException();
            else if (!IsRunning) throw new InvalidOperationException();
            else if (CanInvokeUpdate())
                if (args == null || args.Length == 0) @delegate.DynamicInvoke();
                else @delegate.DynamicInvoke(args);
            else return false;
            return true;
        }
    }
}