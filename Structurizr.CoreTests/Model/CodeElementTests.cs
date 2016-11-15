using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Structurizr.CoreTests
{

    [TestClass]
    public class CodeElementTests
    {

        private readonly CodeElement _codeElement = new CodeElement("Wibble.Wobble, Foo.Bar, Version=1.0.0.0, Culture=neutral, PublicKeyToken=xyz");

        [TestMethod]
        public void Test_Construction_WhenAFullyQualifiedTypeIsSpecified()
        {
            Assert.AreEqual("Wobble", _codeElement.Name);
        }

        [TestMethod]
        public void Test_SetUrl_DoesNotThrowAnException_WhenANullUrlIsSpecified()
        {
            _codeElement.Url = null;
        }

        [TestMethod]
        public void Test_SetUrl_DoesNotThrowAnException_WhenAnEmptyUrlIsSpecified()
        {
            _codeElement.Url = "";
        }

        [TestMethod]
        public void Test_SetUrl_ThrowsAnException_WhenAnInvalidUrlIsSpecified()
        {
            try
            {
                _codeElement.Url = "www.somedomain.com";
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
            _codeElement.Url = "http://www.somedomain.com";
            Assert.AreEqual("http://www.somedomain.com", _codeElement.Url);
        }

    }

}
