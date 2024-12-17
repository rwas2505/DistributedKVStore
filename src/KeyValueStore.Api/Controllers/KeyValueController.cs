using Microsoft.AspNetCore.Mvc;
using KeyValueStore.Core.Interfaces;

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
            var value = _store.Get(key);
            return value is not null ? Ok(value) : NotFound();
        }

        [HttpPut("{key}")]
        public IActionResult Put(string key, [FromBody] string value)
        {
            _store.Put(key, value);
            return Ok();
        }

        [HttpDelete("{key}")]
        public IActionResult Delete(string key)
        {
            _store.Delete(key);
            return NoContent();
        }
    }
}