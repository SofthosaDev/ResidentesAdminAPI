using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TuAdelanto.Models.Utilidades
{
    public class ArchivoModel
    {
        public string Nombre { get; set; }
        public IFormFile Archivo { get; set; }
    }
}
