using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using SignalRChat.ViewModels;
using System.Net.Http.Headers;
using System.Net;

namespace SignalRChat.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ComentarPublicacion : ContentPage
	{
        string sMateria { get; set; }
        string sToken { get; set; }
        string sMat { get; set; }
        string sUsuario { get; set; }
        string sPublic { get; set; }
        string sTitle { get; set; }

        public ComentarPublicacion (string userToken, string sPublicacion, string sMatName, string sTitulo)
		{
            InitializeComponent();
            this.sToken = userToken; 
            this.sPublic = sPublicacion;
            this.sMat = sMatName;
            this.sTitle = sTitulo;
        }

        private void B_Volver(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new PublicacionDetail(this.sToken, this.sPublic, this.sMat));
        }

        public async void B_Comentar(object sender, EventArgs e)
        {
            HttpClient cliente = new HttpClient();
            string sTokenUser = "https://cursivia.herokuapp.com/api_v1/Token/" + this.sToken + "/";
            var responseU = await cliente.GetStringAsync(sTokenUser);
            var sUser = JsonConvert.DeserializeObject<TokenUser>(responseU);
            this.sUsuario = sUser.user;

            Comentario comentario = new Comentario()
            {
                //id = 7,
                comentario = entComentario.Text,
                usuario = sUser.user,
                publicacion = this.sPublic,
                estado_comentario = "p"
            };

            var json = JsonConvert.SerializeObject(comentario);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpClient client2 = new HttpClient();
            var result = await client2.PostAsync("https://cursivia.herokuapp.com/api_v1/comentarios/", content);

            if (result.StatusCode == HttpStatusCode.Created)
            {
                Application.Current.MainPage = new NavigationPage(new PublicacionDetail(this.sToken, this.sPublic, this.sMat));
            }
        }

        protected override void OnAppearing()
        {
            Title = this.sTitle;
        }
    }

    

}
