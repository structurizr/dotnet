using System;
using Xunit;

namespace Structurizr.Core.Tests
{
    
    public class CreateImpliedRelationshipsUnlessSameRelationshipExistsStrategyTests : AbstractTestBase
    {
    
        [Fact]
        public void test_impliedRelationships_WhenNoSummaryRelationshipsExist()
        {
            SoftwareSystem a = Model.AddSoftwareSystem("A", "");
            Container aa = a.AddContainer("AA", "", "");
            Component aaa = aa.AddComponent("AAA", "", "");

            SoftwareSystem b = Model.AddSoftwareSystem("B", "");
            Container bb = b.AddContainer("BB", "", "");
            Component bbb = bb.AddComponent("BBB", "", "");

            Model.ImpliedRelationshipsStrategy = new CreateImpliedRelationshipsUnlessSameRelationshipExistsStrategy();

            aaa.Uses(bbb, "Uses 1", null, InteractionStyle.Asynchronous, new String[] { "Tag 1", "Tag 2" });

            Assert.Equal(9, Model.Relationships.Count);
            Assert.True(aaa.HasEfferentRelationshipWith(bbb, "Uses 1"));

            // and all relationships should have the same interaction style and tags
            foreach (Relationship r in Model.Relationships)
            {
                Assert.Equal(InteractionStyle.Asynchronous, r.InteractionStyle);
                Assert.True(r.GetTagsAsSet().Contains("Tag 1"));
                Assert.True(r.GetTagsAsSet().Contains("Tag 2"));
            }

            // AAA->BBB implies AAA->BB AAA->B AA->BBB AA->BB AA->B A->BBB A->BB A->B
            Assert.True(aaa.HasEfferentRelationshipWith(bb, "Uses 1"));
            Assert.True(aaa.HasEfferentRelationshipWith(b, "Uses 1"));

            Assert.True(aa.HasEfferentRelationshipWith(bbb, "Uses 1"));
            Assert.True(aa.HasEfferentRelationshipWith(bb, "Uses 1"));
            Assert.True(aa.HasEfferentRelationshipWith(b, "Uses 1"));

            Assert.True(a.HasEfferentRelationshipWith(bbb, "Uses 1"));
            Assert.True(a.HasEfferentRelationshipWith(bb, "Uses 1"));
            Assert.True(a.HasEfferentRelationshipWith(b, "Uses 1"));

            // and add another relationship with a different description
            aaa.Uses(bbb, "Uses 2");

            Assert.Equal(18, Model.Relationships.Count);
            Assert.True(aaa.HasEfferentRelationshipWith(bbb, "Uses 2"));

            // AAA->BBB implies AAA->BB AAA->B AA->BBB AA->BB AA->B A->BBB A->BB A->B
            Assert.True(aaa.HasEfferentRelationshipWith(bb, "Uses 2"));
            Assert.True(aaa.HasEfferentRelationshipWith(b, "Uses 2"));

            Assert.True(aa.HasEfferentRelationshipWith(bbb, "Uses 2"));
            Assert.True(aa.HasEfferentRelationshipWith(bb, "Uses 2"));
            Assert.True(aa.HasEfferentRelationshipWith(b, "Uses 2"));

            Assert.True(a.HasEfferentRelationshipWith(bbb, "Uses 2"));
            Assert.True(a.HasEfferentRelationshipWith(bb, "Uses 2"));
            Assert.True(a.HasEfferentRelationshipWith(b, "Uses 2"));
        }
            
    }
    
}