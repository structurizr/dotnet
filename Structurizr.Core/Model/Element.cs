using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Structurizr
{

    /// <summary>
    /// This is the superclass for all model elements.
    /// </summary>
    [DataContract]
    public abstract class Element : ModelItem
    {

        /// <summary>
        /// The name of this element.
        /// </summary>
        [DataMember(Name = "name", EmitDefaultValue = false)]
        public virtual string Name { get; internal set; }

        /// <summary>
        /// A short description of this element.
        /// </summary>
        [DataMember(Name = "description", EmitDefaultValue = false)]
        public string Description { get; set; }

        private string _url;

        /// <summary>
        /// The URL where more information about this element can be found.
        /// </summary>
        [DataMember(Name = "url", EmitDefaultValue = false)]
        public string Url
        {
            get
            {
                return _url;
            }

            set
            {
                if (value != null && value.Trim().Length > 0)
                {
                    if (Util.Url.IsUrl(value))
                    { 
                        this._url = value;
                    }
                    else
                    {
                        throw new ArgumentException(value + " is not a valid URL.");
                    }
                }
            }
        }

        public Model Model { get; set; }

        private HashSet<Relationship> _relationships;

        [DataMember(Name = "relationships", EmitDefaultValue = false)]
        public ISet<Relationship> Relationships
        {
            get
            {
                return new HashSet<Relationship>(_relationships);
            }

            internal set
            {
                _relationships = new HashSet<Relationship>(value);
            }
        }

        public abstract string CanonicalName { get; }

        public abstract Element Parent { get; set; }

        internal Element()
        {
            _relationships = new HashSet<Relationship>();
        }

        internal void AddRelationship(Relationship relationship)
        {
            _relationships.Add(relationship);
        }

        public bool Has(Relationship relationship)
        {
            return _relationships.Contains(relationship);
        }

        /// <summary>
        /// Determines whether this element has afferent (incoming) relationships.
        /// </summary>
        /// <returns>true if this element has afferent relationships, false otherwise</returns>
        public bool HasAfferentRelationships()
        {
            return Model.Relationships.Count(r => r.Destination == this) > 0;
        }

        /// <summary>
        /// Determines whether this element has an efferent (outgoing) relationship
        /// with the specified element.
        /// </summary>
        /// <param name="element">the element to look for</param>
        /// <returns>true if this element has an efferent relationship with the specified element, false otherwise</returns>
        public bool HasEfferentRelationshipWith(Element element)
        {
            return GetEfferentRelationshipWith(element) != null;
        }

        /// <summary>
        /// Determines whether this element has an efferent (outgoing) relationship with
        /// the specified element and description.
        /// </summary>
        /// <param name="element">the element to look for</param>
        /// <param name="description">the relationship description</param>
        /// <returns>true if this element has an efferent relationship with the specified element and description, false otherwise</returns>
        public bool HasEfferentRelationshipWith(Element element, string description)
        {
            return GetEfferentRelationshipWith(element, description) != null;
        }

        /// <summary>
        /// Gets the efferent (outgoing) relationships with the specified element.
        /// </summary>
        /// <param name="element">the element to look for</param>
        /// <returns>a Set of Relationship objects; empty if no relationships exist</returns>
        public ISet<Relationship> GetEfferentRelationshipsWith(Element element)
        {
            ISet<Relationship> set = new HashSet<Relationship>();

            if (element != null)
            {
                foreach (Relationship relationship in Relationships)
                {
                    if (relationship.Destination.Equals(element))
                    {
                        set.Add(relationship);
                    }
                }
            }

            return set;
        }
        
        /// <summary>
        /// Gets the efferent (outgoing) relationship with the specified element.
        /// </summary>
        /// <param name="element">the element to look for</param>
        /// <returns>a Relationship object if an efferent relationship exists, null otherwise</returns>
        public Relationship GetEfferentRelationshipWith(Element element)
        {
            if (element == null)
            {
                return null;
            }

            foreach (Relationship relationship in Relationships)
            {
                if (relationship.Destination.Equals(element))
                {
                    return relationship;
                }
            }

            return null;
        }
        
        /// <summary>
        /// Gets the efferent (outgoing) relationship with the specified element and description.
        /// </summary>
        /// <param name="element">the element to look for</param>
        /// <param name="description">the relationship description</param>
        /// <returns>a Relationship object, or null if the specified relationship doesn't exist</returns>
        public Relationship GetEfferentRelationshipWith(Element element, string description)
        {
            if (element == null)
            {
                return null;
            }

            if (description == null)
            {
                description = "";
            }

            foreach (Relationship relationship in Relationships)
            {
                if (relationship.Destination.Equals(element) && description.Equals(relationship.Description))
                {
                    return relationship;
                }
            }

            return null;
        }


        public override string ToString()
        {
            return "{" + Id + " | " + Name + " | " + Description + "}";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Element);
        }

        public bool Equals(Element element)
        {
            if (element == null)
            {
                return false;
            }

            if (element == this)
            {
                return true;
            }

            return CanonicalName.Equals(element.CanonicalName);
        }

    }
}
