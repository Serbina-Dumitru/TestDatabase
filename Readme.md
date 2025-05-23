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
dotnet restore
dotnet run
```

## Buld docker image and run it
```
git clone https://github.com/Serbina-Dumitru/TestDatabase
cd TestDatabase
docker build -t databasetest .
docker run --rm -it --network host databasetest
```
