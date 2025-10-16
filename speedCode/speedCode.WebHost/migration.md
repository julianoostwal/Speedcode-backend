dotnet ef migrations add InitialExciseDbMigration \
--project modules/speedCode.Excise \
--startup-project speedCode.WebHost \
--context ExciseDb \
--output-dir Migrations

dotnet ef migrations add InitialProfileDbMigration \
--project modules/speedCode.Profile \
--startup-project speedCode.WebHost \
--context ProfileDb \
--output-dir Migrations

dotnet ef database update \
--project modules/speedCode.Excise \
--startup-project speedCode.WebHost \
--context ExciseDb

dotnet ef database update \
--project modules/speedCode.Profile \
--startup-project speedCode.WebHost \
--context ProfileDb


