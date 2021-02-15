using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Structurizr
{

    /// <summary>
    /// A person who uses a software system.
    /// </summary>
    [DataContract]
    public sealed class Person : StaticStructureElement, IEquatable<Person>
    {

        /// <summary>
        /// The location of this person.
        /// </summary>
        [DataMember(Name = "location", EmitDefaultValue = true)]
        public Location Location { get; set; }

        public override string CanonicalName
        {
            get
            {
                return new CanonicalNameGenerator().Generate(this);
            }
        }

        public override Element Parent
        {
            get
            {
                return null;
            }

            set
            {
            }
        }

        internal Person()
        {
        }

        public override List<string> GetRequiredTags()
        {
            return new List<string>
            {
                Structurizr.Tags.Element,
                Structurizr.Tags.Person
            };
        }

        public new Relationship Delivers(Person destination, string description)
        {
            throw new InvalidOperationException();
        }

        public new Relationship Delivers(Person destination, string description, string technology)
        {
            throw new InvalidOperationException();
        }

        public new Relationship Delivers(Person destination, string description, string technology, InteractionStyle interactionStyle)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Adds an interaction between this person and another. 
        /// </summary>
        /// <param name="destination">the Person being interacted with</param>
        /// <param name="description">a description of the interaction</param>
        /// <returns>the resulting Relationship</returns>
        public Relationship InteractsWith(Person destination, string description)
        {
            return InteractsWith(destination, description, null);
        }

        /// <summary>
        /// Adds an interaction between this person and another. 
        /// </summary>
        /// <param name="destination">the Person being interacted with</param>
        /// <param name="description">a description of the interaction</param>
        /// <param name="technology">the technology of the interaction (e.g. Telephone)</param>
        /// <returns>the resulting Relationship</returns>
        public Relationship InteractsWith(Person destination, string description, string technology)
        {
            return InteractsWith(destination, description, technology, null);
        }

        /// <summary>
        /// Adds an interaction between this person and another. 
        /// </summary>
        /// <param name="destination">the Person being interacted with</param>
        /// <param name="description">a description of the interaction</param>
        /// <param name="technology">the technology of the interaction (e.g. Telephone)</param>
        /// <param name="interactionStyle">the interaction style (e.g. Synchronous or Asynchronous)</param>
        /// <returns>the resulting Relationship</returns>
        public Relationship InteractsWith(Person destination, string description, string technology, InteractionStyle? interactionStyle)
        {
            return InteractsWith(destination, description, technology, interactionStyle, new string[0]);
        }

        /// <summary>
        /// Adds an interaction between this person and another. 
        /// </summary>
        /// <param name="destination">the Person being interacted with</param>
        /// <param name="description">a description of the interaction</param>
        /// <param name="technology">the technology of the interaction (e.g. Telephone)</param>
        /// <param name="interactionStyle">the interaction style (e.g. Synchronous or Asynchronous)</param>
        /// <param name="tags">an array of tags</param>
        /// <returns>the resulting Relationship</returns>
        public Relationship InteractsWith(Person destination, string description, string technology, InteractionStyle? interactionStyle, string[] tags)
        {
            return Model.AddRelationship(this, destination, description, technology, interactionStyle, tags);
        }

        public bool Equals(Person person)
        {
            return this.Equals(person as Element);
        }

    }
}
