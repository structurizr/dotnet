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
    public sealed class Component : StaticStructureElement, IEquatable<Component>
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

        private HashSet<CodeElement> _codeElements;

        /// <summary>
        /// The implementation type (e.g. a fully qualified interface/class name).
        /// </summary>
        [DataMember(Name="code", EmitDefaultValue=false)]
        public ISet<CodeElement> CodeElements
        {
            get
            {
                return new HashSet<CodeElement>(_codeElements);
            }

            internal set
            {
                _codeElements = new HashSet<CodeElement>(value);
            }
        }

        internal Component()
        {
            _codeElements = new HashSet<CodeElement>();
        }

        public override string CanonicalName
        {
            get
            {
                return new CanonicalNameGenerator().Generate(this);
            }
        }

        public override List<string> GetRequiredTags()
        {
            return new List<string>
            {
                Structurizr.Tags.Element,
                Structurizr.Tags.Component
            };
        }

        /// <summary>
        /// Gets the type of this component (e.g. a fully qualified interface/class name).
        /// </summary>
        public string Type
        {
            get
            {
                CodeElement codeElement = _codeElements.FirstOrDefault(ce => ce.Role == CodeElementRole.Primary);
                return codeElement?.Type;
            }

            set
            {
                if (value != null && value.Trim().Length > 0)
                {
                    _codeElements.RemoveWhere(ce => ce.Role == CodeElementRole.Primary);
                    CodeElement codeElement = new CodeElement(value);
                    codeElement.Role = CodeElementRole.Primary;
                    _codeElements.Add(codeElement);
                }
            }
        }

        public CodeElement AddSupportingType(string type)
        {
            CodeElement codeElement = new CodeElement(type);
            codeElement.Role = CodeElementRole.Supporting;
            _codeElements.Add(codeElement);

            return codeElement;
        }

        public bool Equals(Component component)
        {
            return this.Equals(component as Element);
        }

    }
}