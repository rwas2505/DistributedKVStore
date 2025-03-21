# Use the .NET SDK image to build and run the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the KeyValueStore.Rest.csproj and restore dependencies
COPY ["KeyValueStore.Rest/KeyValueStore.Rest.csproj", "KeyValueStore.Rest/"]
COPY ["KeyValueStore.Core/KeyValueStore.Core.csproj", "KeyValueStore.Core/"]

RUN dotnet restore "KeyValueStore.Rest/KeyValueStore.Rest.csproj"

# Copy all source files and build the app
COPY . .

RUN dotnet publish "KeyValueStore.Rest/KeyValueStore.Rest.csproj" -c Release -o /publish

# Use a smaller runtime image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /publish . 

# Expose ports
EXPOSE 5001

ENTRYPOINT ["dotnet", "KeyValueStore.Rest.dll"]
