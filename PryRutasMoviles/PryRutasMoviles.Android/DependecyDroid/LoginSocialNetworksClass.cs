using System;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
using Android.Gms.Extensions;
using Android.Gms.Tasks;
using Android.Widget;
using Firebase;
using Firebase.Auth;
using Java.Interop;
using Newtonsoft.Json;
using PryRutasMoviles.Droid.DependecyDroid;
using PryRutasMoviles.Interfaces;
using PryRutasMoviles.Models;
using Xamarin.Forms;
[assembly: Dependency(typeof(LoginSocialNetworksClass))]
namespace PryRutasMoviles.Droid.DependecyDroid
{
    public class LoginSocialNetworksClass : ILoginSocialNetworks
    {

        GoogleSignInOptions gso;
        GoogleApiClient client;

        public FirebaseAuth firebaseAuth;

        public async Task<User> LoginGoogle()
        {
            gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                .RequestIdToken("216415232228-vsuoihg6h9i13p7vl0kdosaj6jl84k67.apps.googleusercontent.com")
                .RequestEmail()
                .Build();

            client = new GoogleApiClient.Builder(MainActivity.ActivityContext)
                .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
                .Build();

            client.Connect();

            firebaseAuth = FirebaseAuth.Instance;

            GoogleSignInResult result = await OpenGoogleLogin();

            if (result.IsSuccess)
            {
                GoogleSignInAccount account = result.SignInAccount;
                var resultado = await LoginWithFirebase(account);

                if (resultado)
                {
                    User user = new User();
                    user.FirstName = account.GivenName;
                    user.LastName = account.FamilyName;
                    user.Email = firebaseAuth.CurrentUser.Email;
                    return user;
                }
                else
                {
                    return new User();
                }
            }
            else
            {
                return new User();
            }
        }

        public async Task<bool> LoginWithFirebase(GoogleSignInAccount account)
        {
            bool isLoggin = false;
            var credentials = GoogleAuthProvider.GetCredential(account.IdToken, null);


            await firebaseAuth.SignInWithCredentialAsync(credentials).ContinueWith(task =>
            {
                if(task.IsCompletedSuccessfully)
                {
                    isLoggin = true;
                }
                else
                {
                    isLoggin = false;
                }
            });

            return isLoggin;
        }

        public async Task<bool> LoginWithFirebaseFB(string token)
        {
            firebaseAuth = FirebaseAuth.Instance;

            bool isLoggin = false;
            var credentials = FacebookAuthProvider.GetCredential(token);

            await firebaseAuth.SignInWithCredentialAsync(credentials).ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    isLoggin = true;
                }
                else
                {
                    isLoggin = false;
                }
            });

            return isLoggin;
        }

        public Task<GoogleSignInResult> OpenGoogleLogin()
        {
            var activity = (MainActivity)MainActivity.ActivityContext;
            var listener = new ActivityResultListener(activity);

            var intent = Auth.GoogleSignInApi.GetSignInIntent(client);

            activity.StartActivityForResult(intent, 2);

            return listener.Task;
        }

        public async Task<bool> CreateNewUserFirebase(string email, string password)
        {
            firebaseAuth = FirebaseAuth.Instance;

            bool isCreated = false;

            await firebaseAuth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
                if (task.IsCompletedSuccessfully)
                {
                    isCreated = true;
                }
                else
                {
                    isCreated = false;
                }
            });

            return isCreated;
        }

        public async Task<bool> LoginWithFirebaseCredentials(string email, string password)
        {
            firebaseAuth = FirebaseAuth.Instance;

            bool isLoggin = false;

            await firebaseAuth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
                if (task.IsCompletedSuccessfully)
                {
                    isLoggin = true;
                }
                else
                {
                    isLoggin = false;
                }
            });

            return isLoggin;
        }

        public string getCurrentUser()
        {
            firebaseAuth = FirebaseAuth.Instance;

            return firebaseAuth.CurrentUser.Email;
        }

        public void Logout()
        {
            firebaseAuth = FirebaseAuth.Instance;

            firebaseAuth.SignOut();
        }
    }

    internal class ActivityResultListener
    {
        private TaskCompletionSource<GoogleSignInResult> Complete = new TaskCompletionSource<GoogleSignInResult>();
        public Task<GoogleSignInResult> Task { get { return this.Complete.Task; } }


        public ActivityResultListener(MainActivity activity)
        {
            // subscribe to activity results
            activity.ActivityResult += OnActivityResult;
        }

        private void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            var activity = (MainActivity)MainActivity.ActivityContext;
            activity.ActivityResult -= OnActivityResult;

            if(requestCode == 2)
            {
                GoogleSignInResult result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                this.Complete.TrySetResult(result);
            }
        }
    }
}
