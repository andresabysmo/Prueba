using System;
using System.Configuration;

namespace TvCable.Integration.BusinessLogic
{
    public class Configuration
    {
        public char SeparadorDatosRespuestaCast { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public Configuration()
        {
            GetConfigurationValue();
        }

        internal void GetConfigurationValue()
        {
            SeparadorDatosRespuestaCast = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["SepradorDatosCast"]) ? Convert.ToChar(ConfigurationManager.AppSettings["SepradorDatosCast"]) : Convert.ToChar(",");
            UserName = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["UserNameTvCable"]) ? ConfigurationManager.AppSettings["UserNameTvCable"] : string.Empty;
            Password = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["PasswordTvCable"]) ? ConfigurationManager.AppSettings["PasswordTvCable"] : string.Empty;
        }
    }
}
