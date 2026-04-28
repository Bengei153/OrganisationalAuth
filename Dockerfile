# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY OrganisationalAuth.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5008
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DATABASE_CONNECTION_STRING=Host=ep-empty-union-anuxnzdz-pooler.c-6.us-east-1.aws.neon.tech; Database=neondb; Username=neondb_owner; Password=npg_acBR3m4kGehT; SSL Mode=VerifyFull; Channel Binding=Require;

EXPOSE 5008

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrganisationalAuth.dll"]
