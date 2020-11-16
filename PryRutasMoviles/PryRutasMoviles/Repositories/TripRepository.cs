using Firebase.Database;
using Firebase.Database.Query;
using PryRutasMoviles.Models;
using System;
using System.Collections.Generic;
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

        public async Task<List<Trip>> GetAllTrips() 
        {
            return (await firebase
               .Child("Trip")
               .OnceAsync<Trip>()).Select(item => new Trip
               {
                   Driver = item.Object.Driver,
                   MeetingDate = item.Object.MeetingDate,                   
                   MeetingTime = item.Object.MeetingTime,
                   Price = item.Object.Price,
                   SeatsAvailables = item.Object.SeatsAvailables,
                   State = item.Object.State,
                   TripId = item.Object.TripId,
                   TripRoute = item.Object.TripRoute,
                   Passengers = item.Object.Passengers
               }).ToList();
        }

        public async Task<List<Trip>> GetTripsOffered()
        {
            var result = await GetAllTrips();
            
            if (result == null)
                return null;

            return result.Where(t => (t.SeatsAvailables>0) 
                 && (t.State.Equals("Posted"))
                 && (Convert.ToDateTime(t.FullMeetingDate) >= DateTime.Now))
                .ToList();
        }

        public async Task<Trip> GetDriverCurrentTrip(User driver)
        {
            var result = await GetAllTrips();

            if (result == null)
                return null;

            return result.Where(d => 
            (d.Driver.Email.Equals(driver.Email))
             && ((d.State.Equals("Posted") 
                && (Convert.ToDateTime(d.FullMeetingDate) >= DateTime.Now))
                || (d.State.Equals("OnWay")))
            ).ToList()
            .FirstOrDefault();            
        }

        public async Task<Trip> GetTripById(string tripId)
        {
            var result = await GetAllTrips();
            if (result == null)
                return null;
            return result.Where(a => a.TripId.Equals(tripId)).FirstOrDefault();
        }

        public async Task AddPassengerOnATrip(User passenger, string tripId)
        {
            var toUpdateTrip = (await firebase
               .Child("Trip")
               .OnceAsync<Trip>())
               .Where(a => a.Object.TripId.Equals(tripId))
               .FirstOrDefault();

            if (toUpdateTrip.Object.Passengers == null)
            {
                toUpdateTrip.Object.Passengers = new List<User>() { passenger };
                toUpdateTrip.Object.SeatsAvailables--;
            }
            else 
            {
                toUpdateTrip.Object.Passengers.Add(passenger);
                toUpdateTrip.Object.SeatsAvailables--;
            }               
                            
            await firebase
                .Child("Trip")
                .Child(toUpdateTrip.Key)
                .PutAsync(new Trip() 
                { 
                    Driver = toUpdateTrip.Object.Driver,
                    MeetingDate = toUpdateTrip.Object.MeetingDate,
                    MeetingTime = toUpdateTrip.Object.MeetingTime,
                    Price = toUpdateTrip.Object.Price,
                    State = toUpdateTrip.Object.State,
                    TripId = toUpdateTrip.Object.TripId,
                    TripRoute = toUpdateTrip.Object.TripRoute,
                    Passengers = toUpdateTrip.Object.Passengers,
                    SeatsAvailables = toUpdateTrip.Object.SeatsAvailables,
                });            
        }

        public async Task RemovePassenger(User passenger, string tripId)
        {

            var toUpdateTrip = (await firebase
               .Child("Trip")
               .OnceAsync<Trip>())
               .Where(a => a.Object.TripId.Equals(tripId))
               .FirstOrDefault();

            toUpdateTrip.Object.Passengers.RemoveAll(p => p.Email.Equals(passenger.Email));
            toUpdateTrip.Object.SeatsAvailables++;

            await firebase
                .Child("Trip")
                .Child(toUpdateTrip.Key)
                .PutAsync(new Trip()
                {
                    Driver = toUpdateTrip.Object.Driver,
                    MeetingDate = toUpdateTrip.Object.MeetingDate,
                    MeetingTime = toUpdateTrip.Object.MeetingTime,
                    Price = toUpdateTrip.Object.Price,
                    State = toUpdateTrip.Object.State,
                    TripId = toUpdateTrip.Object.TripId,
                    TripRoute = toUpdateTrip.Object.TripRoute,
                    Passengers = toUpdateTrip.Object.Passengers,
                    SeatsAvailables = toUpdateTrip.Object.SeatsAvailables,
                });
        }

        public async Task<Trip> GetPassengerCurrentTrip(User currentPassenger)
        {
            var result = await GetAllTrips();
            if (result == null)
                return null;

            var TripsWithPassengers =
                result.Where(p => 
                (p.Passengers != null)
                && ((p.State.Equals("Posted")
                    && (Convert.ToDateTime(p.FullMeetingDate) >= DateTime.Now))
                    || (p.State.Equals("OnWay")))
                ).ToList();            

            if (TripsWithPassengers.Count > 0)
            {
                return TripsWithPassengers
                   .Where(t => t.Passengers.Any(p => (p.Email.Equals(currentPassenger.Email))&& p.State))
                   .FirstOrDefault();
            }
            else
                return null;            
        }

        public async Task<int> GetSeatsAvailableOnATrip(string tripId)
        {
            var selectedTrip = await GetTripById(tripId);
            return selectedTrip.SeatsAvailables;
        }

        public async Task UpdateStateTrip(string state, string tripId)
        {
            var toUpdateTrip = (await firebase
               .Child("Trip")
               .OnceAsync<Trip>())
               .Where(a => a.Object.TripId.Equals(tripId))
               .FirstOrDefault();

            toUpdateTrip.Object.State = state;

            await firebase
                .Child("Trip")
                .Child(toUpdateTrip.Key)
                .PutAsync(new Trip()
                {
                    Driver = toUpdateTrip.Object.Driver,
                    MeetingDate = toUpdateTrip.Object.MeetingDate,
                    MeetingTime = toUpdateTrip.Object.MeetingTime,
                    Price = toUpdateTrip.Object.Price,
                    State = toUpdateTrip.Object.State,
                    TripId = toUpdateTrip.Object.TripId,
                    TripRoute = toUpdateTrip.Object.TripRoute,
                    Passengers = toUpdateTrip.Object.Passengers,
                    SeatsAvailables = toUpdateTrip.Object.SeatsAvailables,
                });
        }

        public async Task CancelTrip(string tripId) 
        {
            var toUpdateTrip = (await firebase
               .Child("Trip")
               .OnceAsync<Trip>())
               .Where(a => a.Object.TripId.Equals(tripId))
               .FirstOrDefault();

            if (toUpdateTrip.Object.Passengers !=null)
                toUpdateTrip.Object.Passengers.Clear();

            toUpdateTrip.Object.State = "Canceled";

            await firebase
                .Child("Trip")
                .Child(toUpdateTrip.Key)
                .PutAsync(new Trip()
                {
                    Driver = toUpdateTrip.Object.Driver,
                    MeetingDate = toUpdateTrip.Object.MeetingDate,
                    MeetingTime = toUpdateTrip.Object.MeetingTime,
                    Price = toUpdateTrip.Object.Price,
                    State = toUpdateTrip.Object.State,
                    TripId = toUpdateTrip.Object.TripId,
                    TripRoute = toUpdateTrip.Object.TripRoute,
                    Passengers = toUpdateTrip.Object.Passengers,
                    SeatsAvailables = toUpdateTrip.Object.SeatsAvailables,
                });
        }

        public async Task<string> GetStatusTrip(string tripId) 
        {
            var tripSelected = (await firebase
               .Child("Trip")
               .OnceAsync<Trip>())
               .Where(a => a.Object.TripId.Equals(tripId))
               .FirstOrDefault();

            return tripSelected.Object.State.ToString();
        }
        
        public async Task FinishTrip(string tripId)
        {
            var toUpdateTrip = (await firebase
               .Child("Trip")
               .OnceAsync<Trip>())
               .Where(a => a.Object.TripId.Equals(tripId))
               .FirstOrDefault();
            
            toUpdateTrip.Object.Passengers.ForEach(p => p.State = false);
            toUpdateTrip.Object.State = "Finished";
            
            await firebase
                .Child("Trip")
                .Child(toUpdateTrip.Key)
                .PutAsync(new Trip()
                {
                    Driver = toUpdateTrip.Object.Driver,
                    MeetingDate = toUpdateTrip.Object.MeetingDate,                    
                    MeetingTime = toUpdateTrip.Object.MeetingTime,
                    Price = toUpdateTrip.Object.Price,
                    State = toUpdateTrip.Object.State,
                    TripId = toUpdateTrip.Object.TripId,
                    TripRoute = toUpdateTrip.Object.TripRoute,
                    Passengers = toUpdateTrip.Object.Passengers,
                    SeatsAvailables = toUpdateTrip.Object.SeatsAvailables,
                });
        }

        public async Task FinishPassengerTrip(User passenger, string tripId)
        {
            var selectedTrip = (await firebase
               .Child("Trip")
               .OnceAsync<Trip>())
               .Where(a => a.Object.TripId.Equals(tripId))
               .FirstOrDefault();

            selectedTrip.Object.Passengers
                .Where(p => p.Email.Equals(passenger.Email))
                .ToList()
                .ForEach(ps => ps.State = false);

            await firebase
                .Child("Trip")
                .Child(selectedTrip.Key)
                .PutAsync(new Trip()
                {
                    Driver = selectedTrip.Object.Driver,
                    MeetingDate = selectedTrip.Object.MeetingDate,
                    MeetingTime = selectedTrip.Object.MeetingTime,
                    Price = selectedTrip.Object.Price,
                    State = selectedTrip.Object.State,
                    TripId = selectedTrip.Object.TripId,
                    TripRoute = selectedTrip.Object.TripRoute,
                    Passengers = selectedTrip.Object.Passengers,
                    SeatsAvailables = selectedTrip.Object.SeatsAvailables,
                });
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
