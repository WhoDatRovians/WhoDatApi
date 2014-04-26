using System.Collections.Generic;
using System.Web.Http;
using BrainBowTestWebAPI.Models;
using Newtonsoft.Json.Linq;

namespace BrainBowTestWebAPI.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new[] { "value1", "value2" };
        }

        
        // GET api/values/5
        public JObject Get(string typeofsearch, string filtercriteria)
        {
            if (typeofsearch == "movie")
            {
                return new RoviApi().GetMovieByKeyword(filtercriteria);
            }
            else if (typeofsearch == "celeb")
            {
                return new RoviApi().GetCelebByName(filtercriteria);
            }
            
            return new RoviApi().GetFilmographyByCelebId(filtercriteria);
           
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}