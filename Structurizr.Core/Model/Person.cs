using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Structurizr
{

    /// <summary>
    /// A person who uses a software system.
    /// </summary>
    [DataContract]
    public class Person : Element, IEquatable<Person>
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
                return CanonicalNameSeparator + FormatForCanonicalName(Name);
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

        public override List<string> getRequiredTags()
        {
            string[] tags = {
                Structurizr.Tags.Element,
                Structurizr.Tags.Person
            };
            return tags.ToList();
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

        public Relationship InteractsWith(Person destination, string description)
        {
            Relationship relationship = new Relationship(this, destination, description);
            Model.AddRelationship(relationship);

            return relationship;
        }

        public Relationship InteractsWith(Person destination, string description, string technology)
        {
            Relationship relationship = new Relationship(this, destination, description, technology);
            Model.AddRelationship(relationship);

            return relationship;
        }

        public Relationship InteractsWith(Person destination, string description, string technology, InteractionStyle interactionStyle)
        {
            Relationship relationship = new Relationship(this, destination, description, technology, interactionStyle);
            Model.AddRelationship(relationship);

            return relationship;
        }

        public bool Equals(Person person)
        {
            return this.Equals(person as Element);
        }

    }
}
