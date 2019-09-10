dotnet clean ../RemindMe.sln
bash ./create_test_db.sh
dotnet test ../Log4Npg/Log4Npg.Tests/Log4Npg.Tests.csproj
bash ./teardown_test_db.sh