using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace WsAdminResidentes.Models.RespuestasBd
{
    public class RespuestaBase
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; }       
        public string Codigo { get; set; }
    }

}