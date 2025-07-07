# Use Microsoft's official .NET 8 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy everything into the container
COPY . .

# Restore dependencies (optional but recommended for caching efficiency)
RUN dotnet restore "Nonny-E-Learning-Platform/Nonny-E-Learning-Platform.csproj"

# Publish the app
RUN dotnet publish "Nonny-E-Learning-Platform/Nonny-E-Learning-Platform.csproj" -c Release -o /app/publish

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Run the app
ENTRYPOINT ["dotnet", "Nonny-E-Learning-Platform.dll"]