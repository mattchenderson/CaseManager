# CaseManager

This repository is meant to represent an in-progress function project representing development of a case management system. Clients are allowed to create, view, and modify cases and upload associated photos. Background tasks capture estimates for case handling and generate thumbnails for attached images.

The project is intentionally in an unfinished state with suboptimal decisions made in its development. **Please do not use this as a basis for design of any real projects.**

## Setup

The project is ready to use when cloned. A `local.settings.json` file is included that uses development storage ([Azurite](https://learn.microsoft.com/azure/storage/common/storage-use-azurite)). This is the recommended path for working with the project, but you can also change the configuration to target a storage account in Azure if desired. Note that if you are using Visual Studio Code, using Azurite requires the extension to be installed, and then it needs to be started using the command pallette (press `F1` and choose `Azurite: Start`).

Debug builds include a function named `AddTestData` which populates the storage with some basic test data for exercising some of the project's behaviors. The function exposes an API that responds to a GET with no parameters, so you can run the project and then navigate to the associated endpoint in a browser to get things set up.

[Azure Storage Explorer](https://azure.microsoft.com/products/storage/storage-explorer/) is useful for cleaning up the test data or for running / confirming additional tests, but it is not required.
