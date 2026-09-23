using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Candiate_search_assesment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidateSearch : ControllerBase
    {
        // GET: api/<CandidateSearch>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<CandidateSearch>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<CandidateSearch>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<CandidateSearch>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CandidateSearch>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
