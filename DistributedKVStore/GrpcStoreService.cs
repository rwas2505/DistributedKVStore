using Grpc.Core;
using System.Net;
using DistributedKVStore;

public class GrpcStoreService : Store.StoreBase
{
    private static readonly Dictionary<string, string> _store = new();

    public override Task<ValueResponse> Get(KeyRequest request, ServerCallContext context)
    {
        var response = new ValueResponse();
        if (_store.TryGetValue(request.Key, out var value))
        {
            response.Value = value;
            response.Found = true;
        }
        else
        {
            response.Found = false;
        }
        return Task.FromResult(response);
    }

    public override Task<PutResponse> Put(PutRequest request, ServerCallContext context)
    {
        _store[request.Key] = request.Value;
        return Task.FromResult(new PutResponse { Message = "Value stored successfully" });
    }
}
