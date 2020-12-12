using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Data.SqlTypes;
using TuAdelanto.Helpers;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using TuAdelanto.Models;

namespace TuAdelanto.Classes
{

    public class RespuestaBDToken : RespuestaBDModel
    {
        public string token { get; set; }
    }

    public class DataBase : IDisposable
    {
        private SqlConnection _conexion;
        private SqlCommand command;
        private readonly AppSettings _appSettings;
        public int result_number = -1;
        public string result_msg = "";
        public void Dispose()
        {
            throw new NotImplementedException();
        }
        public DataBase(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }
        public DataSet getDataSet()
        {
            DataSet data = new DataSet();
            try
            {
                if (_conexion.State == ConnectionState.Closed)
                {
                    _conexion.Open();
                }
                //AddParametersSys();
                SqlDataAdapter dataAdapterSearch = new SqlDataAdapter();

                dataAdapterSearch.SelectCommand = command;
                dataAdapterSearch.Fill(data);
                dataAdapterSearch.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _conexion.Close();
            }
            return data;

        }

        public List<string> GetParametrosInvalidos(Object parametros, string sp)
        {
            List<string> respuesta = new List<string>();
            List<string> lista = new List<string>();
            lista = ListarParametros(sp);

            Type myType = parametros.GetType();
            Type tipo = parametros.GetType();
            SetCommand(sp);
            System.Reflection.PropertyInfo[] propiedades = tipo.GetProperties();
            for (int i = 0; i < propiedades.Length; i++)
            {
                string nombre_col = propiedades[i].Name;
                System.Reflection.PropertyInfo propiedad = parametros.GetType().GetProperty(nombre_col);
                Type tipo_dato = propiedad.GetType();
                object pre_valor_aux = propiedad.GetValue(parametros);
                if (pre_valor_aux != null && !lista.Contains(propiedad.Name))
                {
                    respuesta.Add(propiedad.Name);
                }
            }
            return respuesta;


        }
        private List<string> ListarParametros(string sp)
        {
            List<string> lista = new List<string>();
            SqlCommand cmd = new SqlCommand(sp, _conexion);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlCommandBuilder.DeriveParameters(cmd);
            foreach (SqlParameter p in cmd.Parameters)
            {
                lista.Add(p.ParameterName.Replace("@", ""));
            }
            return lista;
        }


        private void SqlInfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            string[] strMensaje;
            strMensaje = e.Message.Split('§');

            if (e.Errors[0].Number == 8153)
            {
                result_number = -1;
                result_msg = "";
                return;
            }

            if (strMensaje.Length == 1)
            {
                result_number = 0;
                result_msg = strMensaje[0];
            }
            else
            {
                result_number = Convert.ToInt32(strMensaje[0]);
                result_msg = strMensaje[1];
            }
        }
        public void Close()
        {
            if (_conexion != null)
                _conexion.Close();
        }

        public void Open()
        {
            string conString = this._appSettings.CadenaConexion;

            // open connection
            if (_conexion == null)
            {
                _conexion = new SqlConnection(conString);

                _conexion.Open();
                _conexion.InfoMessage += new SqlInfoMessageEventHandler(SqlInfoMessage);
            }
        }

        private void SetCommand(string Comando)
        {
            if (command == null)
            {
                this.Open();
                command = new SqlCommand(Comando, _conexion);
                command.CommandTimeout = 36000;
            }
            else
            {
                try
                { command.Cancel(); }
                catch (Exception)
                { }
                try
                { command.Parameters.Clear(); }
                catch (Exception)
                { }
            }
            command.CommandText = Comando;
            command.CommandType = CommandType.StoredProcedure;
        }

        public DataSet ejecutarSp(string sp, Object parametros, int Id_Usuario = 0)
        {
            Type tipo = parametros.GetType();
            SetCommand(sp);
            List<string> lista_ivalidos = GetParametrosInvalidos(parametros, sp);
            if (lista_ivalidos.Count > 0)
            {
                throw new ArgumentException(
                    $"Los siguientes parámetros no existen en el sp {sp}: {JsonSerializer.Serialize(lista_ivalidos)}"
                );
            }

            //Si el Id_Usuario no es requerido por el SP no se envía
            AgregarUsuarioSiSeRequiere(Id_Usuario, sp);

            System.Reflection.PropertyInfo[] propiedades = tipo.GetProperties();
            for (int i = 0; i < propiedades.Length; i++)
            {
                string nombre_col = propiedades[i].Name;
                System.Reflection.PropertyInfo propiedad = parametros.GetType().GetProperty(nombre_col);
                Type tipo_dato = propiedad.GetType();
                object pre_valor_aux = propiedad.GetValue(parametros);
                string pre_valor = "";

                if (pre_valor_aux == null)
                {
                    pre_valor = "";
                }
                else
                {
                    pre_valor = pre_valor_aux.ToString();
                }

                if (propiedad.PropertyType == typeof(int))
                {
                    int valor = int.Parse(pre_valor);
                    CreateParameter($"@{nombre_col}", valor);
                }
                else if (propiedad.PropertyType == typeof(bool))
                {
                    bool valor = bool.Parse(pre_valor);
                    CreateParameter($"@{nombre_col}", valor);
                }
                else if (propiedad.PropertyType == typeof(string))
                {
                    if (pre_valor_aux != null)
                        CreateParameter($"@{nombre_col}", pre_valor);
                }
                else if (propiedad.PropertyType == typeof(Double))
                {
                    Double valor = Double.Parse(pre_valor);
                    CreateParameter($"@{nombre_col}", valor);
                }
                else if (propiedad.PropertyType == typeof(float))
                {
                    float valor = float.Parse(pre_valor);
                    CreateParameter($"@{nombre_col}", valor);
                }
                else if (propiedad.PropertyType == typeof(decimal))
                {
                    decimal valor = decimal.Parse(pre_valor);
                    CreateParameter($"@{nombre_col}", valor);
                }
                else if (propiedad.PropertyType == typeof(DateTime))
                {
                    DateTime valor = DateTime.Parse(pre_valor);
                    CreateParameter($"@{nombre_col}", valor);
                }
                else if (propiedad.PropertyType == typeof(DateTime?))
                {

                    if (pre_valor == "" || pre_valor == null)
                    {
                    }
                    else
                    {
                        DateTime valor = DateTime.Parse(pre_valor);
                        CreateParameter($"@{nombre_col}", valor);
                    }

                }
                else if (propiedad.PropertyType.IsArray || propiedad.PropertyType.Namespace == "System.Collections.Generic")
                {
                    string valor = JsonSerializer.Serialize(pre_valor_aux);
                    CreateParameter($"@{nombre_col}", valor);
                }
            }

            return this.getDataSet();
        }


        public void CreateParameter(string strParameterName, int nParameter)
        {
            command.Parameters.Add(strParameterName, System.Data.SqlDbType.Int, 4).Value = nParameter;
        }

        public void CreateParameter(string strParameterName, int? nParameter)
        {
            command.Parameters.Add(strParameterName, System.Data.SqlDbType.Int, 4).Value = nParameter;
        }

        /// <summary>
        /// Crea Parametro de entrada de tipo Entero largo
        /// </summary>
        /// <param name="strParameterName"></param>
        /// <param name="nParameter"></param>
        public void CreateParameter(string strParameterName, double nParameter)
        {
            command.Parameters.Add(strParameterName, System.Data.SqlDbType.Money, 8).Value = nParameter;
        }

        public void CreateParameter(string strParameterName, string strParameter, int nLenght)
        {
            //Si la longitud es Cero ponerlo como Text

            if (nLenght == 0)
            {
                command.Parameters.Add(strParameterName, System.Data.SqlDbType.NText).Value = strParameter;
            }
            else
            {
                command.Parameters.Add(strParameterName, System.Data.SqlDbType.NVarChar, nLenght).Value = strParameter;
            }
        }
        public void CreateParameter(string strParameterName, string strParameter)
        {

            command.Parameters.Add(strParameterName, System.Data.SqlDbType.NText).Value = strParameter;

        }

        public void CreateParameter(string strParameterName, DateTime dateTimeParameter)
        {
            command.Parameters.Add(strParameterName, System.Data.SqlDbType.DateTime, 8).Value = dateTimeParameter;
        }

        public void CreateParameter(string strParameterName, float fParameter)
        {
            command.Parameters.Add(strParameterName, System.Data.SqlDbType.Float, 8).Value = fParameter;
        }

        public void CreateParameter(string strParameterName, decimal fParameter)
        {
            command.Parameters.Add(strParameterName, System.Data.SqlDbType.Float, 8).Value = fParameter;
        }

        public void CreateParameter(string strParameterName, bool bParameter)
        {
            command.Parameters.Add(strParameterName, System.Data.SqlDbType.Bit, 1).Value = bParameter;
        }
        public void CreateParameter(string strParameterName, byte[] imgParameter)
        {
            command.Parameters.Add(strParameterName, SqlDbType.Image).Value = imgParameter;
        }
        public void CreateParameter(string strParameterName, XmlDocument nParameter)
        {
            command.Parameters.Add(strParameterName, System.Data.SqlDbType.Xml).Value = new SqlXml(new XmlTextReader(nParameter.InnerXml, XmlNodeType.Document, null));

        }
        public void CreateParameter(string strParameterName, SqlXml nParameter)
        {
            command.Parameters.Add(strParameterName, System.Data.SqlDbType.Xml).Value = nParameter;

        }
        private void AgregarUsuarioSiSeRequiere(int Id_Usuario, string sp) {
            if (Id_Usuario == 0)
                return;
            List<string> l_i = GetParametrosInvalidos(new { UsuarioAccionId = Id_Usuario }, sp);
            if (l_i.Count == 0)
            {
                CreateParameter($"@UsuarioAccionId", Id_Usuario);
            }
        }

    }
}