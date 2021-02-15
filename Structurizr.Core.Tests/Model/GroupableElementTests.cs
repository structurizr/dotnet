using Xunit;

namespace Structurizr.Core.Tests
{
    
    public class GroupableElementTests : AbstractTestBase
    {

        [Fact]
        public void Test_getGroup_ReturnsNullByDefault()
        {
            Person element = Model.AddPerson("Person");
            Assert.Null(element.Group);
        }

        [Fact]
        public void Test_setGroup()
        {
            Person element = Model.AddPerson("Person");
            element.Group = "Group";
            Assert.Equal("Group", element.Group);
        }

        [Fact]
        public void Test_setGroup_TrimsWhiteSpace()
        {
            Person element = Model.AddPerson("Person");
            element.Group = " Group ";
            Assert.Equal("Group", element.Group);
        }

        [Fact]
        public void Test_setGroup_HandlesEmptyAndNullValues()
        {
            Person element = Model.AddPerson("Person");
            element.Group = "Group";

            element.Group = null;
            Assert.Null(element.Group);

            element.Group = "Group";
            element.Group = "";
            Assert.Null(element.Group);

            element.Group = "Group";
            element.Group = " ";
            Assert.Null(element.Group);
        }
        
    }
}