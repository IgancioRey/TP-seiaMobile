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

namespace SignalRChat.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PublicacionDetail : ContentPage
	{
        string sMateria { get; set; }
        string sToken { get; set; }
        string sMat { get; set; }
        public PublicacionDetail (string userToken, string sPublicacion, string sMatName)
		{
			InitializeComponent ();
            MostrarPublicacion(sPublicacion);
            this.sToken = userToken;
            this.sMat = sMatName;

        }
        public async void MostrarPublicacion(string sPublicacion)
        {
            HttpClient cliente = new HttpClient();
            var responseP = await cliente.GetStringAsync(sPublicacion);
            var publicacion = JsonConvert.DeserializeObject<Publicacion>(responseP);
            this.sMateria = publicacion.materia;

            var responsec = await cliente.GetStringAsync("https://cursivia.herokuapp.com/api_v1/comentarios/");
            var comentarios = JsonConvert.DeserializeObject<List<Comentario>>(responsec);

            var responseu = await cliente.GetStringAsync(publicacion.usuario);
            var usuario = JsonConvert.DeserializeObject<Usuario>(responseu);

            sTitulo.Text = publicacion.titulo;
            sUsername.Text = usuario.username;
            sCuerpo.Html = Convert.ToString(publicacion.cuerpo);

            var ListaComentarios = new List<Comentario>();
            foreach (var c in comentarios)
            {
                
                var responseComentarioPertenece = await cliente.GetStringAsync(c.publicacion);
                var publicacionPertenece = JsonConvert.DeserializeObject<Publicacion>(responseComentarioPertenece);
                if (publicacionPertenece.id == publicacion.id)
                {
                    ListaComentarios.Add(c);
                    //Console.WriteLine(c.comentario);
                }

            }

            lComentarios.ItemsSource = ListaComentarios;


        }
         
        private void B_Volver(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new ForoMaterias(this.sToken, this.sMateria, this.sMat));
        }

        public void B_Comentar(object sender, EventArgs e)
        {
            //Application.Current.MainPage = new NavigationPage(new ForoMaterias(this.sToken, this.sMateria, this.sMat));
        }

        public void B_MeGusta(object sender, EventArgs e)
        {
            var authentication = new AuthenticationHeaderValue("Token", this.sToken);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = authentication;
            /*
            MeGusta megusta = new MeGusta()
            {
                
            }
            */
        }

        protected override void OnAppearing()
        {
            Title = sMat;
        }



    }
}