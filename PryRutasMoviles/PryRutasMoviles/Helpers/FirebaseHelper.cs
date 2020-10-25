using System.Collections.Generic;
using PryRutasMoviles.Models;
using Firebase.Database;
using Firebase.Database.Query;
using System.Linq;
using System.Threading.Tasks;

namespace PryRutasMoviles.Helpers
{
    public class FirebaseHelper
    {
        private readonly FirebaseClient firebase = new FirebaseClient("https://xamarinfirebase-c9db8.firebaseio.com/");

        public async Task<List<User>> GetAllPersons()
        {

            return (await firebase
              .Child("User")
              .OnceAsync<User>()).Select(item => new User
              {
                  FirstName = item.Object.FirstName,
                  DriverId = item.Object.DriverId
              }).ToList();
        }

        public async Task AddPerson(string personId, string name)
        {
            //Prueba Git desde visual studio RAMA 1 
            await firebase
            .Child("User")
            .PostAsync(new User());
        }

        //public async Task OfferTrip()
        //{
        //    var driver = new User
        //    {
        //        DriverId = "1725389512",
        //        FirstName = "Sebastian",
        //        LastName = "Reza",
        //        Vehicles = new List<Vehicle> { new Vehicle { Brand = "KIA", Color = "Black", Registration = "PVC-664", State = true, Type = "Auto", Year = "2019" } }
        //    };
        //    var driverVehicle = driver.Vehicles.SingleOrDefault(v => v.State);


        //    await firebase
        //      .Child("OffertedTrip")
        //      .PostAsync(new OffertedTrip()
        //      {
        //          OffertedTripDriver = new OffertedTripDriver
        //          {
        //              DriverId = driver.DriverId,
        //              FirstName = driver.FirstName,
        //              LastName = driver.LastName
        //          },
        //          OffertedTripVehicle = new OffertedTripVehicle
        //          {
        //              Brand = driverVehicle.Brand,
        //              Color = driverVehicle.Color,
        //              Registration = driverVehicle.Registration,
        //              Type = driverVehicle.Type,
        //              Year = driverVehicle.Year
        //          },

        //          MeetingPoint = "Av Jose de Villalengua y Av Gaspar de escalona 232",
        //          MeetingTime = "20:00",
        //          Price = 2.30M,
        //          Route = "Panaderia Arenas - Terminal Quitumbe",
        //          SeatsAvailables = 2,
        //          State = "Disponible"
        //      }); ;

        //    //.PostAsync(new User()
        //    //{
        //    //    DriverId = personId,
        //    //    FirstName = name,
        //    //    Vehicles = new List<Vehicle>() {
        //    //      new Vehicle{ Brand = "Toyota",Color="Red",Registration="PBC-124",State="Activo",Type="Auto",Year="2015"},
        //    //      new Vehicle{ Brand = "KIA",Color="Black",Registration="PVC-664",State="Activo",Type="Auto",Year="2019"}
        //    //}
        //    //});
        //}

        public async Task<User> GetPerson(string personId)
        {
            var allPersons = await GetAllPersons();
            await firebase
              .Child("User")
              .OnceAsync<User>();
            return allPersons.Where(a => a.DriverId == personId).FirstOrDefault();
        }

        public async Task UpdatePerson(string personId, string name)
        {
            var toUpdatePerson = (await firebase
              .Child("User")
              .OnceAsync<User>()).Where(a => a.Object.DriverId == personId).FirstOrDefault();

            await firebase
              .Child("User")
              .Child(toUpdatePerson.Key)
              .PutAsync(new User() { DriverId = personId, FirstName = name });
        }

        public async Task DeletePerson(string personId)
        {
            var toDeletePerson = (await firebase
              .Child("User")
              .OnceAsync<User>()).Where(a => a.Object.DriverId == personId).FirstOrDefault();
            await firebase.Child("User").Child(toDeletePerson.Key).DeleteAsync();

        }
    }
}
