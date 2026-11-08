FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY backend.csproj ./
RUN dotnet restore backend.csproj

COPY . ./

RUN dotnet publish ResumeAnalysis.Worker/ResumeAnalysis.Worker.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "ResumeAnalysis.Worker.dll"]
