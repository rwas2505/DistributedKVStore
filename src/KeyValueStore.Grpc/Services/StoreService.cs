using Grpc.Core;
using KeyValueStore.Core.Interfaces;

namespace KeyValueStore.Grpc.Services;

public class StoreService(ILogger<StoreService> logger, IKeyValueStore store) : Store.StoreBase
{
    private readonly ILogger<StoreService> _logger = logger;
    private readonly IKeyValueStore _store = store;

    public override Task<ValueResponse> Get(KeyRequest request, ServerCallContext context)
    {
        var response = new ValueResponse();

        // Todo: handle null request?
        string value = _store.Get(request.Key);

        if (value != null)
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
        // todo: handle null request?
        _store.Put(request.Key, request.Value);

        return Task.FromResult(new PutResponse { Message = "Value stored successfully" });
    }

    public override Task<DeleteResponse> Delete(DeleteRequest request, ServerCallContext context)
    {
        // todo: handle null request?
        var deleted = _store.Delete(request.Key);

        return Task.FromResult(new DeleteResponse { Deleted = deleted });
    }
}