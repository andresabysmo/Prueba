using System;
using System.IO;
using System.Configuration;

namespace TvCable.Integration.LibBase
{
    public class Util
    {
        public enum ErrorTypeEnum
        {
            ErrorType,
            TraceType,
            Warning
        }        

        /// <summary>
        /// Write log
        /// </summary>
        /// <param name="logModule">Usage mode: this.GetType().ToString()</param>
        /// <param name="message">Usage mode: ex.ToString()</param>
        /// <param name="typeError">Error type enum </param>
        public void WriteLog(string logModule, string message, ErrorTypeEnum ErrorType)
        {
            string logFilePath = ConfigurationManager.AppSettings["LogFileNamePath"];
            string EnableLog = ConfigurationManager.AppSettings["EnableLog"];
            string errorType;
            logFilePath = logFilePath.Replace(".txt", string.Format("{0}{1}{2}.txt", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year));
            // && File.Exists(logFilePath)
            if (string.Compare(EnableLog, "1") == 0 && !string.IsNullOrEmpty(logFilePath))
            {
                switch (ErrorType)
                {
                    case ErrorTypeEnum.ErrorType:
                        errorType = "Error: ";
                        break;
                    case ErrorTypeEnum.TraceType:
                        errorType = "Trace: ";
                        break;
                    case ErrorTypeEnum.Warning:
                        errorType = "Warning: ";
                        break;
                    default:
                        errorType = "Error: ";
                        break;
                }

                WriteToLogFile(logFilePath, errorType + "- " + logModule + " - " + message);
            }
        }

        /// <summary>
        /// Registra el Log en un File
        /// </summary>
        /// <param name="strLogFile"></param>
        /// <param name="logMessage"></param>
        private static void WriteToLogFile(string strLogFile, string logMessage)
        {
            string strLogMessage = string.Empty;
            StreamWriter swLog = null;
            try
            {
                strLogMessage = string.Format("{0}: {1}", DateTime.Now, logMessage);

                if (!File.Exists(strLogFile))
                {
                    swLog = new StreamWriter(strLogFile);
                }
                else
                {
                    swLog = File.AppendText(strLogFile);
                }

                swLog.WriteLine(strLogMessage);
                //swLog.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error escribiendo en archivo de LOG: " + ex.ToString()); // tratar de escribir al eventviewer
            }
            finally
            {
                if (swLog != null)
                {
                    swLog.Close();
                    swLog.Dispose();
                    swLog = null;
                }
                strLogMessage = null;
            }
        }

        /// <summary>
        /// Obtiene los elementos dentro del objeto de string[]
        /// </summary>
        /// <param name="objectWithString"></param>
        /// <returns></returns>
        public string getStringInObjects(string[] objectWithString)
        {
            //CF: Variable que va a tomar los valores
            string returnValues = string.Empty;

            //Verificamos que si no hay elementos, regrese con el valor preestablecido
            if (objectWithString.Length == 0) return returnValues;

            for (int i = 0; i <= objectWithString.Length; i++)
            {
                //Durante el recorrido si es el ultimo elemento, lo agrega sin "."
                if (i == objectWithString.Length - 1)
                    {
                        returnValues += objectWithString[i] + " ";
                        break;
                    }
                    else 
                    {
                    //Si hay mas de 2 elementos agregamos ","
                        returnValues += objectWithString[i] + ", "; 
                    }                                   
            }            
            return returnValues;
        }


        /// <summary>
        /// Generate the timestamp for the signature       
        /// </summary>
        /// <returns></returns>
        public virtual string GenerateTimeStamp()
        {
            // Default implementation of UNIX time of the current UTC time
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }


    }
}
