# Proof of concept

## Idea
A server aplication written in [C#](https://learn.microsoft.com/en-us/dotnet/csharp/) that is meant to receive and send data with [Rest API](https://en.wikipedia.org/wiki/Overview_of_RESTful_API_Description_Languages).
It has a database created with [Entity Framework](https://learn.microsoft.com/en-us/aspnet/entity-framework) and [Fluent API](https://learn.microsoft.com/en-us/ef/ef6/modeling/code-first/fluent/types-and-properties) that is meant to hold information for a messenger app.

## Database schema
![database schema](./Images/Database_Schema.png)

## Compile and run
```
git clone https://github.com/Serbina-Dumitru/TestDatabase
cd TestDatabase
dotnet dev-certs https --trust
dotnet restore
dotnet run
```

## Buld docker image and run it
With [docker](https://www.docker.com/) we would be able to host and containerise the aplication, the ip of the container would be the ip of the host.
```
git clone https://github.com/Serbina-Dumitru/TestDatabase
cd TestDatabase
dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p "Admin123"
dotnet dev-certs https --trust

docker build -t databasetest .
docker run --rm -it -p 5171:5171 \
  -e ASPNETCORE_HTTPS_PORTS=5171 \
  -e ASPNETCORE_URLS="https://[::1]:5171" \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx \
  -e ASPNETCORE_Kestrel__Certificates__Default__Password="Admin123\!" \
  -v ${HOME}/.microsoft/usersecrets/:/home/app/.microsoft/usersecrets \
  -v ${HOME}/.aspnet/https/:/https/ \
  databasetest:latest
```
