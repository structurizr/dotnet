namespace Structurizr
{
    
    public abstract class AbstractImpliedRelationshipsStrategy : IImpliedRelationshipsStrategy
    {
        
        protected bool ImpliedRelationshipIsAllowed(Element source, Element destination)
        {
            if (source.Equals(destination))
            {
                return false;
            }

            return !(IsChildOf(source, destination) || IsChildOf(destination, source));
        }

        private bool IsChildOf(Element e1, Element e2)
        {
            if (e1 is Person || e2 is Person) {
                return false;
            }

            Element parent = e2.Parent;
            while (parent != null) {
                if (parent.Id.Equals(e1.Id)) {
                    return true;
                }

                parent = parent.Parent;
            }

            return false;
        }

        /// <summary>
        /// Called after a relationship has been created in the model,
        /// providing an opportunity to create any resulting implied relationships.
        /// </summary>
        /// <param name="relationship">the newly created Relationship</param>
        public abstract void CreateImpliedRelationships(Relationship relationship);
        
    }
    
}