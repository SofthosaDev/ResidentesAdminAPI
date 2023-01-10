using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using WsAdminResidentes.Models;

namespace WsAdminResidentes.Helpers
{
    public static class ExtensionMethods
    {
        public static bool ValidateJSON(this string s)
        {
            try
            {
                JToken.Parse(s);
                return true;
            }
            catch (JsonReaderException ex)
            {
                Trace.WriteLine(ex);
                return false;
            }
        }
        public static IEnumerable<Usuario> WithoutPasswords(this IEnumerable<Usuario> users)
        {
            return users.Select(x => x.WithoutPassword());
        }

        public static Usuario WithoutPassword(this Usuario usuario)
        {
            usuario.Password = null;
            return usuario;
        }

        public static List<T> FirstToList<T>(this DataSet ds) where T : class, new()
        {
            try
            {
                return ds.Tables[0].ToList<T>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Converts a DataTable to a list with generic objects
        /// </summary>
        /// <typeparam name="T">Generic object</typeparam>
        /// <param name="table">DataTable</param>
        /// <returns>List with generic objects</returns>
        public static List<T> ToList<T>(this DataTable table) where T : class, new()
        {
            try
            {
                List<T> list = new List<T>();

                foreach (var row in table.AsEnumerable())
                {
                    T obj = new T();

                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            //objeto normal 1
                            //Lista de objetos normales 
                            //Lista de objetos personalizados

                            var valor = row[prop.Name];
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            Type tipo = propertyInfo.PropertyType;

                            bool es_objeto_normal = propertyInfo.PropertyType.Namespace.StartsWith("System");
                            bool es_lista = tipo.IsGenericType && tipo.GetGenericTypeDefinition() == typeof(List<>);
                            bool es_lista_basica = false;

                            if (es_lista) {
                                var tipo_de_lista = tipo.GenericTypeArguments.FirstOrDefault();
                                if (tipo_de_lista == null)
                                    continue;
                                es_lista_basica = tipo_de_lista.Namespace.StartsWith("System");
                            }

                            if (es_objeto_normal && !es_lista)
                            {
                                propertyInfo.SetValue(obj, Convert.ChangeType(valor, propertyInfo.PropertyType), null);
                            }
                            else
                            {
                                string valor_json = valor.ToString();
                                object valor_parseado = null;
                                if (valor_json.ValidateJSON())
                                {
                                    //List básica ejemplo: List<string>, int, double, float, etc
                                    if (es_lista && es_lista_basica)
                                    {
                                        valor_parseado = JsonConvert.DeserializeObject(valor_json, tipo);
                                    }
                                    else {
                                        DataTable dt = (DataTable)JsonConvert.DeserializeObject(
                                        valor_json, (typeof(DataTable)));
                                        Type tipo_generico = propertyInfo.PropertyType.GenericTypeArguments.First();
                                        Type[] typeArgs = { tipo_generico };
                                        var methods = typeof(ExtensionMethods).GetMethods();
                                        var method = methods.Single(mi => mi.Name == "ToList");
                                        valor_parseado = method.MakeGenericMethod(typeArgs).Invoke(null, new object[] { dt });
                                    }
                                    
                                }
                                else
                                {
                                    //JSON NO VALIDO, SE LE ASIGNA NULL
                                    valor_parseado = null;
                                }

                                propertyInfo.SetValue(obj, valor_parseado);

                            }

                        }
                        catch (Exception e)
                        {
                            continue;
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }

    }

}