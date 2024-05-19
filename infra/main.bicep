@description('Application name')
param applicationName string = 'domainscanner-${uniqueString(resourceGroup().id)}'

@description('Location')
param location string = 'francecentral'

@allowed(['F1', 'B1', 'B2', 'B3'])
@description('App Service Plan SKU')
param sku string = 'F1'

@description('Repository URL')
param repositoryUrl string = 'https://github.com/arielcostas/domain-scanner.git'

@description('Repository branch')
param repositoryBranch string = 'main'

var cosmosAccountName = toLower(applicationName)
var websiteName = applicationName
var hostingPlanName = '${applicationName}-plan'

resource vnet 'Microsoft.Network/virtualNetworks@2023-11-01' = {
  name: '${applicationName}-vnet'
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        '172.16.0.0/16'
      ]
    }
    subnets: [
      {
        name: 'web'
        properties: {
          addressPrefix: '172.16.0.0/20'
          serviceEndpoints: [
            {
              service: 'Microsoft.AzureCosmosDB'
            }
          ]
        }
      }
    ]
  }
}

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
    virtualNetworkRules: [
      {
        id: vnet.properties.subnets[0].id
        ignoreMissingVNetServiceEndpoint: false
      }
    ]
  }
}

resource hostingPlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: hostingPlanName
  location: location
  sku: {
    name: sku
    capacity: 1
  }
}

resource website 'Microsoft.Web/sites@2023-12-01' = {
  name: websiteName
  location: location
  properties: {
    serverFarmId: hostingPlan.id
    siteConfig: {
      metadata: [
        {
          name: 'CURRENT_STACK'
          value: 'DOTNET|8.0'
        }
      ]
      appSettings: [
        {
          name: 'Cosmos:Endpoint'
          value: cosmosAccount.properties.documentEndpoint
        }
        {
          name: 'Cosmos:Key'
          value: cosmosAccount.listKeys().primaryMasterKey
        }
      ]
    }

    virtualNetworkSubnetId: vnet.properties.subnets[0].id

    clientAffinityEnabled: false
    httpsOnly: true
  }
}

resource sourceControl 'Microsoft.Web/sites/sourcecontrols@2023-12-01' = {
  parent: website
  name: 'web'
  properties: {
    repoUrl: repositoryUrl
    branch: repositoryBranch
    isGitHubAction: true
    deploymentRollbackEnabled: false
    gitHubActionConfiguration: {
      generateWorkflowFile: true
      isLinux: true
      codeConfiguration: {
        runtimeStack: 'dotnet'
        runtimeVersion: '8.0'
      }
    }
  }
}
