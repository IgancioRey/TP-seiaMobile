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
    public partial class ForoMaterias : ContentPage
    {
        
        string sMat { get; set; }
        string sMatLink { get; set; }
        string sToken { get; set; }
        public ForoMaterias(string userToken, string sMateria, string sMatName)
        {
            InitializeComponent();
            ListarPublicaciones(sMateria);
            this.sMatLink = sMateria;
            this.sToken = userToken;
            this.sMat = sMatName;
            OnAppearing();

        }

        public async void ListarPublicaciones(string sMateria)
        {
            HttpClient cliente = new HttpClient();
            var response = await cliente.GetStringAsync("https://cursivia.herokuapp.com/api_v1/noticias/");
            var publicaciones = JsonConvert.DeserializeObject<List<Publicacion>>(response);
            
            var ListaPublicaciones = new List<Publicacion>();

            var responsem = await cliente.GetStringAsync(sMateria);
            var materia = JsonConvert.DeserializeObject<Materia>(responsem);

            foreach (var publicacion in publicaciones)
            {
                if (publicacion.materia != null)
                {
                    var responseMateriaPertenece = await cliente.GetStringAsync(publicacion.materia);
                    var materiaPertenece = JsonConvert.DeserializeObject<Materia>(responseMateriaPertenece);
                    if (materiaPertenece.id == materia.id)
                    {
                        ListaPublicaciones.Add(publicacion);
                    }
                }
            }

            ListaPublicacionesView.ItemsSource = ListaPublicaciones;
            ListaPublicacionesView.ItemTapped += ListaPublicacionesView_TappedAsync;

            void ListaPublicacionesView_TappedAsync(object sender, ItemTappedEventArgs e)
            {
                var elementos = e.Item as Publicacion;
                string sId = Convert.ToString(elementos.id);
                string sPublicacion = "https://cursivia.herokuapp.com/api_v1/noticias/" + sId + "/";
                Application.Current.MainPage = new NavigationPage(new PublicacionDetail(this.sToken, sPublicacion, this.sMat));
            }

        }

        private void B_Volver(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new Materias(this.sToken));
        }

        protected override void OnAppearing()
        {
            Title = sMat;
        }


    }

}