using Android.Content;
using Newtonsoft.Json;
using RestSharp;
using Xamarin.Contacts;
using System.Linq;
using System.Threading.Tasks;

namespace Called_Id
{
    public static class RestQueries
    {
        ///<summary>
        ///<para>Posts a new user to the server</para>
        ///<para>Uses context the get user's contacts and a user entered phone number</para>
        ///</summary>
        public static async Task<string> PostUser(Context context, string Phonenumber)
        {
            var client = new RestClient(AppConsts.RestBaseUrl + "/api/Ids/");
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;

            var book = new AddressBook(context);
            if (!await book.RequestPermission())
            {
                return "-1";
            }
            string query = Phonenumber + "_";
            string numbers_query = "_";
            string names_query = "";

            foreach (var item in book)
            {
                if (item.DisplayName != null && item.Phones.Count() != 0)
                {
                    foreach (var phone in item.Phones)
                    {
                        if (phone.Number != null && phone.Number != "")
                        {
                            names_query += item.DisplayName + ",";
                            numbers_query += phone.Number + ",";
                            break;
                        }
                    }
                }
            }
            names_query.Remove(names_query.Length - 2);
            numbers_query.Remove(numbers_query.Length - 2);
            query += names_query + numbers_query;
            query = query.Substring(0, query.Length - 2);
            request.AddBody(query);

            IRestResponse response = client.Execute(request);
            var content = response.Content;
            return content;
        }
        /// <summary>
        /// <para>Recives a phone number and uthenticates the user</para>
        /// </summary>
        /// <param name="phoneNumber">User's phone number</param>
        /// <returns>Return a tuple -> Logged will determine if user logged in successfully and Data will contaion the query result</returns>
        public static (bool Logged, string Data) Authenticate(string phoneNumber)
        {
            try
            {
                var client = new RestClient(AppConsts.RestBaseUrl);
                var request = new RestRequest("/api/Ids/" + phoneNumber, Method.GET);
                IRestResponse response = client.Execute(request);
                var content = response.Content;
                if (content == "null")
                    return (false, null);
                JsonClass.RootObject Result = JsonConvert.DeserializeObject<JsonClass.RootObject>(content);
                if (Result.Number == phoneNumber)
                    return (true, content);
            }
            catch
            {
                return (false, null);
            }
            return (false, null);
        }
        /// <summary>
        /// <para>Delete the user which phone number is the same as the parameter</para>
        /// </summary>
        /// <param name="phonenumber"></param>
        /// <returns></returns>
        public static bool DeleteUser(string phonenumber)
        {
            try
            {
                var client = new RestClient(AppConsts.RestBaseUrl + "api/ids" + phonenumber);
                var request = new RestRequest(Method.DELETE);
                client.Execute(request);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
