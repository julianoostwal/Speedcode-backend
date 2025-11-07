dotnet ef migrations add InitialProfileDbMigration \
--project modules/speedCode.Profile \
--startup-project speedCode.WebHost \
--context ProfileDb

dotnet ef migrations add InitialRoomsDbMigration \
--project modules/speedCode.Room \
--startup-project speedCode.WebHost \
--context RoomsDb

dotnet ef migrations add InitialExerciseDbMigration \
--project modules/speedCode.Exercise \
--startup-project speedCode.WebHost \
--context ExerciseDb

dotnet ef database update --project modules/speedCode.Profile --startup-project speedCode.WebHost --context ProfileDb
dotnet ef database update --project modules/speedCode.Room --startup-project speedCode.WebHost --context RoomsDb
dotnet ef database update --project modules/speedCode.Exercise --startup-project speedCode.WebHost --context ExerciseDb

psql -U zuohuangcai -d postgres -f ./add-cross-module-foreign-keys.sql
