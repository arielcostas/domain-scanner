@description('Application name')
param applicationName string = 'domainscanner-${uniqueString(resourceGroup().id)}'

@description('Location')
param location string = 'francecentral'

@allowed(['F1', 'B1', 'B2', 'B3'])
@description('App Service Plan SKU')
param sku string = 'B1'

@description('Linux FX Version')
param linuxFxVersion string = 'DOTNETCORE|8.0'

@description('Repository URL')
param repositoryUrl string = 'https://github.com/arielcostas/domain-scanner.git'

@description('Repository branch')
param repositoryBranch string = 'main'

var cosmosAccountName = toLower(applicationName)
var websiteName = applicationName
var hostingPlanName = 'asp-${applicationName}'

resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2023-11-15' = {
  name: cosmosAccountName
  kind: 'GlobalDocumentDB'
  location: location
  properties: {
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    locations: [
      {
        locationName: location
        isZoneRedundant: false
        failoverPriority: 0
      }
    ]
    databaseAccountOfferType: 'Standard'
    enableFreeTier: false
    enableAnalyticalStorage: false
    isVirtualNetworkFilterEnabled: true
    capabilities: [
      { name: 'EnableServerless' }
    ]
    publicNetworkAccess: 'Enabled'
  }
}

resource hostingPlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: hostingPlanName
  location: location
  properties: {
    reserved: true
  }
  sku: {
    name: sku
  }
  kind: 'linux'
}

resource website 'Microsoft.Web/sites@2023-12-01' = {
  name: websiteName
  location: location
  properties: {
    serverFarmId: hostingPlan.id
    siteConfig: {
      linuxFxVersion: linuxFxVersion  
      // Copied from ARM template parameters
      metadata: [
        {
          name: 'currentStack'
          value: 'dotnet'
        }
      ]
      phpVersion: 'OFF'
      netFrameworkVersion: 'v8.0'
      // End of copied parameters
      appSettings: [
        {
          name: 'Cosmos__Endpoint'
          value: cosmosAccount.properties.documentEndpoint
        }
        {
          name: 'Cosmos__Key'
          value: cosmosAccount.listKeys().primaryMasterKey
        }
      ]
    }

    clientAffinityEnabled: false
    httpsOnly: true
  }
}

resource sourceControl 'Microsoft.Web/sites/sourcecontrols@2020-12-01' = {
  parent: website
  name: 'web'
  properties: {
    repoUrl: repositoryUrl
    branch: repositoryBranch
    isManualIntegration: true
  }
}

output websiteUrl string = website.properties.defaultHostName
output cosmosEndpoint string = cosmosAccount.properties.documentEndpoint
