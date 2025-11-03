# Stage 1: Build the app
# Use the official .NET 8 SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY *.csproj .
RUN dotnet restore

# Copy the rest of the source code and build
COPY . .
RUN dotnet publish "MaillotStore.csproj" -c Release -o /app/publish

# Stage 2: Create the final runtime image
# Use the smaller ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Set the environment variable for the port
ENV ASPNETCORE_URLS=http://+:10000

# Entry point for the container
ENTRYPOINT ["dotnet", "MaillotStore.dll"]