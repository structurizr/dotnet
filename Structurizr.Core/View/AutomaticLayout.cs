using System;
using System.Runtime.Serialization;

namespace Structurizr
{
    [DataContract]
    public class AutomaticLayout
    {

        /// <summary>
        /// The rank direction.
        /// </summary>
        [DataMember(Name = "rankDirection", EmitDefaultValue = true)]
        public RankDirection RankDirection;

        private int _rankSeparation;

        /// <summary>
        /// The rank separation (in pixels).
        /// </summary>
        [DataMember(Name = "rankSeparation", EmitDefaultValue = false)]
        public int RankSeparation
        {
            get { return _rankSeparation; }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("The rank separation must be a positive integer.");
                }

                _rankSeparation = value;
            }
        }

        private int _nodeSeparation;
        
        [DataMember(Name = "nodeSeparation", EmitDefaultValue = false)]
        public int NodeSeparation
        {
            get { return _nodeSeparation; }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("The node separation must be a positive integer.");
                }

                _nodeSeparation = value;
            }
        }

        private int _edgeSeparation;

        [DataMember(Name = "edgeSeparation", EmitDefaultValue = false)]
        public int EdgeSeparation
        {
            get { return _edgeSeparation; }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("The edge separation must be a positive integer.");
                }

                _edgeSeparation = value;
            }
        }

        /// <summary>
        /// Whether the automatic layout algorithm should create vertices.
        /// </summary>
        [DataMember(Name = "vertices", EmitDefaultValue = true)]
        public bool Vertices;

        internal AutomaticLayout()
        {
        }

        internal AutomaticLayout(RankDirection rankDirection, int rankSeparation, int nodeSeparation,
            int edgeSeparation, bool vertices)
        {
            RankDirection = rankDirection;
            RankSeparation = rankSeparation;
            NodeSeparation = nodeSeparation;
            EdgeSeparation = edgeSeparation;
            Vertices = vertices;
        }
        
    }

}