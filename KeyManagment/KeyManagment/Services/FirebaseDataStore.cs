using Firebase.Database;
using Firebase.Database.Offline;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace KeyManagment.Services
{
    public class FirebaseDataStore<T> : IDataStore<T>
        where T : class
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

        public async Task<bool> AddItemAsync(T item)
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

        public async Task<bool> UpdateItemAsync(T tobechangeditem, T updatedata)
        {
            try
            {
                var toUpdatePerson = (await _query
                    .OnceAsync<T>()).Where(a => a.Object.Equals(tobechangeditem)).FirstOrDefault();

                await _query
                    .Child(toUpdatePerson.Key)
                  .PutAsync(updatedata);
            }
            catch(Exception ex)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> CreatAPPAccount(T apppw)
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

        public async Task<T> GetItemAsync(string id)
        {
            try
            {
                return await _query
                    .Child(id)
                    .OnceSingleAsync<T>();
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<T> GetItemNode()
        {
            try
            {
                return await _query.Child("Notes").OnceSingleAsync<T>();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<T> GetAppPW()
        {
            try
            {
                var returvalue = await _query.Child("ThisApplication").OnceSingleAsync<T>();
                return returvalue;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false)
        {
            try
            {
                var firebaseObjects = (await _query
                    .OnceAsync<T>()).Where(x => (x.Key != "ThisApplication"));

                return (firebaseObjects
                    .Select(x => x.Object));
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
