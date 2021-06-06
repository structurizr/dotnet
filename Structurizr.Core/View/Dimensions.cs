using System;
using System.Runtime.Serialization;

namespace Structurizr
{
    [DataContract]
    public class Dimensions
    {

        private int _width;
        
        /// <summary>
        /// The width (pixels).
        /// </summary>
        [DataMember(Name = "width", EmitDefaultValue = false)]
        public int Width
        {
            get { return _width; }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("The width must be a positive integer.");
                }

                _width = value;
            }
        }

        private int _height;
        
        /// <summary>
        /// The height (pixels).
        /// </summary>
        [DataMember(Name = "height", EmitDefaultValue = false)]
        public int Height
        {
            get { return _height; }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("The height must be a positive integer.");
                }

                _height = value;
            }
        }

        internal Dimensions()
        {
        }

        public Dimensions(int width, int height)
        {
            Width = width;
            Height = height;
        }
        
    }

}