namespace TWILite.Utilities.CoroutineTasks
{
    public enum CoroutineTaskStates
    {
        Idle = 0,
        Running = 1,
        Completed = 2,
        Cancelling = 3,
        Cancelled = 4,
        Error = 5,
    }
}
