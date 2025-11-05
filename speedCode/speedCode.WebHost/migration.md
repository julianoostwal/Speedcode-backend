dotnet ef migrations add InitialProfileDbMigration \
--project modules/speedCode.Profile \
--startup-project speedCode.WebHost \
--context ProfileDb \
--output-dir Migrations

dotnet ef migrations add InitialRoomsDbMigration \
--project modules/speedCode.Room \
--startup-project speedCode.WebHost \
--context RoomsDb \
--output-dir Migrations

dotnet ef migrations add InitialexerciseDbMigration \
--project modules/speedCode.exercise \
--startup-project speedCode.WebHost \
--context exerciseDb \
--output-dir Migrations

dotnet ef database update \
--project modules/speedCode.Profile \
--startup-project speedCode.WebHost \
--context ProfileDb

dotnet ef database update \
--project modules/speedCode.Room \
--startup-project speedCode.WebHost \
--context RoomsDb


dotnet ef database update \
--project modules/speedCode.exercise \
--startup-project speedCode.WebHost \
--context exerciseDb

