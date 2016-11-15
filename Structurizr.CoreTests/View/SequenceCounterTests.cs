using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Structurizr.CoreTests
{
    [TestClass]
    public class SequenceCounterTests
    {

        [TestMethod]
        public void Test_increment_IncrementsTheCounter_WhenThereIsNoParent()
        {
            SequenceCounter counter = new SequenceCounter();
            Assert.AreEqual("0", counter.AsString());

            counter.Increment();
            Assert.AreEqual("1", counter.AsString());

            counter.Increment();
            Assert.AreEqual("2", counter.AsString());
        }

        [TestMethod]
        public void Test_counter_WhenThereIsOneParent()
        {
            SequenceCounter parent = new SequenceCounter();
            parent.Increment();
            Assert.AreEqual("1", parent.AsString());

            SequenceCounter child = new SequenceCounter(parent);
            child.Increment();
            Assert.AreEqual("1.1", child.AsString());

            child.Increment();
            Assert.AreEqual("1.2", child.AsString());
        }

        [TestMethod]
        public void Test_counter_WhenThereAreTwoParents()
        {
            SequenceCounter parent = new SequenceCounter();
            parent.Increment();
            Assert.AreEqual("1", parent.AsString());

            SequenceCounter child = new SequenceCounter(parent);
            child.Increment();
            Assert.AreEqual("1.1", child.AsString());

            SequenceCounter grandchild = new SequenceCounter(child);
            grandchild.Increment();
            Assert.AreEqual("1.1.1", grandchild.AsString());

            grandchild.Increment();
            Assert.AreEqual("1.1.2", grandchild.AsString());
        }

    }
}
