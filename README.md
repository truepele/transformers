# transformers

## Generating db migration
```
dotnet ef migrations add {MigrationName} --context TransformersDbContext --output-dir Migrations --startup-project ./Transformers.WebApi/Transformers.WebApi.csproj --project ./Transformers.DataAccess/Transformers.DataAccess.csproj
```
