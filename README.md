# Play.Inventory

Play Economy Inventory microservice

## Create and publish package

MacOS

```powershell
version='1.0.4'
owner='iga1dotnetmicroservices'
gh_pat='[PAT HERE]'

dotnet pack src/Play.Inventory.Contracts --configuration Release -p:PackageVersion=$version -p:RepositoryUrl=https://github.com/$owner/play.inventory.git -o ../packages

dotnet nuget push ../packages/Play.Inventory.Contracts.$version.nupkg --api-key $gh_pat --source "github"
```

Windows

```powershell
$version='1.0.4'
$owner='iga1dotnetmicroservices'
$gh_pat='[PAT HERE]'

dotnet pack src/Play.Inventory.Contracts --configuration Release -p:PackageVersion=$version -p:RepositoryUrl=https://github.com/$owner/play.inventory.git -o ../packages

dotnet nuget push ../packages/Play.Inventory.Contracts.$version.nupkg --api-key $gh_pat --source "github"
```

## Build the docker image

MacOS

```shell
appname='iga1playeconomy'

export GH_OWNER='iga1dotnetmicroservices'
export GH_PAT='[PAT HERE]'
docker build --secret id=GH_OWNER --secret id=GH_PAT -t "$appname.azurecr.io/play.inventory:$version" .
```

Windows

```powershell
$appname='iga1playeconomy'

$env:GH_OWNER='iga1dotnetmicroservices'
$env:GH_PAT='[PAT HERE]'
docker build --secret id=GH_OWNER --secret id=GH_PAT -t "$appname.azurecr.io/play.inventory:$version" .
```

## Run the docker image

MacOS

```shell
authority='[AUTHORITY]'
cosmosDbConnString='[CONN STRING HERE]'
serviceBusConnString='[CONN STRING HERE]'

docker run -it --rm -p 5004:5004 --name inventory -e MongoDbSettings__ConnectionString=$cosmosDbConnString -e ServiceBusSettings__ConnectionString=$serviceBusConnString -e ServiceSettings__Authority=$authority -e ServiceSettings__MessageBroker="SERVICEBUS" play.inventory:$version
```

Windows

```powershell
$authority='[AUTHORITY]'
$cosmosDbConnString='[CONN STRING HERE]'
$serviceBusConnString='[CONN STRING HERE]'

docker run -it --rm -p 5004:5004 --name inventory -e MongoDbSettings__ConnectionString=$cosmosDbConnString -e ServiceBusSettings__ConnectionString=$serviceBusConnString -e ServiceSettings__Authority=$authority -e ServiceSettings__MessageBroker="SERVICEBUS" play.inventory:$version
```

## Publishing the Docker image


MacOS

```powershell
appname='iga1playeconomy'
az acr login --name $appname
docker push "$appname.azurecr.io/play.inventory:$version"
```

Windows

```powershell
$appname='iga1playeconomy'
az acr login --name $appname
docker push "$appname.azurecr.io/play.inventory:$version"
```

## Create the Kubernetes namespace

MacOS

```shell
namespace='inventory'
kubectl create namespace $namespace
```

Windows

```powershell
$namespace='inventory'
kubectl create namespace $namespace
```

## Create the pod managed identity

MacOS

```shell
az identity create --resource-group $appname --name $namespace

IDENTITY_RESOURCE_ID=$(az identity show -g $appname -n $namespace --query id -otsv)

az aks pod-identity add --resource-group $appname --cluster-name $appname --namespace $namespace --name $namespace --identity-resource-id $IDENTITY_RESOURCE_ID
```


Windows

```powershell
az identity create --resource-group $appname --name $namespace

$IDENTITY_RESOURCE_ID=az identity show -g $appname -n $namespace --query id -otsv

az aks pod-identity add --resource-group $appname --cluster-name $appname --namespace $namespace --name $namespace --identity-resource-id $IDENTITY_RESOURCE_ID
```

## Grant access to Key Vault secrets

MacOS 

```shell
IDENTITY_CLIENT_ID=$(az identity show -g $appname -n $namespace --query clientId -otsv)
az keyvault set-policy -n $appname --secret-permissions get list --spn $IDENTITY_CLIENT_ID
```

Windows 

```powershell
$IDENTITY_CLIENT_ID=az identity show -g $appname -n $namespace --query clientId -otsv
az keyvault set-policy -n $appname --secret-permissions get list --spn $IDENTITY_CLIENT_ID
```

## Install the Helm chart

MacOS

```shell
helmUser=00000000-0000-0000-0000-000000000000
helmPassword=$(az acr login --name $appname --expose-token --output tsv --query accessToken)

export HELM_EXPERIMENTAL_OCI=1
helm registry login "$appname.azurecr.io" --username $helmUser --password $helmPassword

chartVersion="0.1.0"
helm upgrade inventory-service oci://$appname.azurecr.io/helm/microservice --version $chartVersion -f ./helm/values.yaml -n $namespace --install
```

Windows

```powershell
$helmUser=00000000-0000-0000-0000-000000000000
$helmPassword=az acr login --name $appname --expose-token --output tsv --query accessToken

$env:HELM_EXPERIMENTAL_OCI=1
helm registry login "$appname.azurecr.io" --username $helmUser --password $helmPassword

chartVersion="0.1.0"
helm upgrade inventory-service oci://$appname.azurecr.io/helm/microservice --version $chartVersion -f ./helm/values.yaml -n $namespace --install
```