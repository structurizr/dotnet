using Structurizr.Client;
using System;
using System.Linq;

namespace Structurizr.Examples
{
    class WidgetsLimited
    {

        private const string ExternalPersonTag = "External Person";
        private const string ExternalSoftwareSystemTag = "External Software System";

        private const string InternalPersonTag = "Internal Person";
        private const string InternalSoftwareSystemTag = "Internal Software System";

        public static void Main(string[] args)
        {
            Workspace workspace = new Workspace("Widgets Limited", "Sells widgets to customers online.");
            Model model = workspace.Model;
            ViewSet views = workspace.Views;
            Styles styles = views.Configuration.Styles;

            model.Enterprise = new Enterprise("Widgets Limited");

            Person customer = model.AddPerson(Location.External, "Customer", "A customer of Widgets Limited.");
            Person customerServiceUser = model.AddPerson(Location.Internal, "Customer Service Agent", "Deals with customer enquiries.");
            SoftwareSystem ecommerceSystem = model.AddSoftwareSystem(Location.Internal, "E-commerce System", "Allows customers to buy widgets online via the widgets.com website.");
            SoftwareSystem fulfilmentSystem = model.AddSoftwareSystem(Location.Internal, "Fulfilment System", "Responsible for processing and shipping of customer orders.");
            SoftwareSystem taxamo = model.AddSoftwareSystem(Location.External, "Taxamo", "Calculates local tax (for EU B2C customers) and acts as a front-end for Braintree Payments.");
            taxamo.Url= "https://www.taxamo.com";
            SoftwareSystem braintreePayments = model.AddSoftwareSystem(Location.External, "Braintree Payments", "Processes credit card payments on behalf of Widgets Limited.");
            braintreePayments.Url = "https://www.braintreepayments.com";
            SoftwareSystem jerseyPost = model.AddSoftwareSystem(Location.External, "Jersey Post", "Calculates worldwide shipping costs for packages.");

            model.People.Where(p => p.Location == Location.External).ToList().ForEach(p => p.AddTags(ExternalPersonTag));
            model.People.Where(p => p.Location == Location.Internal).ToList().ForEach(p => p.AddTags(InternalPersonTag));

            model.SoftwareSystems.Where(ss => ss.Location == Location.External).ToList().ForEach(ss => ss.AddTags(ExternalSoftwareSystemTag));
            model.SoftwareSystems.Where(ss => ss.Location == Location.Internal).ToList().ForEach(ss => ss.AddTags(InternalSoftwareSystemTag));

            customer.InteractsWith(customerServiceUser, "Asks questions to", "Telephone");
            customerServiceUser.Uses(ecommerceSystem, "Looks up order information using");
            customer.Uses(ecommerceSystem, "Places orders for widgets using");
            ecommerceSystem.Uses(fulfilmentSystem, "Sends order information to");
            fulfilmentSystem.Uses(jerseyPost, "Gets shipping charges from");
            ecommerceSystem.Uses(taxamo, "Delegates credit card processing to");
            taxamo.Uses(braintreePayments, "Uses for credit card processing");

            EnterpriseContextView enterpriseContextView = views.CreateEnterpriseContextView("enterpriseContext", "The enterprise context for Widgets Limited.");
            enterpriseContextView.AddAllElements();

            SystemContextView systemContextView = views.CreateSystemContextView(ecommerceSystem, "systemContext", "A description of the Widgets Limited e-commerce system.");
            systemContextView.AddNearestNeighbours(ecommerceSystem);
            systemContextView.Remove(customer.GetEfferentRelationshipWith(customerServiceUser));

            SystemContextView fulfilmentSystemContext = views.CreateSystemContextView(fulfilmentSystem, "FulfilmentSystemContext", "The system context diagram for the Widgets Limited fulfilment system.");
            fulfilmentSystemContext.AddNearestNeighbours(fulfilmentSystem);

            DynamicView dynamicView = views.CreateDynamicView("CustomerSupportCall", "A high-level overview of the customer support call process.");
            dynamicView.Add(customer, customerServiceUser);
            dynamicView.Add(customerServiceUser, ecommerceSystem);

            styles.Add(new ElementStyle(Tags.SoftwareSystem) { Shape = Shape.RoundedBox });
            styles.Add(new ElementStyle(Tags.Person) { Shape = Shape.Person });

            styles.Add(new ElementStyle(Tags.Element) { Color = "#ffffff" });
            styles.Add(new ElementStyle(ExternalPersonTag) { Background = "#EC5381" });
            styles.Add(new ElementStyle(ExternalSoftwareSystemTag) { Background = "#EC5381" });

            styles.Add(new ElementStyle(InternalPersonTag) { Background = "#B60037" });
            styles.Add(new ElementStyle(InternalSoftwareSystemTag) { Background = "#B60037" });

            StructurizrClient structurizrClient = new StructurizrClient("key", "secret");
            structurizrClient.MergeWorkspace(14471, workspace);
        }

    }
}
