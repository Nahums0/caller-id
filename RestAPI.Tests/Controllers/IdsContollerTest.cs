using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestAPI.Tests.Controllers
{
    [TestClass]
    public class IdsContollerTest
    {
        [TestMethod]
        public void CheckForDuplicates()
        {
            List<string> Numbers = new List<string>();
            using (Model1 db = new Model1())
            {
                foreach (var item in db.Ids)
                {
                    Numbers.Add(item.Number);
                }
            }
           Assert.IsFalse(Numbers.Count != Numbers.Distinct().Count());
            


        }

    }
}
