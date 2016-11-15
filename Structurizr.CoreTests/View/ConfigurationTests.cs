using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Structurizr.CoreTests
{

    [TestClass]
    public class ConfigurationTests : AbstractTestBase
    {

        [TestMethod]
        public void test_defaultView_DoesNothing_WhenPassedNull()
        {
            Configuration configuration = new Configuration();
            configuration.SetDefaultView(null);
            Assert.IsNull(configuration.DefaultView);
        }

        [TestMethod]
        public void test_defaultView()
        {
            EnterpriseContextView view = views.CreateEnterpriseContextView("key", "Description");
            Configuration configuration = new Configuration();
            configuration.SetDefaultView(view);
            Assert.AreEqual("key", configuration.DefaultView);
        }

        [TestMethod]
        public void test_copyConfigurationFrom()
        {
            Configuration source = new Configuration();
            source.LastSavedView = "someKey";

            Configuration destination = new Configuration();
            destination.CopyConfigurationFrom(source);
            Assert.AreEqual("someKey", destination.LastSavedView);
        }

    }

}
