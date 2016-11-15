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
    public class Relationship : Taggable, IEquatable<Relationship>
    {

        /// <summary>
        /// The ID of this relationship in the model.
        /// </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public string Id { get; set; }

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

            set { _description = value; }
        }

        private string sourceId;

        /// <summary>
        /// The ID of the source element.
        /// </summary>
        [DataMember(Name = "sourceId", EmitDefaultValue = false)]
        public string SourceId
        {
            get
            {
                if (this.Source != null)
                {
                    return this.Source.Id;
                }
                else {
                    return this.sourceId;
                }
            }
            set
            {
                this.sourceId = value;
            }
        }

        public Element Source { get; set; }

        private string destinationId;

        /// <summary>
        /// The ID of the destination element.
        /// </summary>
        [DataMember(Name = "destinationId", EmitDefaultValue = false)]
        public string DestinationId
        {
            get
            {
                if (this.Destination != null)
                {
                    return this.Destination.Id;
                }
                else {
                    return this.destinationId;
                }
            }
            set
            {
                this.destinationId = value;
            }
        }

        public Element Destination { get; set; }

        /// <summary>
        /// The technology associated with this relationship (e.g. HTTPS, JDBC, etc).
        /// </summary>
        [DataMember(Name = "technology", EmitDefaultValue = false)]
        public string Technology { get; set; }

        private InteractionStyle interactionStyle = InteractionStyle.Synchronous;

        /// <summary>
        /// The interaction style (synchronous or asynchronous).
        /// </summary>
        [DataMember(Name = "interactionStyle", EmitDefaultValue = false)]
        public InteractionStyle InteractionStyle
        {
            get
            {
                return this.interactionStyle;
            }
            set
            {
                this.interactionStyle = value;
                if (this.interactionStyle == InteractionStyle.Synchronous)
                {
                    RemoveTag(Structurizr.Tags.Asynchronous);
                    AddTags(Structurizr.Tags.Synchronous);
                }
                else {
                    RemoveTag(Structurizr.Tags.Synchronous);
                    AddTags(Structurizr.Tags.Asynchronous);
                }

            }
        }

        internal Relationship()
        {
        }

        internal Relationship(Element source, Element destination, string description) :
            this(source, destination, description, null)
        {
        }

        internal Relationship(Element source, Element destination, string description, string technology) :
            this(source, destination, description, technology, InteractionStyle.Synchronous)
        {
        }

        internal Relationship(Element source, Element destination, string description, string technology, InteractionStyle interactionStyle) :
            this()
        {
            this.Source = source;
            this.Destination = destination;
            this.Description = description;
            this.Technology = technology;
            this.InteractionStyle = interactionStyle;
        }

        public override List<string> getRequiredTags()
        {
            string[] tags = {
                Structurizr.Tags.Relationship
            };
            return tags.ToList();
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
