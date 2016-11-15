using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Structurizr.CoreTests
{

    [TestClass]
    public class ElementTests : AbstractTestBase
    {

        [TestMethod]
        public void Test_HasEfferentRelationshipWith_ReturnsFalse_WhenANullElementIsSpecified()
        {
            SoftwareSystem softwareSystem1 = model.AddSoftwareSystem("System 1", "");
            Assert.IsFalse(softwareSystem1.HasEfferentRelationshipWith(null));
        }

        [TestMethod]
        public void Test_HasEfferentRelationshipWith_ReturnsFalse_WhenTheSameElementIsSpecifiedAndNoCyclicRelationshipExists()
        {
            SoftwareSystem softwareSystem1 = model.AddSoftwareSystem("System 1", "");
            Assert.IsFalse(softwareSystem1.HasEfferentRelationshipWith(softwareSystem1));
        }

        [TestMethod]
        public void Test_HasEfferentRelationshipWith_ReturnsTrue_WhenTheSameElementIsSpecifiedAndACyclicRelationshipExists()
        {
            SoftwareSystem softwareSystem1 = model.AddSoftwareSystem("System 1", "");
            softwareSystem1.Uses(softwareSystem1, "uses");
            Assert.IsTrue(softwareSystem1.HasEfferentRelationshipWith(softwareSystem1));
        }

        [TestMethod]
        public void Test_HasEfferentRelationshipWith_ReturnsTrue_WhenThereIsARelationship()
        {
            SoftwareSystem softwareSystem1 = model.AddSoftwareSystem("System 1", "");
            SoftwareSystem softwareSystem2 = model.AddSoftwareSystem("System 2", "");
            softwareSystem1.Uses(softwareSystem2, "uses");
            Assert.IsTrue(softwareSystem1.HasEfferentRelationshipWith(softwareSystem2));
        }

        [TestMethod]
        public void Test_GetEfferentRelationshipWith_ReturnsNull_WhenANullElementIsSpecified()
        {
            SoftwareSystem softwareSystem1 = model.AddSoftwareSystem("System 1", "");
            Assert.IsNull(softwareSystem1.GetEfferentRelationshipWith(null));
        }

        [TestMethod]
        public void Test_GetEfferentRelationshipWith_ReturnsNull_WhenTheSameElementIsSpecifiedAndNoCyclicRelationshipExists()
        {
            SoftwareSystem softwareSystem1 = model.AddSoftwareSystem("System 1", "");
            Assert.IsNull(softwareSystem1.GetEfferentRelationshipWith(softwareSystem1));
        }

        [TestMethod]
        public void Test_GetEfferentRelationshipWith_ReturnsCyclicRelationship_WhenTheSameElementIsSpecifiedAndACyclicRelationshipExists()
        {
            SoftwareSystem softwareSystem1 = model.AddSoftwareSystem("System 1", "");
            softwareSystem1.Uses(softwareSystem1, "uses");

            Relationship relationship = softwareSystem1.GetEfferentRelationshipWith(softwareSystem1);
            Assert.AreSame(softwareSystem1, relationship.Source);
            Assert.AreEqual("uses", relationship.Description);
            Assert.AreSame(softwareSystem1, relationship.Destination);
        }

        [TestMethod]
        public void Test_GetEfferentRelationshipWith_ReturnsTheRelationship_WhenThereIsARelationship()
        {
            SoftwareSystem softwareSystem1 = model.AddSoftwareSystem("System 1", "");
            SoftwareSystem softwareSystem2 = model.AddSoftwareSystem("System 2", "");
            softwareSystem1.Uses(softwareSystem2, "uses");

            Relationship relationship = softwareSystem1.GetEfferentRelationshipWith(softwareSystem2);
            Assert.AreSame(softwareSystem1, relationship.Source);
            Assert.AreEqual("uses", relationship.Description);
            Assert.AreSame(softwareSystem2, relationship.Destination);
        }

        [TestMethod]
        public void Test_HasAfferentRelationships_ReturnsFalse_WhenThereAreNoIncomingRelationships()
        {
            SoftwareSystem softwareSystem1 = model.AddSoftwareSystem("System 1", "");
            SoftwareSystem softwareSystem2 = model.AddSoftwareSystem("System 2", "");
            softwareSystem1.Uses(softwareSystem2, "Uses");

            Assert.IsFalse(softwareSystem1.HasAfferentRelationships());
        }

        [TestMethod]
        public void Test_HasAfferentRelationships_ReturnsTrue_WhenThereAreIncomingRelationships()
        {
            SoftwareSystem softwareSystem1 = model.AddSoftwareSystem("System 1", "");
            SoftwareSystem softwareSystem2 = model.AddSoftwareSystem("System 2", "");
            softwareSystem1.Uses(softwareSystem2, "Uses");

            Assert.IsTrue(softwareSystem2.HasAfferentRelationships());
        }

        [TestMethod]
        public void Test_SetUrl_DoesNotThrowAnException_WhenANullUrlIsSpecified()
        {
            SoftwareSystem element = model.AddSoftwareSystem("Name", "Description");
            element.Url = null;
        }

        [TestMethod]
        public void Test_SetUrl_DoesNotThrowAnException_WhenAnEmptyUrlIsSpecified()
        {
            SoftwareSystem element = model.AddSoftwareSystem("Name", "Description");
            element.Url = "";
        }

        [TestMethod]
        public void Test_SetUrl_ThrowsAnException_WhenAnInvalidUrlIsSpecified()
        {
            try
            {
                SoftwareSystem element = model.AddSoftwareSystem("Name", "Description");
                element.Url = "www.somedomain.com";
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("www.somedomain.com is not a valid URL.", e.Message);
            }
        }

        [TestMethod]
        public void Test_SetUrl_DoesNotThrowAnException_WhenAValidUrlIsSpecified()
        {
            SoftwareSystem element = model.AddSoftwareSystem("Name", "Description");
            element.Url = "http://www.somedomain.com";
            Assert.AreEqual("http://www.somedomain.com", element.Url);
        }

    }
}
