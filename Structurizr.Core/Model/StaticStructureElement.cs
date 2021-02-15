namespace Structurizr
{
    
    /// <summary>
    /// This is the superclass for model elements that describe the static structure
    /// of a software system, namely Person, SoftwareSystem, Container and Component.
    /// </summary>
    public abstract class StaticStructureElement : GroupableElement
    {

        /// <summary>
        /// Adds a unidirectional "uses" style relationship between this element and another.
        /// </summary>
        /// <param name="destination"> the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "uses", "gets data from", "sends data to")</param>
        public Relationship Uses(SoftwareSystem destination, string description)
        {
            return Model.AddRelationship(this, destination, description);
        }

        /// <summary>
        /// Adds a unidirectional "uses" style relationship between this element and another.
        /// </summary>
        /// <param name="destination"> the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "uses", "gets data from", "sends data to")</param>
        /// <param name="technology">the technology details (e.g. JSON/HTTPS)</param>
        public Relationship Uses(SoftwareSystem destination, string description, string technology)
        {
            return Model.AddRelationship(this, destination, description, technology);
        }

        /// <summary>
        /// Adds a unidirectional "uses" style relationship between this element and another.
        /// </summary>
        /// <param name="destination"> the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "uses", "gets data from", "sends data to")</param>
        /// <param name="technology">the technology details (e.g. JSON/HTTPS)</param>
        /// <param name="interactionStyle">the interaction style (sync vs async)</param>
        public Relationship Uses(SoftwareSystem destination, string description, string technology, InteractionStyle interactionStyle)
        {
            return Model.AddRelationship(this, destination, description, technology, interactionStyle);
        }

        /// <summary>
        /// Adds a unidirectional "uses" style relationship between this element and a container.
        /// </summary>
        /// <param name="destination">the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "uses", "gets data from", "sends data to")</param>
        public Relationship Uses(Container destination, string description)
        {
            return Model.AddRelationship(this, destination, description);
        }

        /// <summary>
        /// Adds a unidirectional "uses" style relationship between this element and a container.
        /// </summary>
        /// <param name="destination">the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "uses", "gets data from", "sends data to")</param>
        /// <param name="technology">the technology details (e.g. JSON/HTTPS)</param>
        public Relationship Uses(Container destination, string description, string technology)
        {
            return Model.AddRelationship(this, destination, description, technology);
        }

        /// <summary>
        /// Adds a unidirectional "uses" style relationship between this element and a container.
        /// </summary>
        /// <param name="destination">the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "uses", "gets data from", "sends data to")</param>
        /// <param name="technology">the technology details (e.g. JSON/HTTPS)</param>
        /// <param name="interactionStyle">the interaction style (Synchronous or Asynchronous)</param>
        public Relationship Uses(Container destination, string description, string technology, InteractionStyle interactionStyle)
        {
            return Model.AddRelationship(this, destination, description, technology, interactionStyle);
        }

        /// <summary>
        /// Adds a unidirectional "uses" style relationship between this element and a component (within a container).
        /// </summary>
        /// <param name="destination">the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "uses", "gets data from", "sends data to")</param>
        public Relationship Uses(Component destination, string description)
        {
            return Model.AddRelationship(this, destination, description);
        }

        /// <summary>
        /// Adds a unidirectional "uses" style relationship between this element and a component (within a container).
        /// </summary>
        /// <param name="destination">the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "uses", "gets data from", "sends data to")</param>
        /// <param name="technology">the technology details (e.g. JSON/HTTPS)</param>
        public Relationship Uses(Component destination, string description, string technology)
        {
            return Model.AddRelationship(this, destination, description, technology);
        }

        /// <summary>
        /// Adds a unidirectional "uses" style relationship between this element and a component (within a container).
        /// </summary>
        /// <param name="destination">the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "uses", "gets data from", "sends data to")</param>
        /// <param name="technology">the technology details (e.g. JSON/HTTPS)</param>
        /// <param name="interactionStyle">the interaction style (sync vs async)</param>
        public Relationship Uses(Component destination, string description, string technology, InteractionStyle interactionStyle)
        {
            return Model.AddRelationship(this, destination, description, technology, interactionStyle);
        }

        /// <summary>
        /// Adds a unidirectional relationship between this element and a person.
        /// </summary>
        /// <param name="destination">the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "sends e-mail to")</param>
        public Relationship Delivers(Person destination, string description)
        {
            return Model.AddRelationship(this, destination, description);
        }

        /// <summary>
        /// Adds a unidirectional relationship between this element and a person.
        /// </summary>
        /// <param name="destination">the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "sends e-mail to")</param>
        /// <param name="technology">the technology details (e.g. JSON/HTTPS)</param>
        public Relationship Delivers(Person destination, string description, string technology)
        {
            return Model.AddRelationship(this, destination, description, technology);
        }

        /// <summary>
        /// Adds a unidirectional relationship between this element and a person.
        /// </summary>
        /// <param name="destination">the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "sends e-mail to")</param>
        /// <param name="technology">the technology details (e.g. JSON/HTTPS)</param>
        /// <param name="interactionStyle">the interaction style (sync vs async)</param>
        public Relationship Delivers(Person destination, string description, string technology, InteractionStyle interactionStyle)
        {
            return Model.AddRelationship(this, destination, description, technology, interactionStyle);
        }

    }
    
}