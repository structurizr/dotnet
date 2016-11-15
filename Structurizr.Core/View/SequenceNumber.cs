namespace Structurizr
{
    internal class SequenceNumber
    {

        private SequenceCounter _counter = new SequenceCounter();

        internal SequenceNumber()
        {
        }

        internal string GetNext()
        {
            _counter.Increment();
            return _counter.AsString();
        }

        internal void StartChildSequence()
        {
            _counter = new SequenceCounter(_counter);
        }

        internal void EndChildSequence()
        {
            _counter = _counter.Parent;
        }

        internal void StartParallelSequence()
        {
            _counter = new ParallelSequenceCounter(_counter);
        }

        internal void EndParallelSequence()
        {
            _counter = ((ParallelSequenceCounter)_counter).Root;
        }

    }
}
