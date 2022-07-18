---
page_type: sample
languages:
- csharp
products:
- service-connector
description: "Sample projects to connect Azure WebApp to App Configuration via Service Connector"
urlFragment: "serviceconnector-webapp-appconfig-dotnet"
---

# Connect Azure WebApp to App Configuration via ServiceConnector
<!-- 
Guidelines on README format: https://review.docs.microsoft.com/help/onboard/admin/samples/concepts/readme-template?branch=master

Guidance on onboarding samples to docs.microsoft.com/samples: https://review.docs.microsoft.com/help/onboard/admin/samples/process/onboarding?branch=master

Taxonomies for products and languages: https://review.docs.microsoft.com/new-hope/information-architecture/metadata/taxonomies?branch=master
-->

## Overview
[Service Connector](https://docs.microsoft.com/en-us/azure/service-connector/) is an Azure-managed service that helps developers easily connect compute service to target backing services. This service configures the network settings and connection information (for example, generating environment variables) between compute service and target backing service. Developers just use preferred SDK or library that consumes the connection information to do data plane operations against target backing service.

Service Connector supports Azure WebApp, Azure Spring Cloud as source of connection, and over [10+ Azure services](https://docs.microsoft.com/en-us/azure/service-connector/overview#what-services-are-supported-in-service-connector) as target, such as PostgreSQL, MySQL, Storage, Keyvault, ServiceBus, SignalR etc.

This repository contains sample projects to connect Azure WebApp to App Configuration via ServiceConnector. 

## Sample list
Below table shows the list of samples in this repository. All authentication types between Azure WebApp and App Configuration are covered.
| Sample |  Description |
|-------------|-----------------------------|
|[system-managed-identity](./system-managed-identity) | Sample of connecting Azure WebApp hosting DotNet application to AppConfig with **system Managed Identity**|
|[user-assigned-managed-identity](./user-assigned-managed-identity)| Sample of connecting Azure WebApp hosting DotNet application to AppConfig with **user assigned Managed Identity**|
|[service-principal](./service-principal)| Sample of connecting Azure WebApp hosting DotNet application to AppConfig with **service principal**|
|[connection-string](./connection-string)| Sample of connecting Azure WebApp hosting DotNet application to AppConfig with **connection string**|

## Useful References
- [More samples](https://github.com/azure-samples?q=serviceconnector&type=all&language=&sort=) to connect Azure services to service.
- Learn more about [Service Connector](https://aka.ms/scdoc).

## Code of Conduct

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.


## License

Copyright (c) Microsoft Corporation. All rights reserved.
Licensed under the [MIT](./LICENSE.md) license.