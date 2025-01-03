# DistributedKVStore

### GRPC:

#### Run the app
 ```bash
dotnet run --project .\src\KeyValueStore.Grpc
 ``` 
 
#### Use the api (git bash)
##### grpcurl docs: https://github.com/fullstorydev/grpcurl

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