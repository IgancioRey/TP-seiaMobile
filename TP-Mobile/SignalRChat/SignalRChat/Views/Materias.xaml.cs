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
	public partial class Materias : ContentPage
	{
        string sToken { get; set; }
        public Materias(string userToken)
		{
			InitializeComponent ();
            ListarMaterias();
            this.sToken = userToken;
        }

        public async void ListarMaterias()
        {
            HttpClient cliente = new HttpClient();
            var response = await cliente.GetStringAsync("https://cursivia.herokuapp.com/api_v1/materias/");
            var materias = JsonConvert.DeserializeObject<List<Materia>>(response);

            ListaMaterias.ItemsSource = materias;
            ListaMaterias.ItemTapped += ListaMaterias_TappedAsync;

            void ListaMaterias_TappedAsync(object sender, ItemTappedEventArgs e)
            {
                var elementos = e.Item as Materia;
                string sId = Convert.ToString(elementos.id);
                string sMateria = "https://cursivia.herokuapp.com/api_v1/materias/" + sId + "/";
                Application.Current.MainPage = new NavigationPage(new ForoMaterias(this.sToken,sMateria,elementos.descripcion));
            }
        }
         
    }
}