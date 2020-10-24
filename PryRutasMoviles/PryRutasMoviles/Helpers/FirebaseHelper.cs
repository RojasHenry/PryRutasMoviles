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
        FirebaseClient firebase = new FirebaseClient("https://xamarinfirebase-c9db8.firebaseio.com/");

        public async Task<List<Driver>> GetAllPersons()
        {

            return (await firebase
              .Child("Driver")
              .OnceAsync<Driver>()).Select(item => new Driver
              {
                  FirstName = item.Object.FirstName,
                  DriverId = item.Object.DriverId
              }).ToList();
        }

        public async Task AddPerson(string personId, string name)
        {
            //Prueba Git desde visual studio RAMA 1 
            await firebase
            .Child("Driver")
            .PostAsync(new Driver()
            {
                DriverId = personId,
                FirstName = name,
                Vehicles = new List<Vehicle>() {
                    new Vehicle{ Brand = "Toyota",Color="Red",Registration="PBC-124",State = true,Type="Auto",Year="2015"},
                    new Vehicle{ Brand = "KIA",Color="Black",Registration="PVC-664",State=false,Type="Auto",Year="2019"}
            }
            });
        }

        public async Task OfferTrip()
        {
            var driver = new Driver
            {
                DriverId = "1725389512",
                FirstName = "Sebastian",
                LastName = "Reza",
                Vehicles = new List<Vehicle> { new Vehicle { Brand = "KIA", Color = "Black", Registration = "PVC-664", State = true, Type = "Auto", Year = "2019" } }
            };
            var driverVehicle = driver.Vehicles.SingleOrDefault(v => v.State);


            await firebase
              .Child("OffertedTrip")
              .PostAsync(new OffertedTrip()
              {
                  OffertedTripDriver = new OffertedTripDriver
                  {
                      DriverId = driver.DriverId,
                      FirstName = driver.FirstName,
                      LastName = driver.LastName
                  },
                  OffertedTripVehicle = new OffertedTripVehicle
                  {
                      Brand = driverVehicle.Brand,
                      Color = driverVehicle.Color,
                      Registration = driverVehicle.Registration,
                      Type = driverVehicle.Type,
                      Year = driverVehicle.Year
                  },

                  MeetingPoint = "Av Jose de Villalengua y Av Gaspar de escalona 232",
                  MeetingTime = "20:00",
                  Price = 2.30M,
                  Route = "Panaderia Arenas - Terminal Quitumbe",
                  SeatsAvailables = 2,
                  State = "Disponible"
              }); ;

            //.PostAsync(new Driver()
            //{
            //    DriverId = personId,
            //    FirstName = name,
            //    Vehicles = new List<Vehicle>() {
            //      new Vehicle{ Brand = "Toyota",Color="Red",Registration="PBC-124",State="Activo",Type="Auto",Year="2015"},
            //      new Vehicle{ Brand = "KIA",Color="Black",Registration="PVC-664",State="Activo",Type="Auto",Year="2019"}
            //}
            //});
        }

        public async Task<Driver> GetPerson(string personId)
        {
            var allPersons = await GetAllPersons();
            await firebase
              .Child("Driver")
              .OnceAsync<Driver>();
            return allPersons.Where(a => a.DriverId == personId).FirstOrDefault();
        }

        public async Task UpdatePerson(string personId, string name)
        {
            var toUpdatePerson = (await firebase
              .Child("Driver")
              .OnceAsync<Driver>()).Where(a => a.Object.DriverId == personId).FirstOrDefault();

            await firebase
              .Child("Driver")
              .Child(toUpdatePerson.Key)
              .PutAsync(new Driver() { DriverId = personId, FirstName = name });
        }

        public async Task DeletePerson(string personId)
        {
            var toDeletePerson = (await firebase
              .Child("Driver")
              .OnceAsync<Driver>()).Where(a => a.Object.DriverId == personId).FirstOrDefault();
            await firebase.Child("Driver").Child(toDeletePerson.Key).DeleteAsync();

        }
    }
}
