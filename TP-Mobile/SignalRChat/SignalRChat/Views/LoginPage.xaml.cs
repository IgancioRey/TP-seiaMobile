using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using SignalRChat.Contracts;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SignalRChat.ViewModels;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using SignalRChat.Views;




namespace SignalRChat.Views
{
    public partial class LoginPage : ContentPage
    {
        /*
        private readonly IChatService _chatService = DependencyService.Get<IChatService>();
        */

        public LoginPage()
        {
            InitializeComponent();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Task.Run(RequestPermissions);
        }

        private async Task RequestPermissions()
        {
            await CrossPermissions.Current.RequestPermissionsAsync(Permission.Microphone);
            await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            User u = new User()
            {
                username = entUsername.Text,
                password = entPassword.Text,
            };

            var json = JsonConvert.SerializeObject(u);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpClient client = new HttpClient();

            var result = await client.PostAsync("https://cursivia.herokuapp.com/home/api-token-auth/", content);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                var content2 = await result.Content.ReadAsStringAsync();

                var resultoJson = JsonConvert.DeserializeObject<Token>(content2);
                string userToken = resultoJson.token;
                Application.Current.MainPage = new NavigationPage(new Materias(userToken));
                //await Navigation.PushAsync (new NavigationPage(new Materias(userToken)));
            }
            else
            {
                await DisplayAlert("Error", "Contraseña o Usuario incorrectos", "Volver a intentar");
            }
        }
    }
}