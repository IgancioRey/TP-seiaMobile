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
	public partial class PublicacionDetail : ContentPage
	{
        string sMateria { get; set; }
        string sToken { get; set; }
        string sMat { get; set; }
        string sUsuario { get; set; }
        string sPublic { get; set; }
        public PublicacionDetail (string userToken, string sPublicacion, string sMatName)
		{
			InitializeComponent ();
            this.sToken = userToken;
            this.sMat = sMatName;
            this.sPublic = sPublicacion;

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
                    //Console.WriteLine(c.comentario);
                }

            }

            lComentarios.ItemsSource = ListaComentarios;


        }
         
        private void B_Volver(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new ForoMaterias(this.sToken, this.sMateria, this.sMat));
        }

        public async void B_Comentar(object sender, EventArgs e)
        {
            HttpClient cliente = new HttpClient();
            var responseP = await cliente.GetStringAsync(this.sPublic);
            var publicacion = JsonConvert.DeserializeObject<Publicacion>(responseP);


            Application.Current.MainPage = new NavigationPage(new ComentarPublicacion(this.sToken, this.sPublic, this.sMat, publicacion.titulo));
        }

        public async void B_MeGusta(object sender, EventArgs e)
        {
            HttpClient cliente4 = new HttpClient();
            var responseMG = await cliente4.GetStringAsync("https://cursivia.herokuapp.com/api_v1/meGusta/");
            var megustas = JsonConvert.DeserializeObject<List<MeGusta>>(responseMG);

            var ListaMG = new List<MeGusta>();
            bool isOk = true;

            HttpClient cliente2 = new HttpClient();
            string sTokenUser = "https://cursivia.herokuapp.com/api_v1/Token/" + this.sToken + "/";
            var responseU = await cliente2.GetStringAsync(sTokenUser);
            var sUser = JsonConvert.DeserializeObject<TokenUser>(responseU);

            this.sUsuario = sUser.user;

            foreach (var MG in megustas)
            {
                if (MG.usuario == this.sUsuario && MG.publicacion == this.sPublic)
                {
                    isOk = false;
                }
            }


            if (isOk)
            {
                HttpClient client = new HttpClient();

                MeGusta megusta = new MeGusta()
                {
                    //id = 4,
                    usuario = this.sUsuario,
                    publicacion = this.sPublic
                };

                var json = JsonConvert.SerializeObject(megusta);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {

                    var result = await client.PostAsync("https://cursivia.herokuapp.com/api_v1/meGusta/", content);

                    if (result.StatusCode == HttpStatusCode.Created)
                    {
                        HttpClient cliente3 = new HttpClient();

                        var responseP = await cliente2.GetStringAsync(sPublic);
                        var sPublicacion = JsonConvert.DeserializeObject<Publicacion>(responseP);

                        Publicacion publicacion = new Publicacion()
                        {
                            id = sPublicacion.id,
                            usuario = sPublicacion.usuario,
                            titulo = sPublicacion.titulo,
                            cuerpo = sPublicacion.cuerpo,
                            fecha_alta = sPublicacion.fecha_alta,
                            aprovacion = sPublicacion.aprovacion + 1,
                            tipo_publicacion = sPublicacion.tipo_publicacion,
                            materia = sPublicacion.materia
                        };

                        var json2 = JsonConvert.SerializeObject(publicacion);

                        var content2 = new StringContent(json2, Encoding.UTF8, "application/json");

                        var result2 = await client.PutAsync(sPublic, content2);

                        if (result2.IsSuccessStatusCode)
                        {
                            Application.Current.MainPage = new NavigationPage(new PublicacionDetail(this.sToken, this.sPublic, this.sMat));
                        }
                    }

                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                    Console.WriteLine(msg);
                }
            }
            else
            {
                await DisplayAlert("Ups!", "Ya te gusta esta publicación", "Ok");
            }

        }

        protected override void OnAppearing()
        {
            Title = sMat;
        }



    }
}