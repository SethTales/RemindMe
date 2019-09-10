docker pull postgres
docker run --name test-db-instance -d -p 5432:5432 -e POSTGRES_USER=root -e POSTGRES_PASSWORD=password postgres
cd ../Log4Npg/Log4Npg.Logging/Data/
dotnet ef database update --context LoggingDatabaseContext --project ../Log4Npg.Logging.csproj