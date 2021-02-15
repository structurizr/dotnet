namespace Structurizr
{
    
    /// <summary>
    ///  Defines the interface for strategies to create implied relationships in the model,
    /// after a relationship has been created.
    /// </summary>
    public interface IImpliedRelationshipsStrategy
    {
        
        /// <summary>
        /// Called after a relationship has been created in the model,
        /// providing an opportunity to create any resulting implied relationships.
        /// </summary>
        /// <param name="relationship">the newly created Relationship</param>
        void CreateImpliedRelationships(Relationship relationship);
    
    }
    
}