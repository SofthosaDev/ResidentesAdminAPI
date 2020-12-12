using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;


public static class CodigoErrores { 
    public static readonly string USUARIO_INVALIDO = "01";
    public static readonly string USUARIO_NUEVO = "02";
    public static readonly string USUARIO_BLOQUEADO = "03";
    public static readonly string REFRESH_TOKEN_INCORRECTO = "04";
    public static readonly string ERROR_GENERICO = "99";
}
public static class Helpers
{
    static public IEnumerable<object> Combine<T, U>(this T one, U two)
    {
        var properties1 = one.GetType().GetProperties().Where(p => p.CanRead && p.GetValue(one, null) != null).Select(p => p.GetValue(one, null));
        var properties2 = two.GetType().GetProperties().Where(p => p.CanRead && p.GetValue(two, null) != null).Select(p => p.GetValue(two, null));

        return new List<object>(properties1.Concat(properties2));
    }
}