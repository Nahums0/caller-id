using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;


namespace RestAPI.Controllers
{
    public class IdsController : ApiController
    {
        private void DeleteDatabase()
        {
            using (Model1 db = new Model1())
            {
                db.Database.Delete();
                db.SaveChanges();
            }
        }

        public List<Person> Get()//Return All Aviable Records
        {
            List<Person> l = new List<Person>();
            using (Model1 db = new Model1())
            {
                foreach (var item in db.Ids)
                {
                    l.Add(item);
                }
            }
            return l;
        }

        public Person Get([FromUri] string id) //Return A Specific Person By Number
        {
            Person RequestedPerson;
            using (Model1 db=new Model1())
            {
                RequestedPerson = db.Ids.SingleOrDefault(x => x.Number == id);
            }
            if (RequestedPerson == null)
                return null;
            return RequestedPerson;
        }

        [HttpPost]
        public void Post([FromBody] string data)
        {
            //Contacts that are sent here belong to the one who sent them

            //Recives data from body which is written in a (Number_Name1,Name2,..._Number1,Number2,...)
            //Splits the data into a variable -> sdata ---> 
            /*
             * s[0] -> contains the number 
             * s[1] -> contains the names array
             * s[2] -> contains the numbers array
             */ 
             //The names and number are suppose to act like tuple -> Name1:Number1

            var sdata = data.Split('_');
            string Number = sdata[0];

            //Extracting the names
            List<string> Names = new List<string>();
            if (sdata[1] != "None")
            {
                var a = sdata[1].Split(',');
                Names.AddRange(a);
            }
            
            //Extracting the numbers
            List<string> Numbers = new List<string>();
            if (sdata[2] != "None")
            {
                var b = sdata[2].Split(',');
                Numbers.AddRange(b);
            }

            using (Model1 db = new Model1())
            {
                //Checks if the posted number exist in records
                var query = db.Ids.Where(x => x.Number == Number);
                if (query.Count() == 0) // If it doesn't, it will create and add one
                {
                    Person p = new Person { Number = Number };
                    db.Ids.Add(p);
                }

                //TODO: OPTIMIZE
                for (int i = 0; i < Numbers.Count; i++)
                {
                    var numbers_i = Numbers[i];
                    var q = db.Ids.SingleOrDefault(x => x.Number == numbers_i);

                    if (q != null)     //The number exists in records
                    {
                                       //The number already exist so we just need to add the nicknames
                        q.Nicknames.Add(new Tuple<string, string>(Names[i], Number));

                    }
                    else               //The number does not exists and needs to be created
                    {
                        var TempPerson = new Person();
                        TempPerson.Number = numbers_i;
                        TempPerson.Nicknames.Add(new Tuple<string, string>(Names[i], Number));
                        db.Ids.Add(TempPerson);
                    }
                }
                db.SaveChanges();
            }
        }

        [HttpDelete]
        public void Delete(string id)
        {
            using (Model1 db = new Model1())
            {
                db.Ids.Remove(db.Ids.Where(x => x.Number == id).First());
                db.SaveChanges();
            }
        }



    }
}
