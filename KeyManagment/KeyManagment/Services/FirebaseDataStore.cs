using Firebase.Database;
using Firebase.Database.Offline;
using Firebase.Database.Query;
using KeyManagment.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace KeyManagment.Services
{
    public class FirebaseDataStore
    {
        private const string BaseUrl = "https://keymanagement-7be4d.firebaseio.com";

        private readonly ChildQuery _query;

        public FirebaseDataStore(/*FirebaseAuthService authService, */ string path)
        {
            /* try to tailor out the options
            FirebaseOptions options = new FirebaseOptions()
            {
                AuthTokenAsyncFactory = async () => await authService.GetFirebaseAuthToken()
            };
            */

            _query = new FirebaseClient(BaseUrl/*, options*/).Child(path);
        }

        public async Task<bool> AddItemAsync(Item item)
        {
            try
            {
                await _query
                    .PostAsync(item);
            }
            catch(Exception ex)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateItemAsync(EntryData updatedata)
        {
            try
            {
                var toUpdatePerson = (await _query
                    .OnceAsync<Item>()).Where(a => (a.Key == updatedata.EntryId)).FirstOrDefault();

                await _query
                    .Child(updatedata.EntryId)
                  .PutAsync(updatedata.EntryItem);
            }
            catch(Exception ex)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> CreatAPPAccount(Item apppw)
        {
            try
            {
                await _query
                    .Child("ThisApplication")
                  .PutAsync(apppw);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            try
            {
                await _query
                    .Child(id)
                    .DeleteAsync();
            }
            catch(Exception ex)
            {
                return false;
            }

            return true;
        }

        public async Task<Item> GetItemAsync(string id)
        {
            try
            {
                return await _query
                    .Child(id)
                    .OnceSingleAsync<Item>();
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<Item> GetAppPW()
        {
            try
            {
                return (await _query.Child("ThisApplication").OnceSingleAsync<Item>());
            }
            catch (Exception ex)
            {
                Debug.WriteLine("clement hier? {0}", ex);
                return null;
            }
        }

        public async Task<IEnumerable<EntryData>> GetItemsAsync(bool forceRefresh = false)
        {
            try
            {
                var firebaseObjects = (await _query
                    .OnceAsync<Item>()).Where(x => (x.Key != "ThisApplication"));

                return (firebaseObjects
                    .Select(x => new EntryData { EntryId = x.Key, EntryItem = x.Object}));
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
