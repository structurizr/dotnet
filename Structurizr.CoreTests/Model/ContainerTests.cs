using Microsoft.VisualStudio.TestTools.UnitTesting;
using Structurizr.CoreTests.MyApp;

namespace Structurizr.CoreTests
{
    [TestClass]
    public class ContainerTests
    {

        private Workspace workspace;
        private Model model;
        private SoftwareSystem softwareSystem;
        private Container container;

        public ContainerTests()
        {
            workspace = new Workspace("Name", "Description");
            model = workspace.Model;
            softwareSystem = model.AddSoftwareSystem("System", "Description");
            container = softwareSystem.AddContainer("Container", "Description", "Some technology");
        }

        [TestMethod]
        public void Test_CanonicalName()
        {
            Assert.AreEqual("/System/Container", container.CanonicalName);
        }

        [TestMethod]
        public void Test_CanonicalName_WhenNameContainsASlashCharacter()
        {
            container.Name = "Name1/Name2";
            Assert.AreEqual("/System/Name1Name2", container.CanonicalName);
        }

        [TestMethod]
        public void Test_Parent_ReturnsTheParentSoftwareSystem()
        {
            Assert.AreEqual(softwareSystem, container.Parent);
        }

        [TestMethod]
        public void Test_SoftwareSystem_ReturnsTheParentSoftwareSystem()
        {
            Assert.AreEqual(softwareSystem, container.SoftwareSystem);
        }

        [TestMethod]
        public void Test_RemoveTags_DoesNotRemoveRequiredTags()
        {
            Assert.IsTrue(container.Tags.Contains(Tags.Element));
            Assert.IsTrue(container.Tags.Contains(Tags.Container));

            container.RemoveTag(Tags.Container);
            container.RemoveTag(Tags.Element);

            Assert.IsTrue(container.Tags.Contains(Tags.Element));
            Assert.IsTrue(container.Tags.Contains(Tags.Container));
        }

        [TestMethod]
        public void Test_Adding_A_Component_Adds_It_To_The_List_Of_Components()
        {
            MyComponent myComponent = new MyComponent();
            container.Add(myComponent);

            Assert.IsTrue(container.Components.Contains(myComponent));
        }

        [TestMethod]
        public void Test_Adding_A_Component__Does_Not_Add_It_To_The_List_Of_Components_If_A_Component_With_The_Same_Name_Exists()
        {
            MyComponent myComponent = new MyComponent();
            MyComponent myComponent2 = new MyComponent();

            container.Add(myComponent);
            container.Add(myComponent2);

            Assert.IsFalse(container.Components.Contains(myComponent2));
        }

    }
}