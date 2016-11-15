using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Structurizr.CoreTests
{
    [TestClass]
    public class SequenceNumberTests
    {

        [TestMethod]
        public void Test_Increment()
        {
            SequenceNumber sequenceNumber = new SequenceNumber();
            Assert.AreEqual("1", sequenceNumber.GetNext());
            Assert.AreEqual("2", sequenceNumber.GetNext());
        }

        [TestMethod]
        public void Test_ChildSequence()
        {
            SequenceNumber sequenceNumber = new SequenceNumber();
            Assert.AreEqual("1", sequenceNumber.GetNext());

            sequenceNumber.StartChildSequence();
            Assert.AreEqual("1.1", sequenceNumber.GetNext());
            Assert.AreEqual("1.2", sequenceNumber.GetNext());

            sequenceNumber.EndChildSequence();
            Assert.AreEqual("2", sequenceNumber.GetNext());
        }

        [TestMethod]
        public void Test_ParallelSequences()
        {
            SequenceNumber sequenceNumber = new SequenceNumber();
            Assert.AreEqual("1", sequenceNumber.GetNext());

            sequenceNumber.StartParallelSequence();
            Assert.AreEqual("2", sequenceNumber.GetNext());
            sequenceNumber.EndParallelSequence();

            sequenceNumber.StartParallelSequence();
            Assert.AreEqual("2", sequenceNumber.GetNext());
            sequenceNumber.EndParallelSequence();

            Assert.AreEqual("2", sequenceNumber.GetNext());
        }

    }
}
