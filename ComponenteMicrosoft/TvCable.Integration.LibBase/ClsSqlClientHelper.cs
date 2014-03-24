using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace TvCable.Integration.LibBase
{
    public class ClsSqlClientHelper
    {
        /// <summary>
        /// Objecto de Conexión.
        /// </summary>
        SqlConnection sqlConn = null;
        /// <summary>
        /// Cadena de Conexión del tipo: Data Source=[DIRECCION_IP];Initial Catalog=[NOMBRE_INSTANCIA_BDD];User ID=[ID_USUARIO];Password=[CONTRASEÑA]
        /// </summary>
        string connString = null;

        /// <summary>
        /// Propiedad para obtener o asignar una cadena de conexión.
        /// </summary>
        public string ConString
        {
            set { connString = value; }
            get { return connString; }
        }

        public ClsSqlClientHelper()
        {
            //ConString = ConfigurationSettings.AppSettings["ConnectionString"].ToString();
            ConString = ConfigurationManager.AppSettings["ConnectionString"].ToString();
        }

        /// <summary>
        /// Ejecuta un StoredProcedure que no retorna tuplas como resultado.
        /// </summary>
        /// <param name="strSpName">Nombre del Stored Procedure</param>
        /// <param name="parameters">Arreglo de Parámetros que recibe el Stored Procedure, en el caso que no reciba parámetros el Stored Procedure se envía null.</param>
        /// <returns>Número de Registros Afectados por el Stored Procedure.</returns>
        public int ExecuteNonQueryStoredProcedure(string strSpName, SqlParameter[] parameters)
        {
            try
            {
                int intAffectedRows = 0;
                using (sqlConn = new SqlConnection(connString))
                {
                    sqlConn.Open();
                    SqlCommand sqlDbCom = sqlConn.CreateCommand();
                    sqlDbCom.CommandText = strSpName;
                    sqlDbCom.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            sqlDbCom.Parameters.Add(parameters[i]);
                        }
                    }
                    sqlDbCom.Prepare();
                    intAffectedRows = sqlDbCom.ExecuteNonQuery();
                    sqlDbCom.Parameters.Clear();
                    sqlDbCom.Dispose();
                    sqlConn.Close();
                }
                return intAffectedRows;
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                throw ex;
            }
        }

        /// <summary>
        /// Ejecuta un Stored Procedure que retorna tuplas como resultado de su ejecución.
        /// </summary>
        /// <param name="strSpName">Nombre del Stored Procedure</param>
        /// <param name="parameters">Arreglo de Parámetros que recive el Stored Procedure, en el caso que no reciba parámetros el Stored Procedure se envía null.</param>
        /// <returns>Las tuplas de resultado de la ejecución del Stored Procedure.</returns>
        public DataTable ExecuteQueryStoredProcedure(string strSpName, SqlParameter[] parameters)
        {
            try
            {
                DataTable dtQuery = new DataTable();
                using (sqlConn = new SqlConnection(connString))
                {
                    sqlConn.Open();
                    SqlCommand sqlCom = sqlConn.CreateCommand();
                    sqlCom.CommandText = strSpName;
                    sqlCom.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            sqlCom.Parameters.Add(parameters[i]);
                        }
                    }
                    sqlCom.Prepare();
                    sqlCom.CommandTimeout = 1000;
                    SqlDataAdapter data = new SqlDataAdapter(sqlCom);
                    data.Fill(dtQuery);
                    sqlCom.Parameters.Clear();
                    sqlCom.Dispose();
                    sqlConn.Close();
                    return dtQuery;
                }
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                throw ex;
            }
        }

        /// <summary>
        /// Ejecuta un Stored Procedure que retorna tuplas como resultado de su ejecución.
        /// </summary>
        /// <param name="strSpName">Nombre del Stored Procedure</param>
        /// <param name="parameters">Arreglo de Parámetros que recive el Stored Procedure, en el caso que no reciba parámetros el Stored Procedure se envía null.</param>
        /// <returns>Las tuplas de resultado de la ejecución del Stored Procedure.</returns>
        public DataSet DSExecuteQueryStoredProcedure(string strSpName, SqlParameter[] parameters)
        {
            try
            {
                DataSet dtQuery = new DataSet();
                using (sqlConn = new SqlConnection(connString))
                {
                    sqlConn.Open();
                    SqlCommand sqlCom = sqlConn.CreateCommand();
                    sqlCom.CommandText = strSpName;
                    sqlCom.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            sqlCom.Parameters.Add(parameters[i]);
                        }
                    }
                    sqlCom.Prepare();
                    SqlDataAdapter data = new SqlDataAdapter(sqlCom);
                    data.Fill(dtQuery);
                    sqlCom.Parameters.Clear();
                    sqlCom.Dispose();
                    sqlConn.Close();
                    return dtQuery;
                }
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                throw ex;
            }
        }

        /// <summary>
        /// Ejecuta un Stored Procedure que retorna tuplas como resultado de su ejecución.
        /// </summary>
        /// <param name="strSpName">Nombre del Stored Procedure</param>    
        /// <returns>Las tuplas de resultado de la ejecución del Stored Procedure.</returns>
        public DataSet DSExecuteQueryStoredProcedure(string strSpName)
        {
            try
            {
                DataSet dtQuery = new DataSet();
                using (sqlConn = new SqlConnection(connString))
                {
                    sqlConn.Open();
                    SqlCommand sqlCom = sqlConn.CreateCommand();
                    sqlCom.CommandText = strSpName;
                    sqlCom.CommandType = CommandType.StoredProcedure;
                    sqlCom.Prepare();
                    SqlDataAdapter data = new SqlDataAdapter(sqlCom);
                    data.Fill(dtQuery);
                    sqlCom.Dispose();
                    sqlConn.Close();
                    return dtQuery;
                }
            }
            catch (Exception ex)
            {
                sqlConn.Close();
                throw ex;
            }
        }
    }
}
