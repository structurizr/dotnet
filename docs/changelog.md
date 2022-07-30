# Changelog

## 1.1.0 (unreleased)

- Provides a way to store view dimensions.
- Provides a way to add specific relationships to dynamic views (fixes #98). 

## 1.0.0 (14th May 2021)

- Adds support for custom ID generators.
- Improved the support for creating [implied relationships](docs/implied-relationships.md).
- Adds support for software system instances.
- `Parent` of `SoftwareSystemInstance` and `ContainerInstance` now returns the parent deployment node.
- Adds support for element groups.
- Removes restrictions related to adding "external" containers/components to dynamic views.
- Enhanced the logic to merge layout information of elements on views.
- Adds support for "deployment groups", providing a way to scope how software system/container instance relationships are replicated when added to deployment nodes. __breaking change__

## 0.9.7 (29th June 2020)

- Added an ExternalSoftwareSystemBoundariesVisible property to ContainerView, to set whether software system boundaries should be visible for "external" containers (those outside the software system in scope).
- Added an ExternalContainersBoundariesVisible property to ComponentView, to set whether container boundaries should be visible for "external" components (those outside the container in scope).
- Added a 16:10 ratio paper size.
- Added a "Component" element shape.
- Added a "Dotted" element border style.
- Components from any container can now be added to a component view.
- Added the ability to customize the symbols used when rendering metadata.
- Adds support for curved relationship routing.
- Adds support for multiple themes.
- Adds support for infrastructure nodes.
- Adds support for animation steps on deployment views.
- Fixes a bug where all deployment nodes would be added to a deployment view, even if that view had an environment set.

## 0.9.6 (29th February 2020)

- Added A1 and A0 paper sizes.
- Adds support for themes.
- Adds support for tags on deployment nodes.
- Adds support for URLs on relationships.

## 0.9.5 (24th December 2019)

- Fixes a deserialization issue with component views.

## 0.9.4 (3rd December 2019)

- The automatic layout algorithm can now be configured on individual views.

## 0.9.3 (22nd November 2019)

- Fixes a bug that allows relationships to be created between parents and children.
- Added support for perspectives on elements and relationships.
- Add new C4PlantUMLWriter with C4-PlantUML support (issue #47).
- Added support for element stroke colours.

## 0.9.2 (16th September 2019)

- Fixes a bug where element and relationship positioning is lost when workspaces are merged.

## 0.9.1 (10th September 2019)

- The terminology for relationships can now be customised.
- Added support for icons on element styles.
- Top-level deployment nodes can now be given an environment property, to represent which deployment environment they belong to (e.g. "Development", "Live", etc).
- Relationships can no longer be created between container instances (__breaking change__).
- Fixed issue #40 (WorkspaceUtils does not load containers' properties).
- Provided a way to customize the sort order when displaying the list of views.
- Added the ability to lock and unlock workspaces, to prevent concurrent updates.
- Fixes a bug with the PlantUML writer, where relationships were sorted incorrectly (alphabetically, rather than numerically).

## 0.9.0 (8th November 2018)

- Added the ability to specify users who should have read-write or read-only workspace access, via the ```workspace.Configuration.AddUser(username, role)``` method. 

## 0.8.3 (24th October 2018)

- Fixes a bug where client-side encrypted workspaces could not be created.

## 0.8.2 (23rd October 2018)

- Added name-value properties to relationships.
- Added the ability to define animations on the static structure diagrams.
- Removed support for colours in the corporate branding feature (__breaking change__).

## 0.8.0

- Added validation for hex colour codes (on ElementStyle and RelationshipStyle)
- Removed the "groups" property of documentation sections (__breaking change__).
- Added support for the HTTP-based health checks feature.
- Added an ```EndParallelSequence(boolean)``` method to the ```DynamicView``` class, which allows sequence numbering to continue.
- Added support for decision records.
- Moved PlantUML support to a separate Structurizr.PlantUML project.
- Separated the API client from the core library, and created a new Structurizr.Client project.
- Renamed the "type" property on Section to "title".

## 0.7.2

- Added some new shapes: web browser, mobile device (portrait and landscape), and robot.
- Added the ability to enable/disable the enterprise boundary on system landscape and system context views.
- Added the ability to customise the terminology used when rendering views.
- Added the ability to hide element metadata and/or descriptions.
- Bug fixes and performance enhancements.