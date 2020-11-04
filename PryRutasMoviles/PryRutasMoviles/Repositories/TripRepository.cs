using Firebase.Database;
using Firebase.Database.Query;
using PryRutasMoviles.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace PryRutasMoviles.Repositories
{
    public class TripRepository:IDisposable
    {
        private FirebaseClient firebase = new FirebaseClient("https://xamarinfirebase-c9db8.firebaseio.com/");

        /*
         * Status Trip
         * Posted
         * OnWay
         * Canceled
         * Finished
         */

        public async Task AddTrip(Trip trip)
        {
            await firebase
            .Child("Trip")
            .PostAsync(trip);
        }

        public async Task<List<Trip>> GetTripsByState(string state)
        {
            return (await firebase
               .Child("Trip")
               .OnceAsync<Trip>()).Select(item => new Trip
               {
                   Driver = item.Object.Driver,
                   MeetingTime = item.Object.MeetingTime,
                   Price = item.Object.Price,
                   SeatsAvailables = item.Object.SeatsAvailables,
                   State = item.Object.State,
                   TripId = item.Object.TripId,
                   TripRoute = item.Object.TripRoute,
                   Passengers = item.Object.Passengers
               }).ToList()
               .Where(a => a.State == state)
               .ToList();
        }

        public async Task<Trip> GetTrip(string idTrip)
        {
            var resultstrip = await firebase
                          .Child("Trip")
                          .OnceAsync<Trip>();
            return resultstrip.Where(a => a.Object.TripId == idTrip).FirstOrDefault().Object;
        }

        public async Task<bool> AddPassenger(User newPassenger, string idTrip)
        {
            var toUpdateTrip = (await firebase
              .Child("Trip")
              .OnceAsync<Trip>()).Where(a => a.Object.TripId == idTrip).FirstOrDefault();

            if(toUpdateTrip.Object.Passengers.Count <= toUpdateTrip.Object.SeatsAvailables )
            {
                toUpdateTrip.Object.Passengers.Add(newPassenger);
                await firebase
                 .Child("Trip")
                 .Child(toUpdateTrip.Key)
                 .PutAsync(toUpdateTrip);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<Trip> PassengerHasATrip(User actualPassenger)
        {
            actualPassenger.State = true;
            var tripPassengerActual = (await firebase
              .Child("Trip")
              .OnceAsync<Trip>())
              .Where(a => a.Object.Passengers.Contains(actualPassenger))
              .Where(a => a.Object.State == "Posted" || a.Object.State == "OnWay").FirstOrDefault();
            return tripPassengerActual.Object;
        }


        public async Task<List<Trip>> UsersTrip(User actualPassenger)
        {
            var tripPassengerActual = (await firebase
              .Child("Trip")
              .OnceAsync<Trip>()).Select(item => new Trip
              {
                  Driver = item.Object.Driver,
                  MeetingTime = item.Object.MeetingTime,
                  Price = item.Object.Price,
                  SeatsAvailables = item.Object.SeatsAvailables,
                  State = item.Object.State,
                  TripId = item.Object.TripId,
                  TripRoute = item.Object.TripRoute,
                  Passengers = item.Object.Passengers
              }).ToList()
              .Where(a => a.Passengers.Contains(actualPassenger))
              .Where(a => a.State.ToString() == "false")
              .Where(a => a.State == "Finished").ToList();
            return tripPassengerActual;
        }

        public async Task RemovePassenger(User oldPassenger, string idTrip)
        {
            var toDeletePass = (await firebase
              .Child("Trip")
              .OnceAsync<Trip>()).Where(a => a.Object.TripId == idTrip).FirstOrDefault();

            toDeletePass.Object.Passengers.Remove(oldPassenger);
                await firebase
                 .Child("Trip")
                 .Child(toDeletePass.Key)
                 .PutAsync(toDeletePass);
        }

        public async Task RemoveSafePassenger(User oldPassenger, string idTrip)
        {
            oldPassenger.State = false;
            var toDeletePass = (await firebase
              .Child("Trip")
              .OnceAsync<Trip>()).Where(a => a.Object.TripId == idTrip).FirstOrDefault();

            await firebase
             .Child("Trip")
             .Child(toDeletePass.Key)
             .PutAsync(toDeletePass);
        }

        public async Task<int> SeatsOcuppateds(string idTrip)
        {
            var TripSeats = (await firebase
                .Child("Trip")
                .OnceAsync<Trip>()).Where(a => a.Object.TripId == idTrip).FirstOrDefault();

            return TripSeats.Object.Passengers.Count;
        }

        public async Task UpdateStateTrip(string state, string idTrip)
        {
            var toUpdateTrip = (await firebase
               .Child("Trip")
               .OnceAsync<Trip>()).Where(a => a.Object.TripId == idTrip).FirstOrDefault();

            toUpdateTrip.Object.State = state;

            await firebase
                .Child("Trip")
                .Child(toUpdateTrip.Key)
                .PutAsync(toUpdateTrip);
        }

        public async Task<string> GetStateTrip(string idTrip)
        {
            var toUpdateTrip = (await firebase
               .Child("Trip")
               .OnceAsync<Trip>()).Where(a => a.Object.TripId == idTrip).FirstOrDefault();

            return toUpdateTrip.Object.State;
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
