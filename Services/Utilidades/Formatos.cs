using System.Collections.Generic;
using System.Linq;

namespace WsAdminResidentes.Services.Utilidades
{
    public enum Formato
    {
        JPG,
        JPEG,
        PNG,
        GIF,
        XLSX
    }

    public static class FormatosExtension{

        public static bool Contains(this List<Formato> lista, string valor) {
            return lista.Where(x=> x.IsEqualTo(valor )).Any();
        }
        public static bool IsEqualTo(this Formato ext, string ext_) {
            string ext_string = ext.ToString();
            ext_ = ext_.Trim().Replace(".", "").ToUpper();
            return string.Equals(ext_string, ext_);
        }

        public static List<string> ToString(this List<Formato> lista) {
            var retorno = lista.Select(x => x.ToString());
            return retorno.ToList();
        }

        public static string ToString(this Formato ext) {
            string v = "";
            switch (ext)
            {
                case Formato.JPG: v = "JPG"; break;
                case Formato.PNG: v = "PNG"; break;
                case Formato.GIF: v = "GIF"; break;
                case Formato.JPEG: v = "JPEG"; break;
            }
            return v;
        }
    }
}
