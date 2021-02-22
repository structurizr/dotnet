using System.IO;
using System.Linq;
using Structurizr.Core.Util;
using Structurizr.Documentation;

namespace Structurizr.Examples
{

    /// <summary>
    /// This is an example workspace to illustrate the key features of Structurizr,
    /// based around a fictional Internet Banking System for Big Bank plc.
    ///
    /// The live workspace is available to view at https://structurizr.com/share/36141
    /// </summary>
    public class BigBankPlc
    {
        
        private const long WorkspaceId = 36141;
        private const string ApiKey = "key";
        private const string ApiSecret = "secret";

        private const string ExistingSystemTag = "Existing System";
        private const string BankStaffTag = "Bank Staff";
        private const string WebBrowserTag = "Web Browser";
        private const string MobileAppTag = "Mobile App";
        private const string DatabaseTag = "Database";
        private const string FailoverTag = "Failover";

        public static Workspace Create()
        {
            Workspace workspace = new Workspace("Big Bank plc", "This is an example workspace to illustrate the key features of Structurizr, based around a fictional online banking system.");
            Model model = workspace.Model;
            model.ImpliedRelationshipsStrategy = new CreateImpliedRelationshipsUnlessAnyRelationshipExistsStrategy();
            ViewSet views = workspace.Views;

            model.Enterprise = new Enterprise("Big Bank plc");

            // people and software systems
            SoftwareSystem internetBankingSystem = model.AddSoftwareSystem(Location.Internal, "Internet Banking System", "Allows customers to view information about their bank accounts, and make payments.");
            Person customer = model.AddPerson(Location.External, "Personal Banking Customer", "A customer of the bank, with personal bank accounts.");

            customer.Uses(internetBankingSystem, "Views account balances, and makes payments using");

            SoftwareSystem mainframeBankingSystem = model.AddSoftwareSystem(Location.Internal, "Mainframe Banking System", "Stores all of the core banking information about customers, accounts, transactions, etc.");
            mainframeBankingSystem.AddTags(ExistingSystemTag);
            internetBankingSystem.Uses(mainframeBankingSystem, "Gets account information from, and makes payments using");

            SoftwareSystem emailSystem = model.AddSoftwareSystem(Location.Internal, "E-mail System", "The internal Microsoft Exchange e-mail system.");
            internetBankingSystem.Uses(emailSystem, "Sends e-mail using");
            emailSystem.AddTags(ExistingSystemTag);
            emailSystem.Delivers(customer, "Sends e-mails to");

            SoftwareSystem atm = model.AddSoftwareSystem(Location.Internal, "ATM", "Allows customers to withdraw cash.");
            atm.AddTags(ExistingSystemTag);
            atm.Uses(mainframeBankingSystem, "Uses");
            customer.Uses(atm, "Withdraws cash using");

            Person customerServiceStaff = model.AddPerson(Location.Internal, "Customer Service Staff", "Customer service staff within the bank.");
            customerServiceStaff.AddTags(BankStaffTag);
            customerServiceStaff.Uses(mainframeBankingSystem, "Uses");
            customer.InteractsWith(customerServiceStaff, "Asks questions to", "Telephone");

            Person backOfficeStaff = model.AddPerson(Location.Internal, "Back Office Staff", "Administration and support staff within the bank.");
            backOfficeStaff.AddTags(BankStaffTag);
            backOfficeStaff.Uses(mainframeBankingSystem, "Uses");

            // containers
            Container singlePageApplication = internetBankingSystem.AddContainer("Single-Page Application", "Provides all of the Internet banking functionality to customers via their web browser.", "JavaScript and Angular");
            singlePageApplication.AddTags(WebBrowserTag);
            Container mobileApp = internetBankingSystem.AddContainer("Mobile App", "Provides a limited subset of the Internet banking functionality to customers via their mobile device.", "Xamarin");
            mobileApp.AddTags(MobileAppTag);
            Container webApplication = internetBankingSystem.AddContainer("Web Application", "Delivers the static content and the Internet banking single page application.", "Java and Spring MVC");
            Container apiApplication = internetBankingSystem.AddContainer("API Application", "Provides Internet banking functionality via a JSON/HTTPS API.", "Java and Spring MVC");
            Container database = internetBankingSystem.AddContainer("Database", "Stores user registration information, hashed authentication credentials, access logs, etc.", "Relational Database Schema");
            database.AddTags(DatabaseTag);

            customer.Uses(webApplication, "Visits bigbank.com/ib using", "HTTPS");
            customer.Uses(singlePageApplication, "Views account balances, and makes payments using", "");
            customer.Uses(mobileApp, "Views account balances, and makes payments using", "");
            webApplication.Uses(singlePageApplication, "Delivers to the customer's web browser", "");
            apiApplication.Uses(database, "Reads from and writes to", "JDBC");
            apiApplication.Uses(mainframeBankingSystem, "Makes API calls to", "XML/HTTPS");
            apiApplication.Uses(emailSystem, "Sends e-mail using", "SMTP");

            // components
            // - for a real-world software system, you would probably want to extract the components using
            // - static analysis/reflection rather than manually specifying them all
            Component signinController = apiApplication.AddComponent("Sign In Controller", "Allows users to sign in to the Internet Banking System.", "Spring MVC Rest Controller");
            Component accountsSummaryController = apiApplication.AddComponent("Accounts Summary Controller", "Provides customers with a summary of their bank accounts.", "Spring MVC Rest Controller");
            Component resetPasswordController = apiApplication.AddComponent("Reset Password Controller", "Allows users to reset their passwords with a single use URL.", "Spring MVC Rest Controller");
            Component securityComponent = apiApplication.AddComponent("Security Component", "Provides functionality related to signing in, changing passwords, etc.", "Spring Bean");
            Component mainframeBankingSystemFacade = apiApplication.AddComponent("Mainframe Banking System Facade", "A facade onto the mainframe banking system.", "Spring Bean");
            Component emailComponent = apiApplication.AddComponent("E-mail Component", "Sends e-mails to users.", "Spring Bean");

            apiApplication.Components.Where(c => "Spring MVC Rest Controller".Equals(c.Technology)).ToList().ForEach(c => singlePageApplication.Uses(c, "Makes API calls to", "JSON/HTTPS"));
            apiApplication.Components.Where(c => "Spring MVC Rest Controller".Equals(c.Technology)).ToList().ForEach(c => mobileApp.Uses(c, "Makes API calls to", "JSON/HTTPS"));
            signinController.Uses(securityComponent, "Uses");
            accountsSummaryController.Uses(mainframeBankingSystemFacade, "Uses");
            resetPasswordController.Uses(securityComponent, "Uses");
            resetPasswordController.Uses(emailComponent, "Uses");
            securityComponent.Uses(database, "Reads from and writes to", "JDBC");
            mainframeBankingSystemFacade.Uses(mainframeBankingSystem, "Uses", "XML/HTTPS");
            emailComponent.Uses(emailSystem, "Sends e-mail using");

            // deployment nodes and container instances
            DeploymentNode developerLaptop = model.AddDeploymentNode("Development", "Developer Laptop", "A developer laptop.", "Microsoft Windows 10 or Apple macOS");
            DeploymentNode apacheTomcat = developerLaptop
                .AddDeploymentNode("Docker Container - Web Server", "A Docker container.", "Docker")
                .AddDeploymentNode("Apache Tomcat", "An open source Java EE web server.", "Apache Tomcat 8.x", 1, DictionaryUtils.Create("Xmx=512M", "Xms=1024M", "Java Version=8"));
            ContainerInstance developmentWebApplication = apacheTomcat.Add(webApplication);
            ContainerInstance developmentApiApplication = apacheTomcat.Add(apiApplication);

            DeploymentNode bigBankDataCenterForDevelopment = model.AddDeploymentNode("Development", "Big Bank plc", "", "Big Bank plc data center");
            SoftwareSystemInstance developmentMainframeBankingSystem = bigBankDataCenterForDevelopment
                .AddDeploymentNode("bigbank-dev001").Add(mainframeBankingSystem);

            ContainerInstance developmentDatabase = developerLaptop
                .AddDeploymentNode("Docker Container - Database Server", "A Docker container.", "Docker")
                .AddDeploymentNode("Database Server", "A development database.", "Oracle 12c")
                .Add(database);

            ContainerInstance developmentSinglePageApplication = developerLaptop
                .AddDeploymentNode("Web Browser", "", "Chrome, Firefox, Safari, or Edge")
                .Add(singlePageApplication);

            DeploymentNode customerMobileDevice = model.AddDeploymentNode("Live", "Customer's mobile device", "", "Apple iOS or Android");
            ContainerInstance liveMobileApp = customerMobileDevice.Add(mobileApp);

            DeploymentNode customerComputer = model.AddDeploymentNode("Live", "Customer's computer", "", "Microsoft Windows or Apple macOS");
            ContainerInstance liveSinglePageApplication = customerComputer
                .AddDeploymentNode("Web Browser", "", "Chrome, Firefox, Safari, or Edge")
                .Add(singlePageApplication);

            DeploymentNode bigBankDataCenterForLive =
                model.AddDeploymentNode("Live", "Big Bank plc", "", "Big Bank plc data center");
            SoftwareSystemInstance liveMainframeBankingSystem = bigBankDataCenterForLive
                .AddDeploymentNode("bigbank-prod001").Add(mainframeBankingSystem);

            DeploymentNode liveWebServer = bigBankDataCenterForLive.AddDeploymentNode("bigbank-web***", "A web server residing in the web server farm, accessed via F5 BIG-IP LTMs.", "Ubuntu 16.04 LTS", 4, DictionaryUtils.Create("Location=London and Reading"));
            ContainerInstance liveWebApplication = liveWebServer.AddDeploymentNode("Apache Tomcat", "An open source Java EE web server.", "Apache Tomcat 8.x", 1, DictionaryUtils.Create("Xmx=512M", "Xms=1024M", "Java Version=8"))
                .Add(webApplication);

            DeploymentNode liveApiServer = bigBankDataCenterForLive.AddDeploymentNode("bigbank-api***", "A web server residing in the web server farm, accessed via F5 BIG-IP LTMs.", "Ubuntu 16.04 LTS", 8, DictionaryUtils.Create("Location=London and Reading"));
            ContainerInstance liveApiApplication = liveApiServer.AddDeploymentNode("Apache Tomcat", "An open source Java EE web server.", "Apache Tomcat 8.x", 1, DictionaryUtils.Create("Xmx=512M", "Xms=1024M", "Java Version=8"))
                .Add(apiApplication);

            DeploymentNode primaryDatabaseServer = bigBankDataCenterForLive
                .AddDeploymentNode("bigbank-db01", "The primary database server.", "Ubuntu 16.04 LTS", 1, DictionaryUtils.Create("Location=London"))
                .AddDeploymentNode("Oracle - Primary", "The primary, live database server.", "Oracle 12c");
            ContainerInstance livePrimaryDatabase = primaryDatabaseServer.Add(database);

            DeploymentNode bigBankdb02 = bigBankDataCenterForLive.AddDeploymentNode("bigbank-db02", "The secondary database server.", "Ubuntu 16.04 LTS", 1, DictionaryUtils.Create("Location=Reading"));
            bigBankdb02.AddTags(FailoverTag);
            DeploymentNode secondaryDatabaseServer = bigBankdb02.AddDeploymentNode("Oracle - Secondary", "A secondary, standby database server, used for failover purposes only.", "Oracle 12c");
            secondaryDatabaseServer.AddTags(FailoverTag);
            ContainerInstance liveSecondaryDatabase = secondaryDatabaseServer.Add(database);

            model.Relationships.Where(r => r.Destination.Equals(liveSecondaryDatabase)).ToList().ForEach(r => r.AddTags(FailoverTag));
            Relationship dataReplicationRelationship = primaryDatabaseServer.Uses(secondaryDatabaseServer, "Replicates data to", "");
            liveSecondaryDatabase.AddTags(FailoverTag);

            // views/diagrams
            SystemLandscapeView systemLandscapeView = views.CreateSystemLandscapeView("SystemLandscape", "The system landscape diagram for Big Bank plc.");
            systemLandscapeView.AddAllElements();
            systemLandscapeView.PaperSize = PaperSize.A5_Landscape;

            SystemContextView systemContextView = views.CreateSystemContextView(internetBankingSystem, "SystemContext", "The system context diagram for the Internet Banking System.");
            systemContextView.EnterpriseBoundaryVisible = false;
            systemContextView.AddNearestNeighbours(internetBankingSystem);
            systemContextView.PaperSize = PaperSize.A5_Landscape;

            ContainerView containerView = views.CreateContainerView(internetBankingSystem, "Containers", "The container diagram for the Internet Banking System.");
            containerView.Add(customer);
            containerView.AddAllContainers();
            containerView.Add(mainframeBankingSystem);
            containerView.Add(emailSystem);
            containerView.PaperSize = PaperSize.A5_Landscape;

            ComponentView componentView = views.CreateComponentView(apiApplication, "Components", "The component diagram for the API Application.");
            componentView.Add(mobileApp);
            componentView.Add(singlePageApplication);
            componentView.Add(database);
            componentView.AddAllComponents();
            componentView.Add(mainframeBankingSystem);
            componentView.Add(emailSystem);
            componentView.PaperSize = PaperSize.A5_Landscape;

            systemLandscapeView.AddAnimation(internetBankingSystem, customer, mainframeBankingSystem, emailSystem);
            systemLandscapeView.AddAnimation(atm);
            systemLandscapeView.AddAnimation(customerServiceStaff, backOfficeStaff);

            systemContextView.AddAnimation(internetBankingSystem);
            systemContextView.AddAnimation(customer);
            systemContextView.AddAnimation(mainframeBankingSystem);
            systemContextView.AddAnimation(emailSystem);

            containerView.AddAnimation(customer, mainframeBankingSystem, emailSystem);
            containerView.AddAnimation(webApplication);
            containerView.AddAnimation(singlePageApplication);
            containerView.AddAnimation(mobileApp);
            containerView.AddAnimation(apiApplication);
            containerView.AddAnimation(database);

            componentView.AddAnimation(singlePageApplication, mobileApp);
            componentView.AddAnimation(signinController, securityComponent, database);
            componentView.AddAnimation(accountsSummaryController, mainframeBankingSystemFacade, mainframeBankingSystem);
            componentView.AddAnimation(resetPasswordController, emailComponent, database);

            // dynamic diagrams and deployment diagrams are not available with the Free Plan
            DynamicView dynamicView = views.CreateDynamicView(apiApplication, "SignIn", "Summarises how the sign in feature works in the single-page application.");
            dynamicView.Add(singlePageApplication, "Submits credentials to", signinController);
            dynamicView.Add(signinController, "Validates credentials using", securityComponent);
            dynamicView.Add(securityComponent, "select * from users where username = ?", database);
            dynamicView.Add(database, "Returns user data to", securityComponent);
            dynamicView.Add(securityComponent, "Returns true if the hashed password matches", signinController);
            dynamicView.Add(signinController, "Sends back an authentication token to", singlePageApplication);
            dynamicView.PaperSize = PaperSize.A5_Landscape;

            DeploymentView developmentDeploymentView = views.CreateDeploymentView(internetBankingSystem, "DevelopmentDeployment", "An example development deployment scenario for the Internet Banking System.");
            developmentDeploymentView.Environment = "Development";
            developmentDeploymentView.Add(developerLaptop);
            developmentDeploymentView.Add(bigBankDataCenterForDevelopment);
            developmentDeploymentView.PaperSize = PaperSize.A5_Landscape;

            developmentDeploymentView.AddAnimation(developmentSinglePageApplication);
            developmentDeploymentView.AddAnimation(developmentWebApplication, developmentApiApplication);
            developmentDeploymentView.AddAnimation(developmentDatabase);
            developmentDeploymentView.AddAnimation(developmentMainframeBankingSystem);

            DeploymentView liveDeploymentView = views.CreateDeploymentView(internetBankingSystem, "LiveDeployment", "An example live deployment scenario for the Internet Banking System.");
            liveDeploymentView.Environment = "Live";
            liveDeploymentView.Add(bigBankDataCenterForLive);
            liveDeploymentView.Add(customerMobileDevice);
            liveDeploymentView.Add(customerComputer);
            liveDeploymentView.Add(dataReplicationRelationship);
            liveDeploymentView.PaperSize = PaperSize.A5_Landscape;

            liveDeploymentView.AddAnimation(liveSinglePageApplication);
            liveDeploymentView.AddAnimation(liveMobileApp);
            liveDeploymentView.AddAnimation(liveWebApplication, liveApiApplication);
            liveDeploymentView.AddAnimation(livePrimaryDatabase);
            liveDeploymentView.AddAnimation(liveSecondaryDatabase);
            liveDeploymentView.AddAnimation(liveMainframeBankingSystem);

            // colours, shapes and other diagram styling
            Styles styles = views.Configuration.Styles;
            styles.Add(new ElementStyle(Tags.SoftwareSystem) { Background = "#1168bd", Color = "#ffffff" });
            styles.Add(new ElementStyle(Tags.Container) { Background = "#438dd5", Color = "#ffffff" });
            styles.Add(new ElementStyle(Tags.Component) { Background = "#85bbf0", Color = "#000000" });
            styles.Add(new ElementStyle(Tags.Person) { Background = "#08427b", Color = "#ffffff", Shape = Shape.Person, FontSize = 22 });
            styles.Add(new ElementStyle(ExistingSystemTag) { Background = "#999999", Color = "#ffffff" });
            styles.Add(new ElementStyle(BankStaffTag) { Background = "#999999", Color = "#ffffff" });
            styles.Add(new ElementStyle(WebBrowserTag) { Shape = Shape.WebBrowser });
            styles.Add(new ElementStyle(MobileAppTag) { Shape = Shape.MobileDeviceLandscape });
            styles.Add(new ElementStyle(DatabaseTag) { Shape = Shape.Cylinder });
            styles.Add(new ElementStyle(FailoverTag) { Opacity = 25 });
            styles.Add(new RelationshipStyle(FailoverTag) {Opacity = 25, Position = 70 });

            // documentation
            // - usually the documentation would be included from separate Markdown/AsciiDoc files, but this is just an example
            StructurizrDocumentationTemplate template = new StructurizrDocumentationTemplate(workspace);
            template.AddContextSection(internetBankingSystem, Format.Markdown,
                "Here is some context about the Internet Banking System...\n" +
                "![](embed:SystemLandscape)\n" +
                "![](embed:SystemContext)\n" +
                "### Internet Banking System\n...\n" +
                "### Mainframe Banking System\n...\n");
            template.AddContainersSection(internetBankingSystem, Format.Markdown,
                "Here is some information about the containers within the Internet Banking System...\n" +
                "![](embed:Containers)\n" +
                "### Web Application\n...\n" +
                "### Database\n...\n");
            template.AddComponentsSection(webApplication, Format.Markdown,
                "Here is some information about the API Application...\n" +
                "![](embed:Components)\n" +
                "### Sign in process\n" +
                "Here is some information about the Sign In Controller, including how the sign in process works...\n" +
                "![](embed:SignIn)");
            template.AddDevelopmentEnvironmentSection(internetBankingSystem, Format.AsciiDoc,
                "Here is some information about how to set up a development environment for the Internet Banking System...\n" +
                "image::embed:DevelopmentDeployment[]");
            template.AddDeploymentSection(internetBankingSystem, Format.AsciiDoc,
                "Here is some information about the live deployment environment for the Internet Banking System...\n" +
                "image::embed:LiveDeployment[]");

            return workspace;
        }

        static void Main()
        {
            StructurizrClient structurizrClient = new StructurizrClient(ApiKey, ApiSecret);
            structurizrClient.PutWorkspace(WorkspaceId, Create());
        }
        
    }

}