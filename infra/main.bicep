@description('Application name')
param applicationName string = 'domainscanner-${substring(uniqueString(resourceGroup().id), 0, 5)}'

@description('Location')
param location string = 'francecentral'

@allowed(['F1', 'B1', 'B2', 'B3'])
@description('App Service Plan SKU')
param sku string = 'B2'

@description('Repository URL')
param repositoryUrl string = 'https://github.com/arielcostas/domain-scanner.git'

@description('Repository branch')
param repositoryBranch string = 'main'

var linuxFxVersion = 'DOTNETCORE|8.0'

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
    isVirtualNetworkFilterEnabled: false
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

resource rgLock 'Microsoft.Authorization/locks@2016-09-01' = {
  name: 'Lock'
  properties: {
    level: 'CanNotDelete'
  }
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
