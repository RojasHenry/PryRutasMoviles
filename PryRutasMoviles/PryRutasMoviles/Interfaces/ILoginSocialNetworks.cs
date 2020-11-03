using System;
using System.Threading.Tasks;
using PryRutasMoviles.Models;

namespace PryRutasMoviles.Interfaces
{
    public interface ILoginSocialNetworks
    {
        string getCurrentUser();

        Task<User> LoginGoogle();

        Task<bool> LoginWithFirebaseFB(string token);

        Task<bool> CreateNewUserFirebase(string email, string password);

        Task<bool> LoginWithFirebaseCredentials(string email, string password);

        void Logout();

    }
}
