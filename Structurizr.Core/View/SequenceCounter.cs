namespace Structurizr
{

   internal class SequenceCounter
    {

        internal readonly SequenceCounter Parent;
        internal int Sequence { get; set; }

        internal SequenceCounter()
        {
        }

        internal SequenceCounter(SequenceCounter parent)
        {
            Parent = parent;
        }

        internal virtual void Increment()
        {
            Sequence++;
        }

        public virtual string AsString()
        {
            if (Parent == null)
            {
                return "" + Sequence;
            }
            else
            {
                return Parent.AsString() + "." + Sequence;
            }
        }

        internal SequenceCounter Clone()
        {
            SequenceCounter counter;
            if (Parent == null)
            {
                counter = new SequenceCounter();
            }
            else
            {
                counter = new SequenceCounter(Parent.Clone());
            }

            counter.Sequence = Sequence;

            return counter;
        }

    }

}
