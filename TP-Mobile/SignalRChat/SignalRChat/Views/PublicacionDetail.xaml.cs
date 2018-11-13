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

namespace SignalRChat.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PublicacionDetail : ContentPage
	{
        public string sMateria { get; set; }
        public PublicacionDetail (string sPublicacion)
		{
			InitializeComponent ();
            MostrarPublicacion(sPublicacion);

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
                }

                lComentarios.ItemsSource = ListaComentarios;

            }


        }
        /*
        public void bVolver()
        {
            Application.Current.MainPage = new NavigationPage(new ForoMaterias(sMateria));
        }
        */




    }
}