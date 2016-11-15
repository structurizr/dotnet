using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Structurizr
{

    /// <summary>
    /// A component (a grouping of related functionality behind an interface that runs inside a container).
    /// </summary>
    [DataContract]
    public class Component : Element, IEquatable<Component>
    {
        
        public override Element Parent { get; set; }

        public Container Container
        {
            get
            {
                return Parent as Container;
            }
        }

        /// <summary>
        /// The technology associated with this component (e.g. Spring Bean).
        /// </summary>
        [DataMember(Name="technology", EmitDefaultValue=false)]
        public string Technology { get; set; }
          
        /// <summary>
        /// The size of this component (e.g. lines of code).
        /// </summary>
        [DataMember(Name="size", EmitDefaultValue = true)]
        public long Size { get; set; }

        /// <summary>
        /// The implementation type (e.g. a fully qualified interface/class name).
        /// </summary>
        [DataMember(Name="code", EmitDefaultValue=false)]
        public HashSet<CodeElement> Code { get; internal set; }

        internal Component()
        {
            Code = new HashSet<CodeElement>();
        }

        public override string CanonicalName
        {
            get
            {
                return Parent.CanonicalName + CanonicalNameSeparator + FormatForCanonicalName(Name);
            }
        }

        public override List<string> getRequiredTags()
        {
            string[] tags = {
                Structurizr.Tags.Element,
                Structurizr.Tags.Component
            };
            return tags.ToList();
        }

        /// <summary>
        /// Gets the type of this component (e.g. a fully qualified interface/class name.
        /// </summary>
        public string Type
        {
            get
            {
                CodeElement codeElement = Code.FirstOrDefault(ce => ce.Role == CodeElementRole.Primary);
                return codeElement?.Type;
            }

            set
            {
                if (value != null && value.Trim().Length > 0)
                {
                    CodeElement codeElement = new CodeElement(value);
                    codeElement.Role = CodeElementRole.Primary;
                    AddSupportingType(codeElement);
                }
            }
        }

        internal void AddSupportingType(CodeElement codeElement)
        {
            Code.Add(codeElement);
        }

        public bool Equals(Component component)
        {
            return this.Equals(component as Element);
        }

    }
}
