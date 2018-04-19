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
    public abstract class Element : Taggable
    {

        public const string CanonicalNameSeparator = "/";

        /// <summary>
        /// The ID of this element in the model.
        /// </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public string Id { get; set; }

        /// <summary>
        /// The name of this element.
        /// </summary>
        [DataMember(Name = "name", EmitDefaultValue = false)]
        public virtual string Name { get; set; }

        /// <summary>
        /// A short description of this element.
        /// </summary>
        [DataMember(Name = "description", EmitDefaultValue = false)]
        public string Description { get; set; }

        private Dictionary<string, string> _properties = new Dictionary<string, string>();

        /// <summary>
        /// The collection of name-value property pairs associated with this element, as a Dictionary.
        /// </summary>
        [DataMember(Name = "properties", EmitDefaultValue = false)]
        public Dictionary<string, string> Properties
        {
            get
            {
                return new Dictionary<string, string>(_properties);
            }
            internal set { _properties = value; }
        }
        
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

        [DataMember(Name = "relationships", EmitDefaultValue = false)]
        public HashSet<Relationship> Relationships { get; set; }

        public abstract string CanonicalName { get; }

        public abstract Element Parent { get; set; }

        internal Element()
        {
            Relationships = new HashSet<Relationship>();
        }

        internal void AddRelationship(Relationship relationship)
        {
            Relationships.Add(relationship);
        }

        public bool Has(Relationship relationship)
        {
            return Relationships.Any(r => r.SourceId == relationship.SourceId && r.DestinationId == relationship.DestinationId);
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

        protected string FormatForCanonicalName(String name)
        {
            return name.Replace(CanonicalNameSeparator, "");
        }

        /// <summary>
        /// Adds a name-value pair property to this element. 
        /// </summary>
        /// <param name="name">the name of the property</param>
        /// <param name="value">the value of the property</param>
        /// <exception cref="IllegalArgumentException"></exception>
        public void AddProperty(string name, string value) {
            if (String.IsNullOrEmpty(name)) {
                throw new ArgumentException("A property name must be specified.");
            }

            if (String.IsNullOrEmpty(value)) {
                throw new ArgumentException("A property value must be specified.");
            }

            _properties[name] = value;
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

        public override int GetHashCode()
        {
            return CanonicalName.GetHashCode() ^ typeof(Element).GetHashCode();
        }
    }
}
