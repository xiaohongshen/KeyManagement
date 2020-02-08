using Firebase.Database;
using Firebase.Database.Offline;
using Firebase.Database.Query;
using KeyManagment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KeyManagment.Services
{
    public class FirebaseDataStore
    {
        private const string BaseUrl = "https://keymanagement-7be4d.firebaseio.com";

        private readonly FirebaseClient _query;

        public FirebaseDataStore()
        {
            _query = new FirebaseClient(BaseUrl);
        }

        public async Task<Item> GetItemNode()
        {
            try
            {
                Console.WriteLine("tmp {0}", "i get hier");
                var tmplist = await GetItems();
                await _query.Child("Notes").OnceAsync<Item>();
                Console.WriteLine("tmp {0}", "it is suck");
                return tmplist.Where(a => a.NameofApplication == "irgendwas").FirstOrDefault();
                //return tmplist.FindIndex;
                //return new Item { NameofApplication = "dd", PW = "bb", Date = "f" };

            }
            catch (Exception ex)
            {
                return new Item { NameofApplication = "gg",PW="bb",Date="cc"};
            }
        }

        public async Task<List<Item>> GetItems()
        {
            try
            {
                return (await _query.Child("Notes").OnceAsync<Item>()).Select(item => new Item
                {
                    NameofApplication = item.Object.NameofApplication,
                    PW = item.Object.PW,
                    Date = item.Object.Date
                }).ToList();
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}

