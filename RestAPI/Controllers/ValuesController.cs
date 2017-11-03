using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RestAPI.Controllers
{
   
    public class ValuesController : ApiController
    {
        List<string> Values = new List<string>();
        public ValuesController()
        {
            Values.Add("This is value number 0");
            Values.Add("This is value number 1");
            Values.Add("This is value number 2");
            Values.Add("This is value number 3");
            Values.Add("This is value number 4");
            Values.Add("This is value number 5");
            Values.Add("This is value number 6");
            Values.Add("This is value number 7");

        }
        // GET api/values
        public List<string> Get()
        {
            return Values;
        }

        // GET api/values/5
        public string Get(int id)
        {
            return Values[id];
        }

        // POST api/values
        public string Post([FromBody]string value)
        {
            Values.Add(value);
            return value;
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
            Values.RemoveAt(id);
        }
    }
}
