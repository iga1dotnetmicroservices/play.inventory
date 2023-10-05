# Play.Inventory

Play Economy Inventory microservice

## Create and publish package

MacOS

```powershell
version='1.0.2'
owner='iga1dotnetmicroservices'
gh_pat='[PAT HERE]'

dotnet pack src/Play.Inventory.Contracts --configuration Release -p:PackageVersion=$version -p:RepositoryUrl=https://github.com/$owner/play.inventory.git -o ../packages

dotnet nuget push ../packages/Play.Inventory.Contracts.$version.nupkg --api-key $gh_pat --source "github"
```

Windows Powershel

```powershell
$version='1.0.2'
$owner='iga1dotnetmicroservices'
$gh_pat='[PAT HERE]'

dotnet pack src/Play.Inventory.Contracts --configuration Release -p:PackageVersion=$version -p:RepositoryUrl=https://github.com/$owner/play.inventory.git -o ../packages

dotnet nuget push ../packages/Play.Inventory.Contracts.$version.nupkg --api-key $gh_pat --source "github"
```

## Build the docker image

MacOS

```shell
export GH_OWNER='iga1dotnetmicroservices'
export GH_PAT='[PAT HERE]'
docker build --secret id=GH_OWNER --secret id=GH_PAT -t play.inventory:$version .
```

Windows Shell

```powershell
$env:GH_OWNER='iga1dotnetmicroservices'
$env:GH_PAT='[PAT HERE]'
docker build --secret id=GH_OWNER --secret id=GH_PAT -t play.inventory:$version .
```

## Run the docker image

MacOS

```shell
authority='[AUTHORITY]'
docker run -it --rm -p 5004:5004 --name inventory -e MongoDbSettings__Host=mongo -e RabbitMQSettings__Host=rabbitmq -e ServiceSettings__Authority=$authority --network playinfra_default play.inventory:$version
```

Windows Shell

```powershell
authority='[AUTHORITY]'
docker run -it --rm -p 5004:5004 --name inventory -e MongoDbSettings__Host=mongo -e RabbitMQSettings__Host=rabbitmq -e ServiceSettings__Authority=$authority --network playinfra_default play.inventory:$version
```
