using Firebase.Database;
using Firebase.Database.Query;
using PryRutasMoviles.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PryRutasMoviles.Repositories
{
    public class TripRepository:IDisposable
    {
        private FirebaseClient firebase = new FirebaseClient("https://xamarinfirebase-c9db8.firebaseio.com/");

        public async Task AddTrip(Trip trip)
        {
            await firebase
            .Child("Trip")
            .PostAsync(trip);
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
