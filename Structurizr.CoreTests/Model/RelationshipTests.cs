using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Structurizr.CoreTests
{
    [TestClass]
    public class RelationshipTests : AbstractTestBase
    {

        private SoftwareSystem _softwareSystem1, _softwareSystem2;

        public RelationshipTests()
        {
            _softwareSystem1 = model.AddSoftwareSystem("1", "Description");
            _softwareSystem2 = model.AddSoftwareSystem("2", "Description");
        }

        [TestMethod]
        public void Test_Description_NeverReturnsNull()
        {
            Relationship relationship = new Relationship();
            relationship.Description = null;
            Assert.AreEqual("", relationship.Description);
        }

        [TestMethod]
        public void Test_Tags_WhenThereAreNoTags()
        {
            Relationship relationship = _softwareSystem1.Uses(_softwareSystem2, "uses");
            Assert.AreEqual("Relationship,Synchronous", relationship.Tags);
        }

        [TestMethod]
        public void Test_Tags_ReturnsTheListOfTags_WhenThereAreSomeTags()
        {
            Relationship relationship = _softwareSystem1.Uses(_softwareSystem2, "uses");
            relationship.AddTags("tag1", "tag2", "tag3");
            Assert.AreEqual("Relationship,Synchronous,tag1,tag2,tag3", relationship.Tags);
        }

        [TestMethod]
        public void Test_Tags_DoesNotDoAnything_WhenPassedNull()
        {
            Relationship relationship = _softwareSystem1.Uses(_softwareSystem2, "uses");
            relationship.Tags = null;
            Assert.AreEqual("Relationship,Synchronous", relationship.Tags);
        }

        [TestMethod]
        public void Test_AddTags_DoesNotDoAnything_WhenPassedNull()
        {
            Relationship relationship = _softwareSystem1.Uses(_softwareSystem2, "uses");
            relationship.AddTags((string)null);
            Assert.AreEqual("Relationship,Synchronous", relationship.Tags);

            relationship.AddTags(null, null, null);
            Assert.AreEqual("Relationship,Synchronous", relationship.Tags);
        }

        [TestMethod]
        public void Test_AddTags_AddsTags_WhenPassedSomeTags()
        {
            Relationship relationship = _softwareSystem1.Uses(_softwareSystem2, "uses");
            relationship.AddTags(null, "tag1", null, "tag2");
            Assert.AreEqual("Relationship,Synchronous,tag1,tag2", relationship.Tags);
        }

        [TestMethod]
        public void Test_InteractionStyle_ReturnsSynchronous_WhenNotExplicitlySet()
        {
            Relationship relationship = _softwareSystem1.Uses(_softwareSystem2, "uses");
            Assert.AreEqual(InteractionStyle.Synchronous, relationship.InteractionStyle);
        }

        [TestMethod]
        public void test_Tags_IncludesTheInteractionStyleWhenSpecified()
        {
            Relationship relationship = _softwareSystem1.Uses(_softwareSystem2, "uses");
            Assert.IsTrue(relationship.Tags.Contains(Tags.Synchronous));
            Assert.IsFalse(relationship.Tags.Contains(Tags.Asynchronous));

            relationship.InteractionStyle = InteractionStyle.Asynchronous;
            Assert.IsFalse(relationship.Tags.Contains(Tags.Synchronous));
            Assert.IsTrue(relationship.Tags.Contains(Tags.Asynchronous));
        }

    }
}
