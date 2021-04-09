using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WsAdminResidentes.Helpers;
using HandlebarsDotNet;

namespace WsAdminResidentes.Services.Plantillas
{
    public interface IPlantillasService
    {
        string RemplazarPlantilla(string ruta_plantilla, object data);
    }
    public class PlantillasService : IPlantillasService
    {
        public string RemplazarPlantilla(string ruta_plantilla, object data)
        {
            string ruta_absoluta = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, ruta_plantilla
            );


            if (!File.Exists(ruta_absoluta)) {
                throw (new FileNotFoundException("El archivo de plantilla no Existe"));
            }
            string contenido = File.ReadAllText(ruta_absoluta);
            var template = Handlebars.Compile(contenido);
            var resultado = template(data);
            string final = resultado.ToString();
            return final;
        }
    }
}
