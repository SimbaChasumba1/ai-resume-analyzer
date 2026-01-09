# Use the SDK to build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the project file and restore dependencies
COPY backend.csproj ./
RUN dotnet restore

# Copy the rest of the backend source
COPY . ./

# Build and publish the backend
RUN dotnet publish -c Release -o out

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Expose port
EXPOSE 5000
ENTRYPOINT ["dotnet", "backend.dll"] 