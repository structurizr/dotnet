using System;
using Xunit;

namespace Structurizr.Core.Tests
{
    
    public class CreateImpliedRelationshipsUnlessAnyRelationshipExistsStrategyTests : AbstractTestBase
    {
    
        [Fact]
        public void Test_impliedRelationshipsAreCreated()
        {
            SoftwareSystem a = Model.AddSoftwareSystem("A", "");
            Container aa = a.AddContainer("AA", "", "");
            Component aaa = aa.AddComponent("AAA", "", "");

            SoftwareSystem b = Model.AddSoftwareSystem("B", "");
            Container bb = b.AddContainer("BB", "", "");
            Component bbb = bb.AddComponent("BBB", "", "");

            Model.ImpliedRelationshipsStrategy = new CreateImpliedRelationshipsUnlessAnyRelationshipExistsStrategy();

            aaa.Uses(bbb, "Uses 1", null, InteractionStyle.Asynchronous, new[] { "Tag 1", "Tag 2" });

            Assert.Equal(9, Model.Relationships.Count);
            Assert.True(aaa.HasEfferentRelationshipWith(bbb, "Uses 1"));

            // AAA->BBB implies AAA->BB AAA->B AA->BBB AA->BB AA->B A->BBB A->BB A->B
            Assert.True(aaa.HasEfferentRelationshipWith(bb, "Uses 1"));
            Assert.True(aaa.HasEfferentRelationshipWith(b, "Uses 1"));

            Assert.True(aa.HasEfferentRelationshipWith(bbb, "Uses 1"));
            Assert.True(aa.HasEfferentRelationshipWith(bb, "Uses 1"));
            Assert.True(aa.HasEfferentRelationshipWith(b, "Uses 1"));

            Assert.True(a.HasEfferentRelationshipWith(bbb, "Uses 1"));
            Assert.True(a.HasEfferentRelationshipWith(bb, "Uses 1"));
            Assert.True(a.HasEfferentRelationshipWith(b, "Uses 1"));

            // and all relationships should have the same interaction style and tags
            foreach (Relationship r in Model.Relationships)
            {
                Assert.Equal(InteractionStyle.Asynchronous, r.InteractionStyle);
                Assert.True(r.GetTagsAsSet().Contains("Tag 1"));
                Assert.True(r.GetTagsAsSet().Contains("Tag 2"));
            }

            // and add another relationship with a different description
            aaa.Uses(bbb, "Uses 2");
            Assert.Equal(10, Model.Relationships.Count); // no change
        }

        [Fact]
        public void Test_impliedRelationshipsAreCreated_UnlessAnyRelationshipExists()
        {
            SoftwareSystem a = Model.AddSoftwareSystem("A", "");
            Container aa = a.AddContainer("AA", "", "");
            Component aaa = aa.AddComponent("AAA", "", "");

            SoftwareSystem b = Model.AddSoftwareSystem("B", "");
            Container bb = b.AddContainer("BB", "", "");
            Component bbb = bb.AddComponent("BBB", "", "");

            Model.ImpliedRelationshipsStrategy = new CreateImpliedRelationshipsUnlessAnyRelationshipExistsStrategy();

            // add some higher level relationships
            aa.Uses(bb, "Uses");

            Assert.Equal(4, Model.Relationships.Count);
            Assert.True(aa.HasEfferentRelationshipWith(bb, "Uses"));

            // AA->BB implies AA->B A->BB A->B
            Assert.True(aa.HasEfferentRelationshipWith(b, "Uses"));
            Assert.True(a.HasEfferentRelationshipWith(bb, "Uses"));
            Assert.True(a.HasEfferentRelationshipWith(b, "Uses"));

            // and now a lower level relationship, which will be propagated to parents that don't already have relationships between them
            aaa.Uses(bbb, "Uses 1");

            Assert.Equal(9, Model.Relationships.Count);
            Assert.True(aaa.HasEfferentRelationshipWith(bbb, "Uses 1"));

            // AAA->BBB implies AAA->BB AAA->B AA->BBB AA->BB AA->B A->BBB A->BB A->B
            Assert.True(aaa.HasEfferentRelationshipWith(bb, "Uses 1"));
            Assert.True(aaa.HasEfferentRelationshipWith(b, "Uses 1"));

            Assert.True(aa.HasEfferentRelationshipWith(bbb, "Uses 1"));
            Assert.True(aa.HasEfferentRelationshipWith(bb, "Uses")); // existing relationship
            Assert.True(aa.HasEfferentRelationshipWith(b, "Uses"));  // existing relationship

            Assert.True(a.HasEfferentRelationshipWith(bbb, "Uses 1"));
            Assert.True(a.HasEfferentRelationshipWith(bb, "Uses")); // existing relationship
            Assert.True(a.HasEfferentRelationshipWith(b, "Uses")); // existing relationship
        }
            
    }
    
}