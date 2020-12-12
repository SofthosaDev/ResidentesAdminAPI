using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace TuAdelanto.Models
{
    public class RespuestaBDModel
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; }       
        public string Codigo { get; set; }
    }

    public class RespuestaInsercionBDModel : RespuestaBDModel{
        public int Id { get; set; } = 0;
    }
    public class RespuestaInsercionMasivaBDModel : RespuestaBDModel
    {
        public string Ids { get; set; } = "";
    }

    public class RespuestaActualizacionBDModel : RespuestaBDModel
    {
        public int Id { get; set; } = 0;
        public bool Encontrado { get; set; }
    }


}