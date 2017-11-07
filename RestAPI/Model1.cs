namespace RestAPI
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Model1 : DbContext
    {
        // Your context has been configured to use a 'Model1' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'RestAPI.Model1' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'Model1' 
        // connection string in the application configuration file.
        public Model1()
            : base("name=Model1")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<Person> Ids { get; set; }
        public override string ToString()
        {
            string str = "[";
            foreach (var item in Ids)
            {
                str += item.ToString()+",";
            }
            return str+"]";
        }
    }

    public class Person
    {
        [Key]
        public int Id { get; set; }
        public string Number { get; set; }

        [NotMapped]
        public List<Tuple<string, string>> Nicknames { get; set; } = new List<Tuple<string, string>>();// Names-Numbers

        /// <summary> <see cref="Nicknames"/> for database persistence. </summary>
        [Obsolete("Only for Persistence by EntityFramework")]
        public string MetaDataJsonForDb
        {
            get
            {
                return Nicknames == null || !Nicknames.Any()
                           ? null
                           : JsonConvert.SerializeObject(Nicknames);
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    Nicknames.Clear();
                else
                    Nicknames = JsonConvert.DeserializeObject<List<Tuple<string, string>>>(value);
            }
        }
        public string ListFormat()
        {
            string numbers = "";
            foreach (var item in Nicknames)
            {
                numbers += "{\"m_Item1\":\""+item.Item1+ "\",\"m_Item2\":\"" + item.Item2 + "\"},";
            }
            numbers = numbers.Remove(numbers.Length - 1);
            string str = "{\"Id\":"+Id+",\"Number\":\""+Number+"\",\"Nicknames\":["+numbers+"]}";
            return str;
        }
        public override string ToString()
        {
            //{"id":"1", "number":"1","contacts":[["Omer","0527472550"],["Nahum","2548132205"],["Yaron","555"],]}
            string rstr = "{\"id\":\""+Id+"\",\"number\":\""+Number+"\",\"contacts\":[";
            //["Omer","0527472550"],
            if (Nicknames.Count == 0)
            {
                rstr += "]";
            }
            else
            {
                foreach (var item in Nicknames)
                {
                    rstr += String.Format("[\"{0}\",\"{1}\"],", item.Item1, item.Item2);
                }
            }
            rstr=rstr.Remove(rstr.Length - 1);
            rstr += "]}";
            return rstr;

        }

    }
}