namespace Structurizr
{

    internal class ParallelSequenceCounter : SequenceCounter
    {

        internal readonly SequenceCounter Root;

        internal ParallelSequenceCounter(SequenceCounter parent) : base(parent)
        {
            Root = parent.Clone();
            Sequence = Parent.Sequence;
        }

        internal override void Increment()
        {
            Parent.Increment();
        }

        public override string AsString()
        {
            return Parent.AsString();
        }

    }

}
