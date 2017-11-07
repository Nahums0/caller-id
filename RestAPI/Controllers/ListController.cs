using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RestAPI.Controllers
{
    public class ListController : ApiController
    {
        public string Get([FromUri] string data)//Return All Aviable Records
        {
            string str = "{\"List\":[";
            var QueryNumbers = data.Split(',');
            using (Model1 db = new Model1())
            {
                foreach (var item in db.Ids)
                {
                    if (QueryNumbers.Contains(item.Number))
                    {
                        str += item.ListFormat()+",";
                    }
                }
            }
            str= str.Remove(str.Length - 2);
            str += "}]}";
            return str;
        }

    }
}
