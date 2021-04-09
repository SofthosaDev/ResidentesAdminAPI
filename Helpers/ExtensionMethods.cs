using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using WsAdminResidentes.Models;

namespace WsAdminResidentes.Helpers
{
    public static class ExtensionMethods
    {
        public static IEnumerable<Usuario> WithoutPasswords(this IEnumerable<Usuario> users)
        {
            return users.Select(x => x.WithoutPassword());
        }

        public static Usuario WithoutPassword(this Usuario usuario)
        {
            usuario.Password = null;
            return usuario;
        }

        public static List<T> FirstToList<T>(this DataSet ds) where T : class, new() {
            try {
                return ds.Tables[0].ToList<T>();
            }
            catch (Exception er) {
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
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                        }
                        catch
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