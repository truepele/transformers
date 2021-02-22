# transformers

## Run instructions
**App targets net5.0 framework**

Please configure storage connectivity, build and run the app, use Swagger UI.

### Storage configuration
Solution uses EF Core migration mechanism to create DB (including the stored proc).

2 variants of storage are supported (configurable via appsettings.json or appsettings.Development.json):
- `Classic`
- `InDocker`

Configuration key: `DataAccess:DataAccessType`

#### Classic
Just a usual connection to SQL server. Connection details configured via `DataAccess:SqlConnectionStringBuilder` (Matches properties of the .net `ConnectionStringBuilder` class)

#### InDocker (Requires Docker installed and running)
App will attempt to start a container of mssql server for linux, bind it to a host port and then will connect to the port for business.
`ContainerName`, `Image` and `HostPort` are configurable at `DataAccess:DockerSql` (see appsettings.Development.json for an example).

### Running from an IDE
Recommended launch profile is `Transformers.WebApi` (self-hosted), navigates to Swagger UI by default (http://localhost:5000/index.html).

### Running from command line
```
dotnet build
cd ./Transformers.WebApi/bin/Debug/net5.0/
dotnet ./Transformers.WebApi.dll
```
Navigate to http://localhost:5000 in a browser

## List of third party nugets used
#### Dapper - used to call the stored proc in a convenient way
#### Automapper - to map DTOs to entity and in reverse
#### FluentValidation - to validate DTOs
#### Ductus.FluentDocker - to start sql server container when `InDocker` storage option configured
#### Swashbuckle.AspNetCore - for swagger UI

### Used for tests only:
#### xunit
#### Scrutor - to scan assemblies and register services with the DI
#### Bogus - to generate fake data
#### NSubstitute - dependencies substitution
#### DeepEqual - 'deep equal' assertions
#### coverlet - code coverage

## Unit tests specifics
One of the test projects (`Transformers.WebApi.StorageDependent.Tests`) requires real storage connectivity configured at appsettings.Test.json

## Development
### Generating db migration
```
dotnet ef migrations add {MigrationName} --context TransformersDbContext --output-dir Migrations --startup-project ./Transformers.WebApi/Transformers.WebApi.csproj --project ./Transformers.DataAccess/Transformers.DataAccess.csproj
```
