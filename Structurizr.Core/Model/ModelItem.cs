using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Structurizr
{

    [DataContract]
    public abstract class ModelItem
    {

        /// <summary>
        /// The ID of this item in the model.
        /// </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public string Id { get; internal set; }

        private List<string> _tags = new List<string>();

        public IEnumerable<string> GetAllTags()
        {
            if (String.IsNullOrWhiteSpace(Tags))
                return Enumerable.Empty<string>();
            return Tags.Split(new [] { "," }, StringSplitOptions.RemoveEmptyEntries);
        }
        
        public ISet<string> GetTagsAsSet()
        {
            ISet<string> setOfTags = new HashSet<string>(GetRequiredTags());
            foreach (string tag in _tags)
            {
                setOfTags.Add(tag);
            }

            return setOfTags;
        }


        [DataMember(Name = "tags", EmitDefaultValue = false)]
        public string Tags
        {
            get
            {
                List<string> listOfTags = new List<string>(GetRequiredTags());
                listOfTags.AddRange(_tags);

                if (listOfTags.Count == 0)
                {
                    return "";
                }

                StringBuilder buf = new StringBuilder();
                foreach (string tag in listOfTags)
                {
                    buf.Append(tag);
                    buf.Append(",");
                }

                string tagsAsString = buf.ToString();
                return tagsAsString.Substring(0, tagsAsString.Length - 1);
            }

            set
            {
                this._tags.Clear();

                if (value == null)
                {
                    return;
                }

                this._tags.AddRange(value.Split(','));
            }
        }

        internal ModelItem()
        {
        }

        public void AddTags(params string[] tags)
        {
            if (tags == null)
            {
                return;
            }

            foreach (string tag in tags)
            {
                if (tag != null)
                {
                    string t = tag.Trim();
                    if (!_tags.Contains(t))
                    {
                        _tags.Add(t);
                    }
                }
            }
        }

        public virtual void RemoveTag(string tag)
        {
            if (tag != null)
            {
                this._tags.Remove(tag.Trim());
            }
        }

        public abstract List<string> GetRequiredTags();

        private Dictionary<string, string> _properties = new Dictionary<string, string>();

        /// <summary>
        /// The collection of name-value property pairs associated with this element, as a Dictionary.
        /// </summary>
        [DataMember(Name = "properties", EmitDefaultValue = false)]
        public Dictionary<string, string> Properties
        {
            get
            {
                return _properties;
            }

            internal set
            {
                if (value != null)
                {
                    _properties = value;
                }
                else
                {
                    _properties.Clear();
                }
            }
        }

        /// <summary>
        /// Adds a name-value pair property to this element. 
        /// </summary>
        /// <param name="name">the name of the property</param>
        /// <param name="value">the value of the property</param>
        /// <exception cref="ArgumentException"></exception>
        public void AddProperty(string name, string value)
        {
            if (String.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException("A property name must be specified.");
            }

            if (String.IsNullOrWhiteSpace(value)) {
                throw new ArgumentException("A property value must be specified.");
            }

            Properties[name] = value;
        }

        private ISet<Perspective> _perspectives = new HashSet<Perspective>();

        /// <summary>
        /// The set of perspectives associated with this element.
        /// </summary>
        [DataMember(Name = "perspectives", EmitDefaultValue = false)]
        public ISet<Perspective> Perspectives
        {
            get
            {
                return new HashSet<Perspective>(_perspectives);
            }

            internal set
            {
                _perspectives = new HashSet<Perspective>(value);
            }
        }

        /// <summary>
        /// Adds a perspective to this model item.
        /// </summary>
        /// <param name="name">the name of the perspective (e.g. "Security", must be unique)</param>
        /// <param name="description"></param>
        /// <returns>a Perspective object</returns>
        public Perspective AddPerspective(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("A name must be specified.");
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("A description must be specified.");
            }

            if (Perspectives.Contains(new Perspective(name, "")))
            {
                throw new ArgumentException("A perspective named \"" + name + "\" already exists.");
            }

            Perspective perspective = new Perspective(name, description);
            _perspectives.Add(perspective);

            return perspective;
        }

    }
}