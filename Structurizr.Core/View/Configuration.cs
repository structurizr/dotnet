using System.Runtime.Serialization;

namespace Structurizr
{

    /// <summary>
    /// The configuration associated with a set of views.
    /// </summary>
    [DataContract]
    public class Configuration
    {

        internal Configuration()
        {
            this.Styles = new Styles();
        }
        
        [DataMember(Name="styles", EmitDefaultValue=false)]
        public Styles Styles { get; set; }

        [DataMember(Name = "defaultView", EmitDefaultValue = false)]
        public string DefaultView { get; private set; }

        /// <summary>
        /// Sets the view that should be shown by default.
        /// </summary>
        /// <param name="view">A View object</param>
        public void SetDefaultView(View view)
        {
            if (view != null)
            {
                this.DefaultView = view.Key;
            }
        }

        [DataMember(Name = "lastSavedView", EmitDefaultValue = false)]
        internal string LastSavedView { get; set; }

        public void CopyConfigurationFrom(Configuration configuration)
        {
            LastSavedView = configuration.LastSavedView;
        }

    }
}
