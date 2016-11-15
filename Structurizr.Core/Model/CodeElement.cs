using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Structurizr
{

    /// <summary>
    /// Represents a code element, such as a class or interface,
    /// that is part of the implementation of a component.
    /// </summary>
    [DataContract]
    public class CodeElement : IEquatable<CodeElement>
    {

        [DataMember(Name = "role", EmitDefaultValue = true)]
        public CodeElementRole Role { get; set; }

        [DataMember(Name = "name", EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(Name = "type", EmitDefaultValue = false)]
        public readonly string Type;

        [DataMember(Name = "description", EmitDefaultValue = false)]
        public string Description { get; set; }

        private string _url;

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
                    Uri uri;
                    bool result = Uri.TryCreate(value, UriKind.Absolute, out uri);
                    if (result)
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

        [DataMember(Name = "language", EmitDefaultValue = false)]
        public string Language { get; set; }

        [DataMember(Name = "size", EmitDefaultValue = true)]
        public long Size { get; set; }

        [JsonConstructor]
        internal CodeElement()
        {
        }

        public CodeElement(string fullyQualifiedTypeName)
        {
            if (fullyQualifiedTypeName == null || fullyQualifiedTypeName.Trim().Length == 0)
            {
                throw new ArgumentException("A fully qualified name must be provided.");
            }

            string typeName = fullyQualifiedTypeName.Substring(0, fullyQualifiedTypeName.IndexOf(","));
            int dot = typeName.LastIndexOf('.');
            if (dot > -1)
            {
                Name = typeName.Substring(dot + 1);
                Type = fullyQualifiedTypeName;
            }
            else {
                Name = typeName;
                Type = fullyQualifiedTypeName;
            }

            Language = "C#";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CodeElement);
        }

        public bool Equals(CodeElement other)
        {
            return other != null && other.Type.Equals(Type);
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }

    }
}
