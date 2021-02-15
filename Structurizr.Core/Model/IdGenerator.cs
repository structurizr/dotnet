namespace Structurizr
{
    
    /// <summary>
    /// The interface that ID generators, used when creating IDs for model elements/relationships, must implement.
    /// </summary>
    public interface IdGenerator
    {

        /// <summary>
        /// Generates an ID for the specified model element.
        /// </summary>
        /// <param name="element">an Element instance</param>
        /// <returns>the ID</returns>
        string GenerateId(Element element);

        /// <summary>
        /// Generates an ID for the specified model element.
        /// </summary>
        /// <param name="relationship">a Relationship instance</param>
        /// <returns>the ID</returns>
        string GenerateId(Relationship relationship);

        /// <summary>
        /// Called when loading/deserializing a model, to indicate that the specified ID has been found
        /// (and shouldn't be reused when generating new IDs).
        /// </summary>
        /// <param name="id">he ID that has been found</param>
        void Found(string id);
        
    }
}
