using System;
using System.Runtime.Serialization;

namespace Structurizr
{
    
    public abstract class GroupableElement : Element
    {

        private string _group;
        
        /// <summary>
        /// The name of the group in which this element should be included in.
        /// </summary>
        [DataMember(Name = "group", EmitDefaultValue = false)]
        public string Group
        {
            get
            {
                return _group;
            }

            set
            {
                if (value == null)
                {
                    _group = null;
                }
                else {
                    _group = value.Trim();

                    if (String.IsNullOrEmpty(_group))
                    {
                        _group = null;
                    }
                }
            }
        }

    }
    
}