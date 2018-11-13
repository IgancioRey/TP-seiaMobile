using System;
using System.Collections.Generic;
using System.Text;

namespace SignalRChat.ViewModels
{
    class Publicacion
    {
        public int id { get; set; }
        public string usuario { get; set; }
        public string titulo { get; set; }
        public string cuerpo { get; set; }
        public DateTime fecha_alta { get; set; }
        public int aprovacion { get; set; }
        public string tipo_publicacion { get; set; }
        public String materia { get; set; }

    }
}
