using System;
using System.Data;
using System.Data.SqlClient;
using TvCable.Integration.DAL;

namespace TvCable.Integration.BusinessLogic
{
    public class TvCableIntegrationBl : IDisposable
    {

        public DataSet VerificaLoginUser(string usuario, string clave, string codeComando)
        {
            try
            {
                using (var objTvCabeleIntegrationDal = new TvCableIntegrationDal())
                {
                    return objTvCabeleIntegrationDal.VerificaLoginUser(usuario, clave, codeComando);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet GetDescriptionAnswer(string answer) {
            try
            {
                using (var objTvCabeleIntegrationDal = new TvCableIntegrationDal())
                {
                    return objTvCabeleIntegrationDal.GetDescriptionAnswer(answer);
                }
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        #region IDisposable Members
        void IDisposable.Dispose()
        {
            //throw new NotImplementedException();
        }
        #endregion
    }
}
