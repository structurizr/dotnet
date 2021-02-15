using System;

namespace Structurizr
{
    
    /// <summary>
    /// An ID generator that simply uses a sequential number when generating IDs for model elements and relationships.
    /// This is the default ID generator.
    /// </summary>
    public class SequentialIntegerIdGeneratorStrategy : IdGenerator
    {

        private int Id = 0;

        public string GenerateId(Element element)
        {
            lock(this)
            {
                return "" + ++Id;
            }
        }

        
        public string GenerateId(Relationship relationship)
        {
            lock(this)
            {
                return "" + ++Id;
            }
        }

        public void Found(string id)
        {
            int idAsInt = int.Parse(id);
            if (idAsInt > Id)
            {
                Id = idAsInt;
            }
        }
        
    }
}
