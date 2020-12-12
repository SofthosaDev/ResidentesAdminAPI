using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace TuAdelanto.Models
{
    [JsonObject]
    public class RecuperacionContrasenaModel
    {
        public int IdRecuperacionContrasena { get; set; }
        public string Token { get; set; }
        public DateTime FechaLimite { get; set; }

        public String Correo { get; set; }

    }
}