# DistributedKVStore

This app supports both grpc (http2) and rest (http1) services.
<br>
#### Run the tests
```bash
dotnet test tests\KeyValueStore.Rest.Tests\KeyValueStore.Rest.Tests.csproj

dotnet test tests\KeyValueStore.Core.Tests\KeyValueStore.Core.Tests.csproj

dotnet test tests/KeyValueStore.Grpc.Tests/KeyValueStore.Grpc.Tests.csproj
```
#### Run the app
In order to start both services quickly with docker run the below from the root directory to tear down any stale resources and then build and run the services fresh
```bash
docker compose down --rmi all
docker compose up -d
```

To start either service on its own with dotnet, run one of the below from the root directory
 ```bash
dotnet run --project .\src\KeyValueStore.Grpc
 ``` 
 ```bash
dotnet run --project .\src\KeyValueStore.Rest
 ``` 

#### Call the services

##### <ins>REST<ins>
 

##### PUT
```bash
PUT method http://localhost:5000/store/testKey
body:
{
    "value": "someValue2"
}
```

##### GET
```bash
http://localhost:5000/store/testKey
```

##### DELETE
```bash
DELETE method http://localhost:5000/store/testKey
```

##### <ins>GRPC<ins>
 
##### grpcurl docs: https://github.com/fullstorydev/grpcurl

(below grpcurl commands executed in gitbash)

##### PUT
```bash
grpcurl --insecure --proto ./src/KeyValueStore.Grpc/Protos/Store.proto -d '{"key":"some key", "value":"some value"}' localhost:5001 Store.Put
```

##### GET
```bash
grpcurl --insecure --proto ./src/KeyValueStore.Grpc/Protos/Store.proto -d '{"key":"some key"}' localhost:5001 Store.Get
```

##### DELETE
```bash
grpcurl --insecure --proto ./src/KeyValueStore.Grpc/Protos/Store.proto -d '{"key":"some key"}' localhost:5001 Store.Delete
```