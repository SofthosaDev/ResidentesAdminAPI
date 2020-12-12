using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TuAdelanto.Models
{
    public class Empresa
    {
        public int IdEmpresa { get; set; } = 0;
        public string NumEmpresa { get; set; }
        public string Nombre { get; set; }
        public string RazonSocial { get; set; }
        public string RFC { get; set; }
        public string Calle { get; set; }
        public int NumExt { get; set; }
        public string NumInt { get; set; }

        public string Colonia { get; set; }
        public string Municipio { get; set; }

        public string Ciudad { get; set; }
        public string Estado { get; set; }
        public string CP { get; set; }
        public string TelEmpresa { get; set; }
        public string CelEmpresa { get; set; }
        public string Referencia { get; set; }
    }
}
