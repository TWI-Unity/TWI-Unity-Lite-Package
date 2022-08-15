namespace TWILite.Utilities.CoroutineTasks
{
    using System;
    using System.Collections;
    using System.Diagnostics;

    public sealed class CoroutineTaskInterval : CoroutineTaskBase<CoroutineTaskInterval>
    {
        private const long UPDATE_RATE = 40;
        private Stopwatch stopwatch = new Stopwatch();

        private long updateRate = UPDATE_RATE;
        public TimeSpan Elapsed => stopwatch.Elapsed;

        public CoroutineTaskInterval(Action<IEnumerable> startCoroutine, long updateRate = UPDATE_RATE) : base(startCoroutine) => this.updateRate = updateRate > 0 ? updateRate : UPDATE_RATE;
        public CoroutineTaskInterval(Action<IEnumerable> startCoroutine, Action<CoroutineTaskInterval> onStateChanged, long updateRate = UPDATE_RATE) : base(startCoroutine, onStateChanged) => this.updateRate = updateRate > 0 ? updateRate : UPDATE_RATE;

        public CoroutineTaskInterval(Func<IEnumerator, object> startCoroutine, long updateRate = UPDATE_RATE) : base(startCoroutine) => this.updateRate = updateRate > 0 ? updateRate : UPDATE_RATE;
        public CoroutineTaskInterval(Func<IEnumerator, object> startCoroutine, Action<CoroutineTaskInterval> onStateChanged, long updateRate = UPDATE_RATE) : base(startCoroutine, onStateChanged) => this.updateRate = updateRate > 0 ? updateRate : UPDATE_RATE;

        protected override void OnReset() => stopwatch.Reset();
        protected override void OnStart() => stopwatch.Restart();
        protected override void OnStop() => stopwatch.Stop();

        protected override bool CanInvokeUpdate()
        {
            if (stopwatch.ElapsedMilliseconds < updateRate) return false;
            else stopwatch.Restart();
            return true;
        }
    }
}
