using Firebase.Database;
using Firebase.Database.Query;
using PryRutasMoviles.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PryRutasMoviles.Repositories
{
    public class UserRepository:IDisposable
    {
        private FirebaseClient firebase = new FirebaseClient("https://xamarinfirebase-c9db8.firebaseio.com/");
        public async Task AddUser(User user)
        {
            await firebase
            .Child("User")
            .PostAsync(user);
        }

        public async Task<List<User>> GetAll()
        {
            return (await firebase
              .Child("User")
              .OnceAsync<User>()).Select(item => new User
              {
                  Email = item.Object.Email,
                  FirstName = item.Object.FirstName,
                  LastName = item.Object.LastName,
                  Address = item.Object.Address,
                  Password = item.Object.Password,
                  State = item.Object.State,
                  UserType = item.Object.UserType,
                  Vehicle = item.Object.Vehicle,
              }).ToList();
        }

        public async Task AddPerson(string personId, string name)
        {
            await firebase
            .Child("User")
            .PostAsync(new User());
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var result = await GetAll();
            if (result == null)
                return null;
            return result.Where(a => a.Email == email).FirstOrDefault();            
        }

        public async Task UpdatePerson(string personId, string name)
        {
            var toUpdatePerson = (await firebase
              .Child("User")
              .OnceAsync<User>()).Where(a => a.Object.Email == personId).FirstOrDefault();

            await firebase
              .Child("User")
              .Child(toUpdatePerson.Key)
              .PutAsync(new User() { Email = personId, FirstName = name });
        }

        public async Task DeletePerson(string personId)
        {
            var toDeletePerson = (await firebase
              .Child("User")
              .OnceAsync<User>()).Where(a => a.Object.Email == personId).FirstOrDefault();
            await firebase.Child("User").Child(toDeletePerson.Key).DeleteAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                firebase = null;
            }            
        }
    }
}
