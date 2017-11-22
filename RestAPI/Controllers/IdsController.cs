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
            string ClientNumber = sdata[0];
            ClientNumber = ClientNumber.Replace("+972", "0");
            ClientNumber = ClientNumber.Replace("+", "");

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
                foreach (var item in b)
                {
                    var Formated = item;
                    Formated = Formated.Replace("+972", "0");
                    Formated = Formated.Replace("-", "");
                    Formated = Formated.Replace(" ", "");
                    Numbers.Add(Formated);
                }
            }
            //- +972
            using (Model1 db = new Model1())
            {
                //Checks if the posted number exist in records
                var query = db.Ids.Where(x => x.Number == ClientNumber);
                if (query.Count() == 0) // If it doesn't, it will create and add one
                {
                    Person p = new Person { Number = ClientNumber };
                    db.Ids.Add(p);
                }

                //TODO: OPTIMIZE
                for (int i = 0; i < Numbers.Count; i++)
                {
                    var number = Numbers[i];
                    var q = db.Ids.SingleOrDefault(x => x.Number == number);

                    if (q != null)     //The number exists in records
                    {
                                       //The number already exist so we just need to add the nicknames
                        for (int k = 0; k < q.Nicknames.Count; k++)
                        {
                            if (q.Nicknames[k].Item2==ClientNumber)
                            {
                                q.Nicknames.RemoveAt(k);
                                break;
                            }
                        }
                        q.Nicknames.Add(new Tuple<string, string>(Names[i], ClientNumber));
                    }
                    else               //The number does not exists and needs to be created
                    {
                        var TempPerson = new Person();
                        TempPerson.Number = number;
                        TempPerson.Nicknames.Add(new Tuple<string, string>(Names[i], ClientNumber));
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
                if (id == "all")
                {
                    db.Database.Delete();
                }
                else
                {
                    db.Ids.Remove(db.Ids.Where(x => x.Number == id).First());
                    db.SaveChanges();
                }
            }
        }



    }
}
