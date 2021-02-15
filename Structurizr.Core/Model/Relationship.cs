using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Structurizr
{

    /// <summary>
    /// A relationship between two elements.
    /// </summary>
    [DataContract]
    public sealed class Relationship : ModelItem, IEquatable<Relationship>
    {

        private string _description;

        /// <summary>
        /// A short description of this relationship.
        /// </summary>
        [DataMember(Name = "description", EmitDefaultValue = false)]
        public string Description
        {
            get
            {
                return _description ?? "";
            }

            internal set { _description = value; }
        }

        private string _sourceId;

        /// <summary>
        /// The ID of the source element.
        /// </summary>
        [DataMember(Name = "sourceId", EmitDefaultValue = false)]
        public string SourceId
        {
            get
            {
                if (Source != null)
                {
                    return Source.Id;
                }
                else
                {
                    return _sourceId;
                }
            }
            set
            {
                _sourceId = value;
            }
        }

        public Element Source { get; set; }

        private string _destinationId;

        /// <summary>
        /// The ID of the destination element.
        /// </summary>
        [DataMember(Name = "destinationId", EmitDefaultValue = false)]
        public string DestinationId
        {
            get
            {
                if (Destination != null)
                {
                    return Destination.Id;
                }
                else
                {
                    return _destinationId;
                }
            }
            set
            {
                _destinationId = value;
            }
        }

        public Element Destination { get; set; }

        /// <summary>
        /// The technology associated with this relationship (e.g. HTTPS, JDBC, etc).
        /// </summary>
        [DataMember(Name = "technology", EmitDefaultValue = false)]
        public string Technology { get; internal set; }

        private InteractionStyle? _interactionStyle;

        [DataMember(Name = "linkedRelationshipId", EmitDefaultValue = false)]
        public string LinkedRelationshipId { get; internal set; }

        /// <summary>
        /// The interaction style (synchronous or asynchronous).
        /// </summary>
        [DataMember(Name = "interactionStyle", EmitDefaultValue = false)]
        public InteractionStyle? InteractionStyle
        {
            get
            {
                return _interactionStyle;
            }
            set
            {
                _interactionStyle = value;
            }
        }
        
        private string _url;

        /// <summary>
        /// The URL where more information about this relationship can be found.
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

        internal Relationship()
        {
        }

        internal Relationship(Element source, Element destination, string description, string technology, InteractionStyle? interactionStyle, string[] tags) :
            this()
        {
            Source = source;
            Destination = destination;
            Description = description;
            Technology = technology;
            InteractionStyle = interactionStyle;

            AddTags(tags);
        }

        public override List<string> GetRequiredTags()
        {
            if (LinkedRelationshipId == null) {
                List<string> tags = new List<string>(); 
                tags.Add(Structurizr.Tags.Relationship);
                
                if (InteractionStyle == Structurizr.InteractionStyle.Synchronous)
                {
                    tags.Add(Structurizr.Tags.Synchronous);
                }
                else if (InteractionStyle == Structurizr.InteractionStyle.Asynchronous)
                {
                    tags.Add(Structurizr.Tags.Asynchronous);
                }
                
                return tags.ToList();
            } else {
                return new List<string>();
            }
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Relationship);
        }

        public bool Equals(Relationship relationship)
        {
            if (relationship == null)
            {
                return false;
            }

            if (relationship == this)
            {
                return true;
            }

            if (!Description.Equals(relationship.Description)) return false;
            if (!Destination.Equals(relationship.Destination)) return false;
            if (!Source.Equals(relationship.Source)) return false;

            return true;
        }

        public override int GetHashCode()
        {
            int result = SourceId.GetHashCode();
            result = 31 * result + DestinationId.GetHashCode();
            result = 31*result + Description.GetHashCode();
            return result;
        }

        public override string ToString()
        {
            return Source.ToString() + " ---[" + Description + "]---> " + Destination.ToString();
        }

    }
}