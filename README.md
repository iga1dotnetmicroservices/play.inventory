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

dotnet pack src/Play.Inventory.Contracts --configuration Release -p:PackageVersion=$version -p:RepositoryUrl=https://github.com/$ownergit st/play.inventory.git -o ../packages

dotnet nuget push ../packages/Play.Inventory.Contracts.$version.nupkg --api-key $gh_pat --source "github"
```
