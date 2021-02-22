using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TuAdelanto.Models
{
    public class Empleado
    {
        public int IdEmpleado { get; set; } = 0;
        public int EmpresaId { get; set; } = 0;
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string RFC { get; set; } = "";
        public string CURP { get; set; } = "";
        public string Calle { get; set; } = "";
        public int NumExt { get; set; } = 0;
        public string NumInt { get; set; } = "";
        public string Colonia { get; set; } = "";
        public string Municipio { get; set; } = "";
        public string Ciudad { get; set; } = "";
        public string Estado { get; set; } = "";
        public string CP { get; set; } = "";
        public string Celular { get; set; }
        public string Puesto { get; set; } = "";
        public string Corte { get; set; }
        public decimal IngresoNeto { get; set; } = 0;
        public decimal LimiteCredito { get; set; } = 0;
        public string NumEmpleado { get; set; }
        public string Correo { get; set; }
    }

    public class ConfirmacionCelular { 
        public string Celular { get; set; }
        public string CodigoConfirmacion { get; set; }
    }
}
