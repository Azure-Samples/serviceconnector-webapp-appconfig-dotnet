# Connect Azure WebApp to App Configuration using system managed indentity
This is a sample to demo how to connect a .net core application hosting in Azure WebApp to Azure App Configuration. Authentication between Azure WebApp and App Configuration is system Managed Identity.

- [Prerequisite](#prerequisite)
- [How to run](#how-to-run)
- [How it works](#how-it-works)
- [Test (optional)](#test-optional)
- [Cleanup](#cleanup)
- [Useful links](#useful-links)

## Prerequisite
- An Azure account with an active subscription. You need to have Subscription Contributor role. [Create an account for free](https://azure.microsoft.com/en-in/free/).
- Azure CLI. You can [install it locally](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest) or use it in [CloudShell](https://shell.azure.com).
- Git.
- [**Optional**] [Visual Studio](https://visualstudio.microsoft.com/downloads/) or [VSCode](https://code.visualstudio.com/download).


## How to run
1. Setup Azure resources
   1. Create Azure WebApp.
   ```bash
   # login to Azure CLI, skip if running in Cloudshell.
   az login
   # switch to a subscription where you have Subscription Contributor role.
   az account set -s <myTestSubsId>
   # create resource group
   az group create -n <myResourceGroupName> -l eastus
   # create appservice plan
   az appservice plan create -g <myResourceGroupName> -n <myPlanName> --is-linux --sku B1
   # create webapp
   az webapp create -g <myResourceGroupName> -n <myWebAppName> --runtime "DOTNETCORE|3.1" --plan <myPlanName>
   ```
   1. Create Azure App Configuration Store, import test configuration file [./sampleconfigs.json](./sampleconfigs.json).
      If using Cloudshell, [upload sampleconfigs.json](https://docs.microsoft.com/en-us/azure/cloud-shell/persisting-shell-storage#upload-files) before run the command.
   ```bash
   # create app configuration store
   az appconfig create -g <myResourceGroupName> -n <myAppConfigStoreName> --sku Free -l eastus
   # import test config into app configuration store.
   az appconfig kv import -n <myAppConfigStoreName> --source file --format json --path ./sampleconfigs.json --separator : --yes
   ```
   

1. Create connection between the WebApp and App Configuration by auth type system assigned Managed Identity via Service Connector.
   ```bash
   # connect webapp and appconfigure
   az webapp connection create appconfig -g <myResourceGroupName> -n <myWebAppName> --app-config <myAppConfigStoreName> --tg <myResourceGroupName> --connection <myConnectioName> --system-identity
   ```
   `system-identity` is authentication type, other supported authentication types: user assigned Managed Identity; connection string; service principal, please refer to [more samples](https://github.com/yungezz/serviceconnector-webapp-appconfig-dotnet/).   

1. Build and Deploy App to Azure. Use below steps or any approach you're familiar with to build and publish to Azure WebApp.
   1. Clone the sample repo
      ```bash
      git clone https://github.com/Azure-Samples/serviceconnector-webapp-appconfig-dotnet.git
      ```
   1. cd to the folder `serviceconnector-webapp-appconfig-dotnet\system-managed-identity\Microsoft.Azure.ServiceConnector.Sample`, do build
      ```bash
      cd serviceconnector-webapp-appconfig-dotnet\system-managed-identity\Microsoft.Azure.ServiceConnector.Sample
      dotnet publish .\Microsoft.Azure.ServiceConnector.Sample.csproj -c Release
      ```
   1. Deploy to the Azure Web App.
   Recommend to use Visual Studio or VSCode.
      - Visual Studio. Open the sample solution in Visual Studio, right click on the project name, click `Publish`, follow the wizard to publish to Azure. 
        More detail at [instruction](https://docs.microsoft.com/en-us/azure/app-service/tutorial-dotnetcore-sqldb-app?toc=%2Faspnet%2Fcore%2Ftoc.json&bc=%2Faspnet%2Fcore%2Fbreadcrumb%2Ftoc.json&view=aspnetcore-6.0&tabs=azure-portal%2Cvisualstudio-deploy%2Cdeploy-instructions-azcli%2Cazure-portal-logs%2Cazure-portal-resources#4---deploy-to-the-app-service).
      - VSCode. Install extension [Azure App Service extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azureappservice). 
        Open the sample folder with VSCode, right click on the project, click `Deploy to WebApp`, follow the wizard to publish to Azure. 
        More detail at [instruction](https://docs.microsoft.com/en-us/azure/app-service/tutorial-dotnetcore-sqldb-app?toc=%2Faspnet%2Fcore%2Ftoc.json&bc=%2Faspnet%2Fcore%2Fbreadcrumb%2Ftoc.json&view=aspnetcore-6.0&tabs=azure-portal%2Cvisualstudio-deploy%2Cdeploy-instructions-azcli%2Cazure-portal-logs%2Cazure-portal-resources#4---deploy-to-the-app-service).
      - Azure CLI.
        ```bash
        # set deplyment project in Azure WebApp to this project in sample repo.
        az webapp config appsettings set -g <myResourceGroupName> -n <myWebAppName> --settings PROJECT=system-managed-identity/Microsoft.Azure.ServiceConnector.Sample/Microsoft.Azure.ServiceConnector.Sample.csproj
        # config deployment source to local git repo
        az webapp deployment source config-local-git -g <myResourceGroupName> -n <myWebAppName>
        # get publish credential
        az webapp deployment list-publishing-credentials -g <myResourceGroupName> -n <myWebAppName>  --query "{Username:publishingUserName, Password:publishingPassword}"
        git remote add azure https://<myWebAppName>.scm.azurewebsites.net/<myWebAppName>.git
        # push local main to remote master branch, the command will prompt for username and password, which are in output of above list-publishing-credentials command
        git push azure main:master
        ```
1. Validate the connection is working. Nagivate to your WebApp `https://<myWebAppName>.azurewebsites.net/` from browser, you can see the site is up, 
   displaying `Hello. Your Azure WebApp is connected to App Configuration by ServiceConnector now`.

## How it works
Service Connector service do the connection configuration for you. 
- set WebApp Appsetting `AZURE_APPCONFIGURATION_ENDPOINT`, 
so the application could read it to get app configuration endpoint in [code](https://github.com/yungezz/serviceconnector-webapp-appconfig-dotnet/blob/main/system-managed-identity/Microsoft.Azure.ServiceConnector.Sample/Program.cs#L37);
- enable WebApp system Managed Identity and grant App Configuration Data Reader role to it, so the application could be authenticated to the App Configuration in [code](https://github.com/yungezz/serviceconnector-webapp-appconfig-dotnet/blob/main/system-managed-identity/Microsoft.Azure.ServiceConnector.Sample/Program.cs#L43), by using `DefaultAzureCredential` from [Azure.Identity](https://azuresdkdocs.blob.core.windows.net/$web/dotnet/Azure.Identity/1.0.0/api/index.html).
- Learn more about the detail from [Service Connector Internal](https://docs.microsoft.com/en-us/azure/service-connector/concept-service-connector-internals).

## Test (optional)
1. Update value of key `SampleApplication:Settings:Messages` in the App Configuration Store.
   ```bash
   az appconfig kv set -n <myAppConfigStoreName> --key SampleApplication:Settings:Messages --value hello --yes
   ```

1. Navigate to your Azure WebApp `https://<myWebAppName>.azurewebsites.net/`, refresh the page, you'll see the message is updated to `hello`.

## Cleanup
After you're done, delete Azure resources created.
```
az group delete -n <myResourceGroupName> --yes
```

## Useful links
- [More Service Connector samples](https://github.com/azure-samples?q=serviceconnector&type=all&language=&sort=) to connect Azure WebApp, Azure Spring Cloud to other Azure services.
- Learn more about [Service Connector](https://aka.ms/scdoc).

## Troubleshooting