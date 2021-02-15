namespace Structurizr
{
    
    /// <summary>
    /// The default strategy is to NOT create implied relationships.
    /// </summary>
    public class DefaultImpliedRelationshipsStrategy : AbstractImpliedRelationshipsStrategy
    {
        
        public override void CreateImpliedRelationships(Relationship relationship)
        {
            // do nothing
        }
        
    }
    
}