using Microsoft.VisualStudio.TestTools.UnitTesting;
using Structurizr.CoreTests.MyApp;

namespace Structurizr.CoreTests
{
    [TestClass]
    public class ComponentTests
    {

        private Workspace workspace;
        private Model model;
        private SoftwareSystem softwareSystem;
        private Container container;

        public ComponentTests()
        {
            workspace = new Workspace("Name", "Description");
            model = workspace.Model;
            softwareSystem = model.AddSoftwareSystem("System", "Description");
            container = softwareSystem.AddContainer("Container", "Description", "Some technology");
        }

        [TestMethod]
        public void Test_Name_ReturnsTheGivenName_WhenANameIsGiven()
        {
            Component component = new Component();
            component.Name = "Some name";
            Assert.AreEqual("Some name", component.Name);
        }

        [TestMethod]
        public void Test_CanonicalName()
        {
            Component component = container.AddComponent("Component", "Description");
            Assert.AreEqual("/System/Container/Component", component.CanonicalName);
        }

        [TestMethod]
        public void Test_CanonicalName_WhenNameContainsASlashCharacter()
        {
            Component component = container.AddComponent("Name1/Name2", "Description");
            Assert.AreEqual("/System/Container/Name1Name2", component.CanonicalName);
        }

        [TestMethod]
        public void Test_Parent_ReturnsTheParentContainer()
        {
            Component component = container.AddComponent("Component", "Description");
            Assert.AreEqual(container, component.Parent);
        }

        [TestMethod]
        public void Test_Container_ReturnsTheParentContainer()
        {
            Component component = container.AddComponent("Name", "Description");
            Assert.AreEqual(container, component.Container);
        }

        [TestMethod]
        public void Test_RemoveTags_DoesNotRemoveRequiredTags()
        {
            Component component = new Component();
            Assert.IsTrue(component.Tags.Contains(Tags.Element));
            Assert.IsTrue(component.Tags.Contains(Tags.Component));

            component.RemoveTag(Tags.Component);
            component.RemoveTag(Tags.Element);

            Assert.IsTrue(component.Tags.Contains(Tags.Element));
            Assert.IsTrue(component.Tags.Contains(Tags.Component));
        }

        [TestMethod]
        public void Test_Adding_A_CodeElement_Adds_It_To_The_List_Of_Elements()
        {
            Component component = new Component();
            MyClass myClass = new MyClass();
            component.AddSupportingType(myClass);

            Assert.IsTrue(component.Code.Contains(myClass));
        }
    }
}