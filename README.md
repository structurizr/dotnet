![Structurizr](docs/images/structurizr-banner.png)

# Structurizr for .NET

This GitHub repository is an official client library for the [Structurizr](https://structurizr.com) cloud service and on-premises installation, both of which are web-based publishing platforms for software architecture models based upon the [C4 model](https://c4model.com). __This repository is supported by Structurizr Limited__, as a part of the Structurizr service.

The component finder, adr-tools importer, and alternative diagram export formats (e.g. PlantUML) can be found at [Structurizr for .NET extensions](https://github.com/structurizr/dotnet-extensions).

## A quick example

As an example, the following C# code can be used to create a software architecture model that describes a user using a software system.

```c#
Workspace workspace = new Workspace("Getting Started", "This is a model of my software system.");
Model model = workspace.Model;

Person user = model.AddPerson("User", "A user of my software system.");
SoftwareSystem softwareSystem = model.AddSoftwareSystem("Software System", "My software system.");
user.Uses(softwareSystem, "Uses");

ViewSet viewSet = workspace.Views;
SystemContextView contextView = viewSet.CreateSystemContextView(softwareSystem, "SystemContext", "An example of a System Context diagram.");
contextView.AddAllSoftwareSystems();
contextView.AddAllPeople();

Styles styles = viewSet.Configuration.Styles;
styles.Add(new ElementStyle(Tags.SoftwareSystem) { Background = "#1168bd", Color = "#ffffff" });
styles.Add(new ElementStyle(Tags.Person) { Background = "#08427b", Color = "#ffffff", Shape = Shape.Person });
```

The view can then be exported to be visualised using the [Structurizr service](https://structurizr.com), or other formats including PlantUML, WebSequenceDiagrams and Graphviz via the [Structurizr for .NET extensions](https://github.com/structurizr/dotnet-extensions).

![Views can be exported and visualised in many ways; e.g. PlantUML, Structurizr and Graphviz](docs/images/readme-1.png)

[![Build status](https://ci.appveyor.com/api/projects/status/t7oph9oynedawkm0?svg=true)](https://ci.appveyor.com/project/structurizr/dotnet)

## Table of contents

* Introduction
    * [Getting started](docs/getting-started.md)
    * [About Structurizr and how it compares to other tooling](https://structurizr.com/help/about)
    * [Basic concepts](https://structurizr.com/help/concepts) (workspaces, models, views and documentation)
    * [C4 model](https://structurizr.com/help/c4)
    * [Binaries](docs/binaries.md)
    * [API Client](docs/api-client.md)
    * [Usage patterns](docs/usage-patterns.md)
* Diagrams
    * [System Context diagram](docs/system-context-diagram.md)
    * [Container diagram](docs/container-diagram.md)
    * [Component diagram](docs/component-diagram.md)
    * [Dynamic diagram](docs/dynamic-diagram.md)
    * [Deployment diagram](docs/deployment-diagram.md)
    * [System Landscape diagram](docs/system-landscape-diagram.md)
    * [Styling elements](docs/styling-elements.md)
    * [Styling relationships](docs/styling-relationships.md)
    * [Filtered views](docs/filtered-views.md)
* Documentation
    * [Documentation overview](docs/documentation.md)
    * [Structurizr](docs/documentation-structurizr.md)
    * [arc42](docs/documentation-arc42.md)
    * [Viewpoints and Perspectives](docs/documentation-viewpoints-and-perspectives.md)
* Other
	* [HTTP-based health checks](docs/health-checks.md)
    * [Client-side encryption](docs/client-side-encryption.md)
    * [Corporate branding](docs/corporate-branding.md)
* Related projects
    * [dotnet-extensions](https://github.com/structurizr/dotnet-extensions): A collection of Structurizr for .NET extensions; including the ability to extract software architecture information from code, export views to PlantUML, etc.
    * [dotnet-core-quickstart](https://github.com/structurizr/dotnet-core-quickstart): A quickstart for .NET Core
    * [dotnet-framework-quickstart](https://github.com/structurizr/dotnet-framework-quickstart): A quickstart for .NET Framework
    * [structurizr-java](https://github.com/structurizr/java): Structurizr for Java
* [changelog](docs/changelog.md)
