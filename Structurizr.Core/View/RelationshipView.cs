using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Structurizr
{

    /// <summary>
    /// An instance of a model relationship in a View.
    /// </summary>
    [DataContract]
    public sealed class RelationshipView : IEquatable<RelationshipView>
    {

        public Relationship Relationship { get; set; }

        private string id;

        /// <summary>
        /// The ID of the relationship.
        /// </summary>
        /// <value>The ID of the relationship.</value>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public string Id
        {
            get
            {
                if (this.Relationship != null)
                {
                    return this.Relationship.Id;
                }
                else
                {
                    return this.id;
                }
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// The order of this relationship (used in dynamic views only; e.g. 1.0, 1.1, 2.0, etc).
        /// </summary>
        [DataMember(Name = "order", EmitDefaultValue = false)]
        public string Order { get; set; }

        /// <summary>
        /// The description of this relationship (used in dynamic views only).
        /// </summary>
        [DataMember(Name = "description", EmitDefaultValue = false)]
        public string Description { get; set; }

        private List<Vertex> _vertices = new List<Vertex>();

        /// <summary>
        /// The set of vertices used to render the relationship.
        /// </summary>
        [DataMember(Name = "vertices", EmitDefaultValue = false)]
        public List<Vertex> Vertices
        {
            get
            {
                return new List<Vertex>(_vertices);
            }

            internal set
            {
                _vertices = new List<Vertex>(value);
            }
        }

        /// <summary>
        /// The routing of the line.
        /// </summary>
        [DataMember(Name = "routing", EmitDefaultValue = false)]
        public Routing? Routing { get; set; }

        private int? _position;

        /// <summary>
        /// The position of the annotation along the line; 0 (start) to 100 (end).
        /// </summary>
        [DataMember(Name = "position", EmitDefaultValue = false)]
        public int? Position
        {
            get { return _position; }
            set
            {
                if (value != null)
                {
                    if (value < 0)
                    {
                        _position = 0;
                    }
                    else if (value > 100)
                    {
                        _position = 100;
                    }
                    else
                    {
                        _position = value;
                    }
                }
            }
        }

        /// <summary>
        /// Whether this relationship view represents a response (used in dynamic views only). 
        /// </summary>
        /// <returns></returns>
        [DataMember(Name = "response", EmitDefaultValue = false)]
        public bool? Response;
        
        internal RelationshipView()
        {
            Vertices = new List<Vertex>();
        }

        internal RelationshipView(Relationship relationship)
        {
            Vertices = new List<Vertex>();
            this.Relationship = relationship;
        }

        public override string ToString()
        {
            return this.Relationship.ToString();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RelationshipView);
        }

        public bool Equals(RelationshipView relationshipView)
        {
            if (relationshipView == null)
            {
                return false;
            }
            if (relationshipView == this)
            {
                return true;
            }

            if (Description != null ? Description != relationshipView.Description : relationshipView.Description != null) return false;
            if (Id != relationshipView.Id) return false;
            return !(Order != null ? Order != relationshipView.Order : relationshipView.Order != null);

        }

        public override int GetHashCode()
        {
            int result = Id.GetHashCode();
            result = 31 * result + (Description != null ? Description.GetHashCode() : 0);
            result = 31 * result + (Order != null ? Order.GetHashCode() : 0);
            return result;
        }

        internal void CopyLayoutInformationFrom(RelationshipView source)
        {
            if (source != null)
            {
                this.Vertices = source.Vertices;
                this.Routing = source.Routing;
                this.Position = source.Position;
            }
        }
    }
}
