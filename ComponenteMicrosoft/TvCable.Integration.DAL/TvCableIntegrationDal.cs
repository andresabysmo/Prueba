using System;
using System.Data;
using System.Data.SqlClient;
using TvCable.Integration.DTO;
using TvCable.Integration.LibBase;

namespace TvCable.Integration.DAL
{
    public class TvCableIntegrationDal : IDisposable
    {
        public DataSet VerificaLoginUser(string usuario, string clave, string codeComando)
        {
            try
            {
                var prmParametros = new SqlParameter[]
                {
                    string.IsNullOrEmpty(usuario) ? new SqlParameter("@i_user",  DBNull.Value) : new SqlParameter("@i_user",  usuario),
                    string.IsNullOrEmpty(clave) ? new SqlParameter("@i_password",  DBNull.Value) : new SqlParameter("@i_password",  clave),
                    string.IsNullOrEmpty(codeComando) ? new SqlParameter("@i_commandCode",  DBNull.Value) : new SqlParameter("@i_commandCode",  codeComando)             
                };                
                var objDatos = new ClsSqlClientHelper();
                var dsResult = objDatos.DSExecuteQueryStoredProcedure(Constants.SpValidateUserLogin, prmParametros);                
                return dsResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public DataSet GetDescriptionAnswer(string answer) {
            try
            {
                var prmParametros = new SqlParameter[]
                {
                    new SqlParameter("@i_answer", answer)
                };

                var objDatos = new ClsSqlClientHelper();
                var dsResult = objDatos.DSExecuteQueryStoredProcedure(Constants.SpGetDescriptionAnswer, prmParametros);
                return dsResult;
            }
            catch (Exception ex) { 
                    throw ex;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            //throw new NotImplementedException();
        }

        #endregion
    }
}
