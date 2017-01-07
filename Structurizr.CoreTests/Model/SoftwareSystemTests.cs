using Microsoft.VisualStudio.TestTools.UnitTesting;
using Structurizr.CoreTests.MyApp;

namespace Structurizr.CoreTests
{
    [TestClass]
    public class SoftwareSystemTests
    {

        private Workspace workspace;
        private Model model;
        private SoftwareSystem softwareSystem;

        public SoftwareSystemTests()
        {
            workspace = new Workspace("Name", "Description");
            model = workspace.Model;
            softwareSystem = model.AddSoftwareSystem("System", "Description");
        }

        [TestMethod]
        public void Test_Adding_A_Container_Adds_It_To_The_List_Of_Containers()
        {
            MyContainer myContainer = new MyContainer();
            softwareSystem.AddContainer(myContainer);

            Assert.IsTrue(softwareSystem.Containers.Contains(myContainer));
        }

        [TestMethod]
        public void Test_Adding_A_Container__Does_Not_Add_It_To_The_List_Of_Containers_If_A_Container_With_The_Same_Name_Exists()
        {
            MyContainer myContainer = new MyContainer();
            MyContainer myContainer2 = new MyContainer();
            softwareSystem.AddContainer(myContainer);
            softwareSystem.AddContainer(myContainer2);

            Assert.IsFalse(softwareSystem.Containers.Contains(myContainer2));
        }

    }
}