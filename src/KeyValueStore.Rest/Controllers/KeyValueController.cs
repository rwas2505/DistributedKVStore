using Microsoft.AspNetCore.Mvc;
using KeyValueStore.Core.Interfaces;
using KeyValueStore.Rest.Models;

namespace KeyValueStore.Rest.Controllers
{
    [ApiController]
    [Route("store")]
    public class StoreController : ControllerBase
    {
        private readonly IKeyValueStore _store;

        public StoreController(IKeyValueStore store)
        {
            _store = store;
        }

        [HttpGet("{key}")]
        public IActionResult Get(string key)
        {
            var result = _store.Get(key);
            return Ok(result);
        }

        [HttpPut("{key}")]
        public IActionResult Put(string key, [FromBody] PutRequestDto request)
        {
            var result = _store.Put(key, request.Value);
            return Ok(result);
        }

        [HttpDelete("{key}")]
        public IActionResult Delete(string key)
        {
            var result = _store.Delete(key);
            return Ok(result);
        }
    }
}