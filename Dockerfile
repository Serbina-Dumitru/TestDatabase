FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
#WORKDIR /src

RUN cat /etc/resolv.conf
#Restore
COPY ["TestDatabase.csproj","TestDatabase/"]
RUN dotnet restore "TestDatabase/TestDatabase.csproj"

#Build
COPY . "TestDatabase/"
WORKDIR /TestDatabase
RUN dotnet build "TestDatabase.csproj" -c Release -o /app/build

#Publish
FROM build AS publish
RUN dotnet publish "TestDatabase.csproj" -c Release -o /app/publish

#RUN
FROM mcr.microsoft.com/dotnet/aspnet:9.0
ENV ASPNETCORE_HTTP_PORTS=5171
EXPOSE 5171
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet","TestDatabase.dll"]
