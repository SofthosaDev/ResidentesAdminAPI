using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerrosApp.Models.Empleado
{
    public class EmpleadoDatos
    {
		public string RFC { get; set; }
		public string CURP { get; set; }
		public string Calle { get; set; }
		public int NumExt { get; set; }
		public string NumInt { get; set; }
		public string Colonia { get; set; }

		public string Municipio { get; set; }

		public string Ciudad { get; set; }
		public string Estado { get; set; }
		public string CP { get; set; }
		public string Correo { get; set; }

	}
}
