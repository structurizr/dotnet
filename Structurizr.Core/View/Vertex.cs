using System.Runtime.Serialization;

namespace Structurizr
{

    /// <summary>
    /// The X, Y coordinate of a bend in a line.
    /// </summary>
    [DataContract]
    public sealed class Vertex
    {

        internal Vertex()
        {
        }

        internal Vertex(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// The horizontal position of the vertex when rendered.
        /// </summary>
        [DataMember(Name="x", EmitDefaultValue=false)]
        public int? X { get; set; }
  
        
        /// <summary>
        /// The vertical position of the vertex when rendered.
        /// </summary>
        [DataMember(Name="y", EmitDefaultValue=false)]
        public int? Y { get; set; }
  
    }
    
}
