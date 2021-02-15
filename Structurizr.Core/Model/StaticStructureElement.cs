namespace Structurizr
{
    
    /// <summary>
    /// This is the superclass for model elements that describe the static structure
    /// of a software system, namely Person, SoftwareSystem, Container and Component.
    /// </summary>
    public abstract class StaticStructureElement : GroupableElement
    {

        /// <summary>
        /// Adds a unidirectional "uses" style relationship between this element and a software system.
        /// </summary>
        /// <param name="destination"> the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "uses", "gets data from", "sends data to")</param>
        public Relationship Uses(SoftwareSystem destination, string description)
        {
            return Uses(destination, description, null);
        }

        /// <summary>
        /// Adds a unidirectional "uses" style relationship between this element and a software system.
        /// </summary>
        /// <param name="destination"> the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "uses", "gets data from", "sends data to")</param>
        /// <param name="technology">the technology details (e.g. JSON/HTTPS)</param>
        public Relationship Uses(SoftwareSystem destination, string description, string technology)
        {
            return Uses(destination, description, technology, null);
        }

        /// <summary>
        /// Adds a unidirectional "uses" style relationship between this element and a software system.
        /// </summary>
        /// <param name="destination"> the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "uses", "gets data from", "sends data to")</param>
        /// <param name="technology">the technology details (e.g. JSON/HTTPS)</param>
        /// <param name="interactionStyle">the interaction style (sync vs async)</param>
        public Relationship Uses(SoftwareSystem destination, string description, string technology, InteractionStyle? interactionStyle)
        {
            return Uses(destination, description, technology, interactionStyle, new string[0]);
        }

        /// <summary>
        /// Adds a unidirectional "uses" style relationship between this element and a software system.
        /// </summary>
        /// <param name="destination"> the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "uses", "gets data from", "sends data to")</param>
        /// <param name="technology">the technology details (e.g. JSON/HTTPS)</param>
        /// <param name="interactionStyle">the interaction style (sync vs async)</param>
        /// <param name="tags">an array of tags</param>
        public Relationship Uses(SoftwareSystem destination, string description, string technology, InteractionStyle? interactionStyle, string[] tags)
        {
            return Model.AddRelationship(this, destination, description, technology, interactionStyle, tags);
        }

        /// <summary>
        /// Adds a unidirectional "uses" style relationship between this element and a container.
        /// </summary>
        /// <param name="destination">the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "uses", "gets data from", "sends data to")</param>
        public Relationship Uses(Container destination, string description)
        {
            return Uses(destination, description, null);
        }

        /// <summary>
        /// Adds a unidirectional "uses" style relationship between this element and a container.
        /// </summary>
        /// <param name="destination">the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "uses", "gets data from", "sends data to")</param>
        /// <param name="technology">the technology details (e.g. JSON/HTTPS)</param>
        public Relationship Uses(Container destination, string description, string technology)
        {
            return Uses(destination, description, technology, null);
        }

        /// <summary>
        /// Adds a unidirectional "uses" style relationship between this element and a container.
        /// </summary>
        /// <param name="destination">the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "uses", "gets data from", "sends data to")</param>
        /// <param name="technology">the technology details (e.g. JSON/HTTPS)</param>
        /// <param name="interactionStyle">the interaction style (Synchronous or Asynchronous)</param>
        public Relationship Uses(Container destination, string description, string technology, InteractionStyle? interactionStyle)
        {
            return Uses(destination, description, technology, interactionStyle, new string[0]);
        }

        /// <summary>
        /// Adds a unidirectional "uses" style relationship between this element and a container
        /// </summary>
        /// <param name="destination"> the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "uses", "gets data from", "sends data to")</param>
        /// <param name="technology">the technology details (e.g. JSON/HTTPS)</param>
        /// <param name="interactionStyle">the interaction style (sync vs async)</param>
        /// <param name="tags">an array of tags</param>
        public Relationship Uses(Container destination, string description, string technology, InteractionStyle? interactionStyle, string[] tags)
        {
            return Model.AddRelationship(this, destination, description, technology, interactionStyle, tags);
        }

        /// <summary>
        /// Adds a unidirectional "uses" style relationship between this element and a component.
        /// </summary>
        /// <param name="destination">the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "uses", "gets data from", "sends data to")</param>
        public Relationship Uses(Component destination, string description)
        {
            return Uses(destination, description, null);
        }

        /// <summary>
        /// Adds a unidirectional "uses" style relationship between this element and a component.
        /// </summary>
        /// <param name="destination">the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "uses", "gets data from", "sends data to")</param>
        /// <param name="technology">the technology details (e.g. JSON/HTTPS)</param>
        public Relationship Uses(Component destination, string description, string technology)
        {
            return Uses(destination, description, technology, null);
        }

        /// <summary>
        /// Adds a unidirectional "uses" style relationship between this element and a component.
        /// </summary>
        /// <param name="destination">the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "uses", "gets data from", "sends data to")</param>
        /// <param name="technology">the technology details (e.g. JSON/HTTPS)</param>
        /// <param name="interactionStyle">the interaction style (sync vs async)</param>
        public Relationship Uses(Component destination, string description, string technology, InteractionStyle? interactionStyle)
        {
            return Uses(destination, description, technology, interactionStyle, new string[0]);
        }

        /// <summary>
        /// Adds a unidirectional "uses" style relationship between this element and a component
        /// </summary>
        /// <param name="destination"> the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "uses", "gets data from", "sends data to")</param>
        /// <param name="technology">the technology details (e.g. JSON/HTTPS)</param>
        /// <param name="interactionStyle">the interaction style (sync vs async)</param>
        /// <param name="tags">an array of tags</param>
        public Relationship Uses(Component destination, string description, string technology, InteractionStyle? interactionStyle, string[] tags)
        {
            return Model.AddRelationship(this, destination, description, technology, interactionStyle, tags);
        }

        /// <summary>
        /// Adds a unidirectional relationship between this element and a person.
        /// </summary>
        /// <param name="destination">the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "sends e-mail to")</param>
        public Relationship Delivers(Person destination, string description)
        {
            return Delivers(destination, description, null);
        }

        /// <summary>
        /// Adds a unidirectional relationship between this element and a person.
        /// </summary>
        /// <param name="destination">the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "sends e-mail to")</param>
        /// <param name="technology">the technology details (e.g. JSON/HTTPS)</param>
        public Relationship Delivers(Person destination, string description, string technology)
        {
            return Delivers(destination, description, technology, null);
        }

        /// <summary>
        /// Adds a unidirectional relationship between this element and a person.
        /// </summary>
        /// <param name="destination">the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "sends e-mail to")</param>
        /// <param name="technology">the technology details (e.g. JSON/HTTPS)</param>
        /// <param name="interactionStyle">the interaction style (sync vs async)</param>
        public Relationship Delivers(Person destination, string description, string technology, InteractionStyle? interactionStyle)
        {
            return Delivers(destination, description, technology, interactionStyle, new string[0]);
        }

        /// <summary>
        /// Adds a unidirectional "uses" style relationship between this element and a person
        /// </summary>
        /// <param name="destination"> the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "uses", "gets data from", "sends data to")</param>
        /// <param name="technology">the technology details (e.g. JSON/HTTPS)</param>
        /// <param name="interactionStyle">the interaction style (sync vs async)</param>
        /// <param name="tags">an array of tags</param>
        public Relationship Delivers(Person destination, string description, string technology, InteractionStyle? interactionStyle, string[] tags)
        {
            return Model.AddRelationship(this, destination, description, technology, interactionStyle, tags);
        }

    }
    
}