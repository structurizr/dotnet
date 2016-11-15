using Structurizr.Client;
using System.IO;
using System.Threading.Tasks;
using Structurizr.Core.Async;

namespace Structurizr.Examples
{

    /// <summary>
    /// This is a simple example of how to create a software architecture model using Structurizr. The model
    /// represents a sample solution for the "Financial Risk System" architecture kata, included in
    /// "The Art of Visualising Software Architecture" book (available FREE from Leanpub).
    /// 
    /// The live version of the diagrams can be found at https://structurizr.com/public/9481
    /// </summary>
    class FinancialRiskSystem
    {

        private const string AlertTag = "Alert";

        static void Main(string[] args)
        {
            Workspace workspace = new Workspace("Financial Risk System", "A simple example C4 model based upon the financial risk system architecture kata, created using Structurizr for .NET");
            Model model = workspace.Model;

            // create the basic model
            SoftwareSystem financialRiskSystem = model.AddSoftwareSystem(Location.Internal, "Financial Risk System", "Calculates the bank's exposure to risk for product X");

            Person businessUser = model.AddPerson(Location.Internal, "Business User", "A regular business user");
            businessUser.Uses(financialRiskSystem, "Views reports using");

            Person configurationUser = model.AddPerson(Location.Internal, "Configuration User", "A regular business user who can also configure the parameters used in the risk calculations");
            configurationUser.Uses(financialRiskSystem, "Configures parameters using");

            SoftwareSystem tradeDataSystem = model.AddSoftwareSystem(Location.Internal, "Trade Data System", "The system of record for trades of type X");
            financialRiskSystem.Uses(tradeDataSystem, "Gets trade data from");

            SoftwareSystem referenceDataSystem = model.AddSoftwareSystem(Location.Internal, "Reference Data System", "Manages reference data for all counterparties the bank interacts with");
            financialRiskSystem.Uses(referenceDataSystem, "Gets counterparty data from");

            SoftwareSystem emailSystem = model.AddSoftwareSystem(Location.Internal, "E-mail system", "Microsoft Exchange");
            financialRiskSystem.Uses(emailSystem, "Sends a notification that a report is ready to");
            emailSystem.Delivers(businessUser, "Sends a notification that a report is ready to", "E-mail message", InteractionStyle.Asynchronous);

            SoftwareSystem centralMonitoringService = model.AddSoftwareSystem(Location.Internal, "Central Monitoring Service", "The bank-wide monitoring and alerting dashboard");
            financialRiskSystem.Uses(centralMonitoringService, "Sends critical failure alerts to", "SNMP", InteractionStyle.Asynchronous).AddTags(AlertTag);

            SoftwareSystem activeDirectory = model.AddSoftwareSystem(Location.Internal, "Active Directory", "Manages users and security roles across the bank");
            financialRiskSystem.Uses(activeDirectory, "Uses for authentication and authorisation");

            Container webApplication = financialRiskSystem.AddContainer("Web Application", "Allows users to view reports and modify risk calculation parameters", "ASP.NET MVC");
            businessUser.Uses(webApplication, "Views reports using");
            configurationUser.Uses(webApplication, "Modifies risk calculation parameters using");
            webApplication.Uses(activeDirectory, "Uses for authentication and authorisation");

            Container batchProcess = financialRiskSystem.AddContainer("Batch Process", "Calculates the risk", "Windows Service");
            batchProcess.Uses(emailSystem, "Sends a notification that a report is ready to");
            batchProcess.Uses(tradeDataSystem, "Gets trade data from");
            batchProcess.Uses(referenceDataSystem, "Gets counterparty data from");
            batchProcess.Uses(centralMonitoringService, "Sends critical failure alerts to", "SNMP", InteractionStyle.Asynchronous).AddTags(AlertTag);

            Container fileSystem = financialRiskSystem.AddContainer("File System", "Stores risk reports", "Network File Share");
            webApplication.Uses(fileSystem, "Consumes risk reports from");
            batchProcess.Uses(fileSystem, "Publishes risk reports to");

            Component scheduler = batchProcess.AddComponent("Scheduler", "Starts the risk calculation process at 5pm New York time", "Quartz.NET");
            Component orchestrator = batchProcess.AddComponent("Orchestrator", "Orchestrates the risk calculation process", "C#");
            Component tradeDataImporter = batchProcess.AddComponent("Trade data importer", "Imports data from the Trade Data System", "C#");
            Component referenceDataImporter = batchProcess.AddComponent("Reference data importer", "Imports data from the Reference Data System", "C#");
            Component riskCalculator = batchProcess.AddComponent("Risk calculator", "Calculates risk", "C#");
            Component reportGenerator = batchProcess.AddComponent("Report generator", "Generates a Microsoft Excel compatible risk report", "C# and Microsoft.Office.Interop.Excel");
            Component reportPublisher = batchProcess.AddComponent("Report distributor", "Publishes the report to the web application", "C#");
            Component emailComponent = batchProcess.AddComponent("E-mail component", "Sends e-mails", "C#");
            Component reportChecker = batchProcess.AddComponent("Report checker", "Checks that the report has been generated by 9am singapore time", "C#");
            Component alertComponent = batchProcess.AddComponent("Alert component", "Sends SNMP alerts", "C# and #SNMP Library");

            scheduler.Uses(orchestrator, "Starts");
            scheduler.Uses(reportChecker, "Starts");
            orchestrator.Uses(tradeDataImporter, "Imports data using");
            tradeDataImporter.Uses(tradeDataSystem, "Imports data from");
            orchestrator.Uses(referenceDataImporter, "Imports data using");
            referenceDataImporter.Uses(referenceDataSystem, "Imports data from");
            orchestrator.Uses(riskCalculator, "Calculates the risk using");
            orchestrator.Uses(reportGenerator, "Generates the risk report using");
            orchestrator.Uses(reportPublisher, "Publishes the risk report using");
            reportPublisher.Uses(fileSystem, "Publishes the risk report to");
            orchestrator.Uses(emailComponent, "Sends e-mail using");
            emailComponent.Uses(emailSystem, "Sends a notification that a report is ready to");
            reportChecker.Uses(alertComponent, "Sends alerts using");
            alertComponent.Uses(centralMonitoringService, "Sends alerts using", "SNMP", InteractionStyle.Asynchronous).AddTags(AlertTag);

            // create some views
            ViewSet viewSet = workspace.Views;
            SystemContextView contextView = viewSet.CreateSystemContextView(financialRiskSystem, "Context", "");
            contextView.PaperSize = PaperSize.A4_Landscape;
            contextView.AddAllSoftwareSystems();
            contextView.AddAllPeople();

            ContainerView containerView = viewSet.CreateContainerView(financialRiskSystem, "Containers", "");
            contextView.PaperSize = PaperSize.A4_Landscape;
            containerView.AddAllElements();

            ComponentView componentViewForBatchProcess = viewSet.CreateComponentView(batchProcess, "Components", "");
            contextView.PaperSize = PaperSize.A3_Landscape;
            componentViewForBatchProcess.AddAllElements();
            componentViewForBatchProcess.Remove(configurationUser);
            componentViewForBatchProcess.Remove(webApplication);
            componentViewForBatchProcess.Remove(activeDirectory);

            // tag and style some elements
            Styles styles = viewSet.Configuration.Styles;
            financialRiskSystem.AddTags("Risk System");

            styles.Add(new ElementStyle(Tags.Element) { Color = "#ffffff", FontSize = 34 });
            styles.Add(new ElementStyle("Risk System") { Background = "#8a458a" });
            styles.Add(new ElementStyle(Tags.SoftwareSystem) { Width = 650, Height = 400, Background = "#510d51", Shape = Shape.Box });
            styles.Add(new ElementStyle(Tags.Person) { Width = 550, Background = "#62256e", Shape = Shape.Person });
            styles.Add(new ElementStyle(Tags.Container) { Width = 650, Height = 400, Background = "#a46ba4", Shape = Shape.Box });
            styles.Add(new ElementStyle(Tags.Component) { Width = 550, Background = "#c9a1c9", Shape = Shape.Box });

            styles.Add(new RelationshipStyle(Tags.Relationship) { Thickness = 4, Dashed = false, FontSize = 32, Width = 400 });
            styles.Add(new RelationshipStyle(Tags.Synchronous) { Dashed = false });
            styles.Add(new RelationshipStyle(Tags.Asynchronous) { Dashed = true });
            styles.Add(new RelationshipStyle(AlertTag) { Color = "#ff0000" });

            Documentation documentation = workspace.Documentation;
            FileInfo documentationRoot = new FileInfo(@"..\..\FinancialRiskSystem");
            documentation.Add(financialRiskSystem, SectionType.Context, DocumentationFormat.Markdown, new FileInfo(Path.Combine(documentationRoot.FullName, "context.md")));
            documentation.Add(financialRiskSystem, SectionType.FunctionalOverview, DocumentationFormat.Markdown, new FileInfo(Path.Combine(documentationRoot.FullName, "functional-overview.md")));
            documentation.Add(financialRiskSystem, SectionType.QualityAttributes, DocumentationFormat.Markdown, new FileInfo(Path.Combine(documentationRoot.FullName, "quality-attributes.md")));
            documentation.AddImages(documentationRoot);

            // and upload the model to structurizr.com
            StructurizrClient structurizrClient = new StructurizrClient("key", "secret");
            structurizrClient.MergeWorkspace(9481, workspace);
        }

        static async void MainAsync(string[] args)
        {
            long workspaceId = 9481;
            AsyncStructurizrClient asyncClient = new AsyncStructurizrClient("key", "secret");

            // start getting workspace in the background
            Task<Workspace> currentWorkspaceTask = asyncClient.GetWorkspaceAsync(workspaceId);

            Workspace workspace = new Workspace("Financial Risk System", "A simple example C4 model based upon the financial risk system architecture kata, created using Structurizr for .NET");
            Model model = workspace.Model;

            // create the basic model
            SoftwareSystem financialRiskSystem = model.AddSoftwareSystem(Location.Internal, "Financial Risk System", "Calculates the bank's exposure to risk for product X");

            Person businessUser = model.AddPerson(Location.Internal, "Business User", "A regular business user");
            businessUser.Uses(financialRiskSystem, "Views reports using");

            Person configurationUser = model.AddPerson(Location.Internal, "Configuration User", "A regular business user who can also configure the parameters used in the risk calculations");
            configurationUser.Uses(financialRiskSystem, "Configures parameters using");

            SoftwareSystem tradeDataSystem = model.AddSoftwareSystem(Location.Internal, "Trade Data System", "The system of record for trades of type X");
            financialRiskSystem.Uses(tradeDataSystem, "Gets trade data from");

            SoftwareSystem referenceDataSystem = model.AddSoftwareSystem(Location.Internal, "Reference Data System", "Manages reference data for all counterparties the bank interacts with");
            financialRiskSystem.Uses(referenceDataSystem, "Gets counterparty data from");

            SoftwareSystem emailSystem = model.AddSoftwareSystem(Location.Internal, "E-mail system", "Microsoft Exchange");
            financialRiskSystem.Uses(emailSystem, "Sends a notification that a report is ready to");
            emailSystem.Delivers(businessUser, "Sends a notification that a report is ready to", "E-mail message", InteractionStyle.Asynchronous);

            SoftwareSystem centralMonitoringService = model.AddSoftwareSystem(Location.Internal, "Central Monitoring Service", "The bank-wide monitoring and alerting dashboard");
            financialRiskSystem.Uses(centralMonitoringService, "Sends critical failure alerts to", "SNMP", InteractionStyle.Asynchronous).AddTags(AlertTag);

            SoftwareSystem activeDirectory = model.AddSoftwareSystem(Location.Internal, "Active Directory", "Manages users and security roles across the bank");
            financialRiskSystem.Uses(activeDirectory, "Uses for authentication and authorisation");

            Container webApplication = financialRiskSystem.AddContainer("Web Application", "Allows users to view reports and modify risk calculation parameters", "ASP.NET MVC");
            businessUser.Uses(webApplication, "Views reports using");
            configurationUser.Uses(webApplication, "Modifies risk calculation parameters using");
            webApplication.Uses(activeDirectory, "Uses for authentication and authorisation");

            Container batchProcess = financialRiskSystem.AddContainer("Batch Process", "Calculates the risk", "Windows Service");
            batchProcess.Uses(emailSystem, "Sends a notification that a report is ready to");
            batchProcess.Uses(tradeDataSystem, "Gets trade data from");
            batchProcess.Uses(referenceDataSystem, "Gets counterparty data from");
            batchProcess.Uses(centralMonitoringService, "Sends critical failure alerts to", "SNMP", InteractionStyle.Asynchronous).AddTags(AlertTag);

            Container fileSystem = financialRiskSystem.AddContainer("File System", "Stores risk reports", "Network File Share");
            webApplication.Uses(fileSystem, "Consumes risk reports from");
            batchProcess.Uses(fileSystem, "Publishes risk reports to");

            Component scheduler = batchProcess.AddComponent("Scheduler", "Starts the risk calculation process at 5pm New York time", "Quartz.NET");
            Component orchestrator = batchProcess.AddComponent("Orchestrator", "Orchestrates the risk calculation process", "C#");
            Component tradeDataImporter = batchProcess.AddComponent("Trade data importer", "Imports data from the Trade Data System", "C#");
            Component referenceDataImporter = batchProcess.AddComponent("Reference data importer", "Imports data from the Reference Data System", "C#");
            Component riskCalculator = batchProcess.AddComponent("Risk calculator", "Calculates risk", "C#");
            Component reportGenerator = batchProcess.AddComponent("Report generator", "Generates a Microsoft Excel compatible risk report", "C# and Microsoft.Office.Interop.Excel");
            Component reportPublisher = batchProcess.AddComponent("Report distributor", "Publishes the report to the web application", "C#");
            Component emailComponent = batchProcess.AddComponent("E-mail component", "Sends e-mails", "C#");
            Component reportChecker = batchProcess.AddComponent("Report checker", "Checks that the report has been generated by 9am singapore time", "C#");
            Component alertComponent = batchProcess.AddComponent("Alert component", "Sends SNMP alerts", "C# and #SNMP Library");

            scheduler.Uses(orchestrator, "Starts");
            scheduler.Uses(reportChecker, "Starts");
            orchestrator.Uses(tradeDataImporter, "Imports data using");
            tradeDataImporter.Uses(tradeDataSystem, "Imports data from");
            orchestrator.Uses(referenceDataImporter, "Imports data using");
            referenceDataImporter.Uses(referenceDataSystem, "Imports data from");
            orchestrator.Uses(riskCalculator, "Calculates the risk using");
            orchestrator.Uses(reportGenerator, "Generates the risk report using");
            orchestrator.Uses(reportPublisher, "Publishes the risk report using");
            reportPublisher.Uses(fileSystem, "Publishes the risk report to");
            orchestrator.Uses(emailComponent, "Sends e-mail using");
            emailComponent.Uses(emailSystem, "Sends a notification that a report is ready to");
            reportChecker.Uses(alertComponent, "Sends alerts using");
            alertComponent.Uses(centralMonitoringService, "Sends alerts using", "SNMP", InteractionStyle.Asynchronous).AddTags(AlertTag);

            // create some views
            ViewSet viewSet = workspace.Views;
            SystemContextView contextView = viewSet.CreateSystemContextView(financialRiskSystem, "Context", "");
            contextView.PaperSize = PaperSize.A4_Landscape;
            contextView.AddAllSoftwareSystems();
            contextView.AddAllPeople();

            ContainerView containerView = viewSet.CreateContainerView(financialRiskSystem, "Containers", "");
            contextView.PaperSize = PaperSize.A4_Landscape;
            containerView.AddAllElements();

            ComponentView componentViewForBatchProcess = viewSet.CreateComponentView(batchProcess, "Components", "");
            contextView.PaperSize = PaperSize.A3_Landscape;
            componentViewForBatchProcess.AddAllElements();
            componentViewForBatchProcess.Remove(configurationUser);
            componentViewForBatchProcess.Remove(webApplication);
            componentViewForBatchProcess.Remove(activeDirectory);

            // tag and style some elements
            Styles styles = viewSet.Configuration.Styles;
            financialRiskSystem.AddTags("Risk System");

            styles.Add(new ElementStyle(Tags.Element) { Color = "#ffffff", FontSize = 34 });
            styles.Add(new ElementStyle("Risk System") { Background = "#8a458a" });
            styles.Add(new ElementStyle(Tags.SoftwareSystem) { Width = 650, Height = 400, Background = "#510d51", Shape = Shape.Box });
            styles.Add(new ElementStyle(Tags.Person) { Width = 550, Background = "#62256e", Shape = Shape.Person });
            styles.Add(new ElementStyle(Tags.Container) { Width = 650, Height = 400, Background = "#a46ba4", Shape = Shape.Box });
            styles.Add(new ElementStyle(Tags.Component) { Width = 550, Background = "#c9a1c9", Shape = Shape.Box });

            styles.Add(new RelationshipStyle(Tags.Relationship) { Thickness = 4, Dashed = false, FontSize = 32, Width = 400 });
            styles.Add(new RelationshipStyle(Tags.Synchronous) { Dashed = false });
            styles.Add(new RelationshipStyle(Tags.Asynchronous) { Dashed = true });
            styles.Add(new RelationshipStyle(AlertTag) { Color = "#ff0000" });

            Documentation documentation = workspace.Documentation;
            FileInfo documentationRoot = new FileInfo(@"..\..\FinancialRiskSystem");
            documentation.Add(financialRiskSystem, SectionType.Context, DocumentationFormat.Markdown, new FileInfo(Path.Combine(documentationRoot.FullName, "context.md")));
            documentation.Add(financialRiskSystem, SectionType.FunctionalOverview, DocumentationFormat.Markdown, new FileInfo(Path.Combine(documentationRoot.FullName, "functional-overview.md")));
            documentation.Add(financialRiskSystem, SectionType.QualityAttributes, DocumentationFormat.Markdown, new FileInfo(Path.Combine(documentationRoot.FullName, "quality-attributes.md")));
            documentation.AddImages(documentationRoot);

            // wait for background process to finish
            var currentWorkspace = await currentWorkspaceTask;

            // merge with new workspace
            var mergedWorkspace = asyncClient.MergeWorkspaces(currentWorkspace, workspace);

            // and upload the model to structurizr.com asynchronously
            await asyncClient.PutWorkspaceAsync(workspaceId, mergedWorkspace);
        }

    }
}
