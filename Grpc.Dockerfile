# Use the .NET SDK image to build and run the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the KeyValueStore.Grpc.csproj and restore dependencies
COPY ["KeyValueStore.Grpc/KeyValueStore.Grpc.csproj", "KeyValueStore.Grpc/"]
COPY ["KeyValueStore.Core/KeyValueStore.Core.csproj", "KeyValueStore.Core/"]

RUN dotnet restore "KeyValueStore.Grpc/KeyValueStore.Grpc.csproj"

# Copy all source files and build the app
COPY . .

RUN dotnet publish "KeyValueStore.Grpc/KeyValueStore.Grpc.csproj" -c Release -o /publish

# Use a smaller runtime image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /publish . 

# Copy the self signed cert into the container. This is required for http2 over https communication
# TODO: dynamically generate this with a script each time since it will eventually expire
COPY ["resources/cert.p12", "/app/cert.p12"]

# Set the environment variables for the certificate path and password
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/app/cert.p12
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=""

# Expose ports
EXPOSE 5001

ENTRYPOINT ["dotnet", "KeyValueStore.Grpc.dll"]
