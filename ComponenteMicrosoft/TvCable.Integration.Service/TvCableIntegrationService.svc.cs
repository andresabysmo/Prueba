using System;
using TvCable.Integration.DTO;
using TvCable.Integration.LibBase;
using TvCable.Integration.Service.WRTuvesService;
using TvCable.Integration.BusinessLogic;
using System.Text;
using System.Web;

//---------------------------------------------------------------------------------------------
//Modificaciones:
//  7/feb/2013 (Mauricio Camañero) MC1 - Se cambia el tipo de dato del parámetro MASTERCARD
//---------------------------------------------------------------------------------------------

namespace TvCable.Integration.Service
{
    public class TvCableIntegrationService : ITvCableIntegrationService
    {
        readonly Configuration _configuration = new Configuration();
        readonly Util _util = new Util();
        readonly Gateway2 _wrTuvesService = new Gateway2Client();
        readonly TvCableIntegrationBl _tvCableIntegrationBl = new TvCableIntegrationBl();
        
        string timeStampProcess;

        /// <summary>
        /// Este comando permite habilitar el servicio de televisión satelital a un cliente final.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="sequence"></param>
        /// <param name="smartCards"></param>
        /// <param name="settopBox"></param>
        /// <param name="packs"></param>
        /// <param name="masterCard"></param>
        /// <param name="userId"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="notes"></param>
        /// <param name="customerId"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        public InstallResponse Install(string userName, string password, string sequence, string smartCards, SettopBox settopBox, string[] packs, string masterCard, string userId,
            string referenceNumber, string notes, string customerId, string zone)
        {
            var objInstallResponse = new InstallResponse();

            timeStampProcess = string.Empty;
            timeStampProcess = _util.GenerateTimeStamp();                
            
            try
            {
                //CF: Escribimos en el Log para verificar los datos recibidos.
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.InstallMetodo, 
                    // MC1 Se agrega la SMARTCARD para que se muestre en el LOG
                    "REQUEST: [ProcessId: " + timeStampProcess + "] [UserName: " + userName + ". Password: " + password + ". Sequence: " + sequence + ". SmartCard: " + smartCards +
                    ". SetTopBox: [ " + "Code: " + settopBox.Code + ". Brand: " + settopBox.Brand + ". Model: " + settopBox.Model + ". Firmware: " + settopBox.Firmware + "] " +
                    ". Packs: [ " + _util.getStringInObjects(packs) + "]. MasterCard: " + masterCard + ". UserId: " + userId + "]", Util.ErrorTypeEnum.TraceType);
                
               // var passwordEncriptado = EncriptaMD5(password);
                var validateUserResponse = _tvCableIntegrationBl.VerificaLoginUser(userName, password, Constants.InstallMetodo);
                
                if (validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString().Equals(Constants.DefaultOkCode))
                {
                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.InstallMetodo, 
                        "VALIDATE USER RESPONSE: [ProcessId: " + timeStampProcess + "] [" + validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString() + "]", Util.ErrorTypeEnum.TraceType);

                    var objSettopboxType = new SettopboxType();

                    if (settopBox != null)
                    {
                        objSettopboxType = new SettopboxType
                        {
                            code = settopBox.Code,
                            brand = settopBox.Brand,
                            model = settopBox.Model,
                            firmware = settopBox.Firmware
                        };
                    }

                    var installRequest = new install
                    {
                        username = _configuration.UserName,
                        password = _configuration.Password,
                        sequence = int.Parse(sequence),
                        customer_id = customerId,
                        //mastercard = Convert.ToDouble(masterCard),
                        //mc1 Cambia de tipo de dato de Double a String
                        mastercard = masterCard,
                        notes = notes,
                        Packs = packs,
                        reference_number = referenceNumber,
                        Settopbox = objSettopboxType,
                        smartcard = Convert.ToDouble(smartCards),
                        user_id = userId,
                        zone = zone
                    };

                    var objinstallRequest = new installRequest { install = installRequest };
                    var responseInstallWsTuves = _wrTuvesService.install(objinstallRequest);

                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.InstallMetodo, 
                        "RESPONSE TUVES: [ProcessId: " + timeStampProcess + "] [Answer: " + responseInstallWsTuves.installResponse.answer + ". ReturnValue: " + Convert.ToString(responseInstallWsTuves.installResponse.@return) + "]", 
                        Util.ErrorTypeEnum.TraceType);

                    ///////////////
                    //CF: Verifica si el resultado es igual a 0, caso contrario, devolvemos el resultado del error generico y error generado por Tuves
                    if (responseInstallWsTuves.installResponse.@return.ToString().Equals(Constants.DefaultOkCode))
                    {
                        objInstallResponse.ResultCode = Constants.DefaultOkCode;
                        objInstallResponse.ResultDescription = Constants.DefaultOkDescription;
                        objInstallResponse.Answer = responseInstallWsTuves.installResponse.answer;
                        objInstallResponse.ReturnValue = Convert.ToString(responseInstallWsTuves.installResponse.@return);                   
                    }
                    else
                    {
                        var getDescriptionAnswerResponse = _tvCableIntegrationBl.GetDescriptionAnswer(responseInstallWsTuves.installResponse.@return.ToString());
                        objInstallResponse.ResultCode = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                        objInstallResponse.ResultDescription = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                        objInstallResponse.Answer = responseInstallWsTuves.installResponse.answer;
                        objInstallResponse.ReturnValue = Convert.ToString(responseInstallWsTuves.installResponse.@return);                    
                    }              
                    ////////
                }
                else {
                    objInstallResponse.ResultCode = validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                    objInstallResponse.ResultDescription = validateUserResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();                    
                    objInstallResponse.Answer = string.Empty;
                    objInstallResponse.ReturnValue = string.Empty;
                }
                /////
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.InstallMetodo, 
                    "RESPONSE: [ProcessId: " + timeStampProcess + "] [Answer: " + objInstallResponse.Answer + ". ReturnValue: " + objInstallResponse.ReturnValue + "]" + 
                    " [ResultCode: " + objInstallResponse.ResultCode + "]" + " [ResultDescription: " + objInstallResponse.ResultDescription + "]", Util.ErrorTypeEnum.TraceType);
                /////
                return objInstallResponse;              
            }
            catch (Exception ex)
            {
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.InstallMetodo, "EXCEPTION: [ProcessId: " + timeStampProcess +"] " + ex.ToString(), Util.ErrorTypeEnum.ErrorType);
                objInstallResponse = new InstallResponse();
                objInstallResponse.ResultCode = Constants.DefaultErrorCode;
                objInstallResponse.ResultDescription = Constants.DefaultErrorDescription;
                objInstallResponse.Answer = string.Empty;
                objInstallResponse.ReturnValue = string.Empty;
                return objInstallResponse;
            }
        }

        /// <summary>
        /// Este comando permite cortar el servicio de televisión a los equipos instalados en el cliente.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="sequence"></param>
        /// <param name="smartCards"></param>
        /// <param name="userId"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="notes"></param>
        /// <param name="customerId"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        public DisconnectResponse Disconnect(string userName, string password, string sequence, string smartCards, string userId,
            string referenceNumber, string notes, string customerId, string zone)
        {
            var objDisconnectResponse = new DisconnectResponse();

            timeStampProcess = string.Empty;
            timeStampProcess = _util.GenerateTimeStamp();   

            try
            {
                //CF: Escribimos en el Log para verificar los datos recibidos.
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.Disconnect, 
                    "REQUEST: [ProcessId: " + timeStampProcess + "] [UserName: " + userName + ". Password: " + password + ". Sequence: " + sequence + ". SmartCards: " + smartCards + 
                    ". UserId: " + userId + ". ReferenceNumber: " + referenceNumber + ". Notes: "  + notes + ". CustomerId: " + customerId + ". Zone: " + zone + "]", Util.ErrorTypeEnum.TraceType);

               // var passwordEncriptado = EncriptaMD5(password);
                var validateUserResponse = _tvCableIntegrationBl.VerificaLoginUser(userName, password, Constants.Disconnect);                

                if (validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString().Equals(Constants.DefaultOkCode))
                {
                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.Disconnect, 
                        "VALIDATE USER RESPONSE: [ProcessId: " + timeStampProcess + "] [" + validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString() + "]", Util.ErrorTypeEnum.TraceType);

                    var objDisconnect = new disconnect
                    {
                        username = _configuration.UserName,
                        password = _configuration.Password,
                        sequence = int.Parse(sequence),
                        smartcard = Convert.ToDouble(smartCards),
                        user_id = userId,
                        reference_number = referenceNumber,
                        notes = notes,
                        customer_id = customerId,
                        zone = zone
                    };

                    var objDisconnectRequest = new disconnectRequest();
                    objDisconnectRequest.disconnect = objDisconnect;
                    var responseDisconnectWsTuves = _wrTuvesService.disconnect(objDisconnectRequest);

                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.Disconnect, 
                        "RESPONSE TUVES: [ProcessId: " + timeStampProcess + "] [Answer: " + responseDisconnectWsTuves.disconnectResponse.answer + ". ReturnValue: " + Convert.ToString(responseDisconnectWsTuves.disconnectResponse.@return) + "]", Util.ErrorTypeEnum.TraceType);

                      ///////////////
                       //CF: Verifica si el resultado es igual a 0, caso contrario, devolvemos el resultado del error generico y error generado por Tuves
                    if (responseDisconnectWsTuves.disconnectResponse.@return.ToString().Equals(Constants.DefaultOkCode))
                    {
                        objDisconnectResponse.ResultCode = Constants.DefaultOkCode;
                        objDisconnectResponse.ResultDescription = Constants.DefaultOkDescription;
                        objDisconnectResponse.Answer = responseDisconnectWsTuves.disconnectResponse.answer;
                        objDisconnectResponse.ReturnValue = Convert.ToString(responseDisconnectWsTuves.disconnectResponse.@return);
                    }
                    else
                    {
                        var getDescriptionAnswerResponse = _tvCableIntegrationBl.GetDescriptionAnswer(responseDisconnectWsTuves.disconnectResponse.@return.ToString());
                        objDisconnectResponse.ResultCode = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                        objDisconnectResponse.ResultDescription = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                        objDisconnectResponse.Answer = responseDisconnectWsTuves.disconnectResponse.answer;
                        objDisconnectResponse.ReturnValue = Convert.ToString(responseDisconnectWsTuves.disconnectResponse.@return);                    
                    }
                    ///////////////////                   
                }
                else {
                    // Obtengo el mensaje del codigo de error para agregar al objeto de respuesta
                    objDisconnectResponse.ResultCode = validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                    objDisconnectResponse.ResultDescription = validateUserResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                    objDisconnectResponse.ReturnValue = string.Empty;
                    objDisconnectResponse.Answer = string.Empty;                 
                }
                ////
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.Disconnect, 
                    "RESPONSE: [ProcessId " + timeStampProcess + "] [Answer: " + objDisconnectResponse.Answer + ". ReturnValue: " + objDisconnectResponse.ReturnValue + "]" + 
                    " [ResultCode: " + objDisconnectResponse.ResultCode + "]" + " [ResultDescription: " + objDisconnectResponse.ResultDescription + "]", Util.ErrorTypeEnum.TraceType);
                ////
                return objDisconnectResponse;
            }
            catch (Exception ex)
            {
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.Disconnect, "EXCEPTION: [ProcessId: " + timeStampProcess + "] " + ex.ToString(), Util.ErrorTypeEnum.ErrorType);
                objDisconnectResponse = new DisconnectResponse();
                objDisconnectResponse.ResultCode = Constants.DefaultErrorCode;
                objDisconnectResponse.ResultDescription = Constants.DefaultErrorDescription;
                objDisconnectResponse.Answer = string.Empty;
                objDisconnectResponse.ReturnValue = string.Empty;
                return objDisconnectResponse;
            }
        }

        /// <summary>
        /// Este comando permite reconectar el servicio de televisión a un equipo previamente deshabilitado por el sistema. 
        /// Se habilitará la última información registrada previamente.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="sequence"></param>
        /// <param name="smartCards"></param>
        /// <param name="packs"></param>
        /// <param name="userId"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="notes"></param>
        /// <param name="customerId"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        public ReconnectResponse Reconnect(string userName, string password, string sequence, string smartCards, string[] packs, string userId,
            string referenceNumber, string notes, string customerId, string zone)
        {
            var objReconnectResponse = new ReconnectResponse();
            
            timeStampProcess = string.Empty;            
            timeStampProcess = _util.GenerateTimeStamp();   

            try
            {
                //CF: Escribimos en el Log para verificar los datos recibidos.
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.Reconnect, 
                    "REQUEST: [ProcessId: " + timeStampProcess + "] [UserName: " + userName + ". Password: " + password + ". Sequence: " + sequence + ". SmartCards: " + smartCards + 
                    ". Packs: [" + _util.getStringInObjects(packs) + "]. UserId: " + userId + ". ReferenceNumber: " + referenceNumber + ". Notes: " + notes + ". CustomerId: " + customerId + 
                    ". Zone: " + zone + "]", Util.ErrorTypeEnum.TraceType);

              //  var passwordEncriptado = EncriptaMD5(password);
                var validateUserResponse = _tvCableIntegrationBl.VerificaLoginUser(userName, password, Constants.Reconnect);

                if (validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString().Equals(Constants.DefaultOkCode))
                {
                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.Reconnect, 
                        "VALIDATE USER RESPONSE: [ProcessId: " + timeStampProcess + "] [" + validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString() + "]", Util.ErrorTypeEnum.TraceType);

                    var objReconnect = new reconnect
                    {
                        username = _configuration.UserName,
                        password = _configuration.Password,
                        sequence = int.Parse(sequence),
                        smartcard = Convert.ToDouble(smartCards),
                        Packs = packs,
                        user_id = userId,
                        reference_number = referenceNumber,
                        notes = notes,
                        customer_id = customerId,
                        zone = zone
                    };

                    var objreconnectRequest = new reconnectRequest();
                    objreconnectRequest.reconnect = objReconnect;
                    var responseReconnectWsTuves = _wrTuvesService.reconnect(objreconnectRequest);

                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.Reconnect, 
                        "RESPONSE TUVES: [ProcessId: " + timeStampProcess + "] [Answer: " + responseReconnectWsTuves.reconnectResponse.answer + ". ReturnValue: " + Convert.ToString(responseReconnectWsTuves.reconnectResponse.@return) + "]", Util.ErrorTypeEnum.TraceType);

                    ///////////////
                    //CF: Verifica si el resultado es igual a 0, caso contrario, devolvemos el resultado del error generico y error generado por Tuves
                    if (responseReconnectWsTuves.reconnectResponse.@return.ToString().Equals(Constants.DefaultOkCode))
                    {
                        objReconnectResponse.ResultCode = Constants.DefaultOkCode;
                        objReconnectResponse.ResultDescription = Constants.DefaultOkDescription;
                        objReconnectResponse.Answer = responseReconnectWsTuves.reconnectResponse.answer;
                        objReconnectResponse.ReturnValue = Convert.ToString(responseReconnectWsTuves.reconnectResponse.@return);
                    }
                    else 
                    {
                        var getDescriptionAnswerResponse = _tvCableIntegrationBl.GetDescriptionAnswer(responseReconnectWsTuves.reconnectResponse.@return.ToString());
                        objReconnectResponse.ResultCode = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                        objReconnectResponse.ResultDescription = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                        objReconnectResponse.Answer = responseReconnectWsTuves.reconnectResponse.answer;
                        objReconnectResponse.ReturnValue = Convert.ToString(responseReconnectWsTuves.reconnectResponse.@return);                     
                    }
                    ////////////////////////
                }
                else{
                    objReconnectResponse.ResultCode = validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                    objReconnectResponse.ResultDescription = validateUserResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                    objReconnectResponse.Answer = string.Empty;
                    objReconnectResponse.ReturnValue = string.Empty;               
                }
                /////
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.Reconnect, 
                    "RESPONSE: [ProcessId: " + timeStampProcess + "] [Answer: " + objReconnectResponse.Answer + ". ReturnValue: " + objReconnectResponse.ReturnValue + "]" + 
                    " [ResultCode: " + objReconnectResponse.ResultCode + "]" +
                    " [ResultDescription: " + objReconnectResponse.ResultDescription + "]", Util.ErrorTypeEnum.TraceType);
                /////
                return objReconnectResponse;
            }
            catch (Exception ex)
            {
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.Reconnect, "EXCEPTION: [ProcessId " + timeStampProcess + "] " + ex.ToString(), Util.ErrorTypeEnum.ErrorType);
                objReconnectResponse = new ReconnectResponse();
                objReconnectResponse.ResultCode = Constants.DefaultErrorCode;
                objReconnectResponse.ResultDescription = Constants.DefaultErrorDescription;
                objReconnectResponse.Answer = string.Empty;
                objReconnectResponse.ReturnValue = string.Empty;
                return objReconnectResponse;
            }
        }

        /// <summary>
        /// Permite desinstalar completamente los equipos asignados a un cliente que ha renunciado al producto.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="sequence"></param>
        /// <param name="smartCards"></param>
        /// <param name="userId"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="notes"></param>
        /// <param name="customerId"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        public UninstallResponse Uninstall(string userName, string password, string sequence, string smartCards, string userId, string referenceNumber, 
            string notes, string customerId, string zone)
        {
            var objUninstallResponse = new UninstallResponse();

            timeStampProcess = string.Empty;
            timeStampProcess = _util.GenerateTimeStamp();   

            try
            {
                //CF: Escribimos en el Log para verificar los datos recibidos.                
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.Uninstall, 
                    "REQUEST: [ProcessId: " + timeStampProcess + " ] [UserName: " + userName + ". Password: " + password + ". Sequence: " + sequence + ". SmartCards: " + smartCards + ". UserId: " + userId + ". ReferenceNumber: "
                    + referenceNumber + ". Notes: " + notes + ". CustomerId: " + customerId + ". Zone: " + zone + "]", Util.ErrorTypeEnum.TraceType);

               // var passwordEncriptado = EncriptaMD5(password);
                var validateUserResponse = _tvCableIntegrationBl.VerificaLoginUser(userName, password, Constants.Uninstall);

                  if (validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString().Equals(Constants.DefaultOkCode))
                  {
                      _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.Uninstall, 
                          "VALIDATE USER RESPONSE: [ProcessId: " + timeStampProcess + "] [" + validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString() + "]", Util.ErrorTypeEnum.TraceType);

                      var objUninstall = new uninstall
                      {
                          username = _configuration.UserName,
                          password = _configuration.Password,
                          sequence = int.Parse(sequence),
                          smartcard = Convert.ToDouble(smartCards),
                          user_id = userId,
                          reference_number = referenceNumber,
                          notes = notes,
                          customer_id = customerId,
                          zone = zone
                      };

                      var objuninstallRequest = new uninstallRequest();
                      objuninstallRequest.uninstall = objUninstall;
                      var responseUninstallWsTuves = _wrTuvesService.uninstall(objuninstallRequest);

                      _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.Uninstall, 
                          "RESPONSE TUVES: [ProcessId: " + timeStampProcess + "] [Answer: " + responseUninstallWsTuves.uninstallResponse.answer + ". ReturnValue: " + Convert.ToString(responseUninstallWsTuves.uninstallResponse.@return) + "]", Util.ErrorTypeEnum.TraceType);

                      ///////////////
                      //CF: Verifica si el resultado es igual a 0, caso contrario, devolvemos el resultado del error generico y error generado por Tuves
                      if (responseUninstallWsTuves.uninstallResponse.@return.ToString().Equals(Constants.DefaultOkCode))
                      {
                          objUninstallResponse.ResultCode = Constants.DefaultOkCode;
                          objUninstallResponse.ResultDescription = Constants.DefaultOkDescription;
                          objUninstallResponse.Answer = responseUninstallWsTuves.uninstallResponse.answer;
                          objUninstallResponse.ReturnValue = Convert.ToString(responseUninstallWsTuves.uninstallResponse.@return);
                      }
                      else
                      {
                          var getDescriptionAnswerResponse = _tvCableIntegrationBl.GetDescriptionAnswer(responseUninstallWsTuves.uninstallResponse.@return.ToString());
                          objUninstallResponse.ResultCode = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                          objUninstallResponse.ResultDescription = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                          objUninstallResponse.Answer = responseUninstallWsTuves.uninstallResponse.answer;
                          objUninstallResponse.ReturnValue = Convert.ToString(responseUninstallWsTuves.uninstallResponse.@return);                        
                      }
                      ///////
                  }
                  else {
                      objUninstallResponse.ResultCode = validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                      objUninstallResponse.ResultDescription = validateUserResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                      objUninstallResponse.Answer = string.Empty;
                      objUninstallResponse.ReturnValue = string.Empty;                   
                  }        
                /////
                  _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.Uninstall, 
                      "RESPONSE: [ProcessId: " + timeStampProcess + "] [Answer: " + objUninstallResponse.Answer + ". ReturnValue: " + objUninstallResponse.ReturnValue + "]" + 
                      " [ResultCode: " + objUninstallResponse.ResultCode + "]" + " [ResultDescription: " + objUninstallResponse.ResultDescription + "]", Util.ErrorTypeEnum.TraceType);
                /////
                return objUninstallResponse;
            }
            catch (Exception ex)
            {
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.Uninstall, "EXCEPTION: [ProcessId: " + timeStampProcess + "] " + ex.ToString(), Util.ErrorTypeEnum.ErrorType);
                objUninstallResponse = new UninstallResponse();
                objUninstallResponse.ResultCode = Constants.DefaultErrorCode;
                objUninstallResponse.ResultDescription = Constants.DefaultErrorDescription;
                objUninstallResponse.Answer = string.Empty;
                objUninstallResponse.ReturnValue = string.Empty;
                return objUninstallResponse;
            }
        }

        /// <summary>
        /// Este comando agrega nuevos packs a una SmartCard previamente instalada.
        /// Se debe especificar el número interno de la SmartCard y sus códigos de Packs asociados.
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="smartCards"></param>
        /// <param name="packs"></param>
        /// <param name="userId"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="notes"></param>
        /// <param name="customerId"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        public AddPacksResponse AddPacks(string userName, string password, string sequence, string smartCards, string[] packs, string userId, string referenceNumber, 
            string notes, string customerId, string zone)
        {
            var objAddPacksResponse = new AddPacksResponse();
            timeStampProcess = string.Empty;
            timeStampProcess = _util.GenerateTimeStamp();   
            try
            {
                //CF: Escribimos en el Log para verificar los datos recibidos.
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.AddPacks, 
                    "REQUEST: [ProcessId: " + timeStampProcess + "] [UserName: " + userName + ". Password: " + password + ". Sequence: " + sequence + ". SmartCards: " + smartCards + ". Packs: [" + _util.getStringInObjects(packs) + 
                    "]. UserId: " + userId + ". ReferenceNumber: " + referenceNumber + ". Notes: " + notes + ". CustomerId: " + customerId + ". Zone: " + zone + "]", Util.ErrorTypeEnum.TraceType);

               // var passwordEncriptado = EncriptaMD5(password);
                var validateUserResponse = _tvCableIntegrationBl.VerificaLoginUser(userName, password, Constants.AddPacks);

                if (validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString().Equals(Constants.DefaultOkCode))
                {
                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.AddPacks,
                          "VALIDATE USER RESPONSE: [ProcesId: " + timeStampProcess + "] [" + validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString() + "]", Util.ErrorTypeEnum.TraceType);
                    
                    var objAddPacks = new addPacks
                    {
                        username = _configuration.UserName,
                        password = _configuration.Password,
                        sequence = int.Parse(sequence),
                        smartcard = Convert.ToDouble(smartCards),
                        Packs = packs,
                        user_id = userId,
                        reference_number = referenceNumber,
                        notes = notes,
                        customer_id = customerId,
                        zone = zone
                    };

                    var objaddPacksRequest = new addPacksRequest();
                    objaddPacksRequest.addPacks = objAddPacks;
                    var responseAddPakcsWsTuves = _wrTuvesService.addPacks(objaddPacksRequest);

                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.AddPacks, 
                        "RESPONSE TUVES: [ProcessId: " + timeStampProcess + "] [Answer: " + responseAddPakcsWsTuves.addPacksResponse.answer + ". ReturnValue: " + Convert.ToString(responseAddPakcsWsTuves.addPacksResponse.@return) + "]", Util.ErrorTypeEnum.TraceType);

                    ///////////////
                    //CF: Verifica si el resultado es igual a 0, caso contrario, devolvemos el resultado del error generico y error generado por Tuves
                    if (responseAddPakcsWsTuves.addPacksResponse.@return.ToString().Equals(Constants.DefaultOkCode))
                    {
                        objAddPacksResponse.ResultCode = Constants.DefaultOkCode;
                        objAddPacksResponse.ResultDescription = Constants.DefaultOkDescription;
                        objAddPacksResponse.Answer = responseAddPakcsWsTuves.addPacksResponse.answer;
                        objAddPacksResponse.ReturnValue = Convert.ToString(responseAddPakcsWsTuves.addPacksResponse.@return);
                    }
                    else
                    {
                        var getDescriptionAnswerResponse = _tvCableIntegrationBl.GetDescriptionAnswer(responseAddPakcsWsTuves.addPacksResponse.@return.ToString());
                        objAddPacksResponse.ResultCode = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                        objAddPacksResponse.ResultDescription = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                        objAddPacksResponse.Answer = responseAddPakcsWsTuves.addPacksResponse.answer;
                        objAddPacksResponse.ReturnValue = Convert.ToString(responseAddPakcsWsTuves.addPacksResponse.@return);                      
                    }
                    ////////                   
                }
                else {
                    objAddPacksResponse.ResultCode = validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                    objAddPacksResponse.ResultDescription = validateUserResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                    objAddPacksResponse.Answer = string.Empty;
                    objAddPacksResponse.ReturnValue = string.Empty;                 
                }              
                /////
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.AddPacks, 
                    "RESPONSE: [ProcessId: " + timeStampProcess + "] [Answer: " + objAddPacksResponse.Answer + ". ReturnValue: " + objAddPacksResponse.ReturnValue + "]" + 
                    " [ResultCode: " + objAddPacksResponse.ResultCode + "]" + " [ResultDescription: " + objAddPacksResponse.ResultDescription + "]", Util.ErrorTypeEnum.TraceType);
                /////
                return objAddPacksResponse;
            }
            catch (Exception ex)
            {
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.AddPacks, "EXCEPTION: [ProcessId: " + timeStampProcess + "] " + ex.ToString(), Util.ErrorTypeEnum.ErrorType);
                objAddPacksResponse = new AddPacksResponse();
                objAddPacksResponse.ResultCode = Constants.DefaultErrorCode;
                objAddPacksResponse.ResultDescription = Constants.DefaultErrorDescription;
                objAddPacksResponse.Answer = string.Empty;
                objAddPacksResponse.ReturnValue = string.Empty;
                return objAddPacksResponse;
            }
        }

        /// <summary>
        /// Este comando elimina los Packs especificados de una SmartCard activa.
        /// Se debe especificar el número interno de la SmartCard y los Packs a remover.
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="smartCards"></param>
        /// <param name="packs"></param>
        /// <param name="userId"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="notes"></param>
        /// <param name="customerId"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        public RemovePacksResponse RemovePacks(string userName, string password, string sequence, string smartCards, string[] packs, string userId, string 
            referenceNumber, string notes, string customerId, string zone)
        {
            var objRemovePacksResponse = new RemovePacksResponse();
            timeStampProcess = string.Empty;
            timeStampProcess = _util.GenerateTimeStamp();   
            try
            {
                //CF: Escribimos en el Log para verificar los datos recibidos.
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.RemovePacks, 
                    "REQUEST: [ProcessId: " + timeStampProcess + "] [UserName: " + userName + ". Password: " + password + ". Sequence: " + sequence + ". SmartCards: " + smartCards + ". Packs: [" + _util.getStringInObjects(packs) + "]. UserId: " + 
                    userId + ". ReferenceNumber: " + referenceNumber + ". Notes: " + notes + ". CustomerId: " + customerId + ". Zone: " + zone + "]", Util.ErrorTypeEnum.TraceType);

                //var passwordEncriptado = EncriptaMD5(password);
                  var validateUserResponse = _tvCableIntegrationBl.VerificaLoginUser(userName, password, Constants.RemovePacks);

                  if (validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString().Equals(Constants.DefaultOkCode))
                  {
                      _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.RemovePacks,
                          "VALIDATE USER RESPONSE: [ProcessId " + timeStampProcess + "] [" + validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString() + "]", Util.ErrorTypeEnum.TraceType);
                    
                      var objRemovePacks = new removePacks
                      {
                          username = _configuration.UserName,
                          password = _configuration.Password,
                          sequence = int.Parse(sequence),
                          smartcard = Convert.ToDouble(smartCards),
                          Packs = packs,
                          user_id = userId,
                          reference_number = referenceNumber,
                          notes = notes,
                          customer_id = customerId,
                          zone = zone
                      };
                      var objRemovePacksRequest = new removePacksRequest();
                      objRemovePacksRequest.removePacks = objRemovePacks;
                      var responseRemovePacksWsTuves = _wrTuvesService.removePacks(objRemovePacksRequest);

                      _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.RemovePacks, 
                          "RESPONSE TUVES: [ProcessId: " + timeStampProcess + "] [Answer: " + responseRemovePacksWsTuves.removePacksResponse.answer + ". ReturnValue: " + Convert.ToString(responseRemovePacksWsTuves.removePacksResponse.@return) + "]", Util.ErrorTypeEnum.TraceType);

                      ///////////////
                      //CF: Verifica si el resultado es igual a 0, caso contrario, devolvemos el resultado del error generico y error generado por Tuves
                      if (responseRemovePacksWsTuves.removePacksResponse.@return.ToString().Equals(Constants.DefaultOkCode))
                      {
                          objRemovePacksResponse.ResultCode = Constants.DefaultOkCode;
                          objRemovePacksResponse.ResultDescription = Constants.DefaultOkDescription;
                          objRemovePacksResponse.Answer = responseRemovePacksWsTuves.removePacksResponse.answer;
                          objRemovePacksResponse.ReturnValue = Convert.ToString(responseRemovePacksWsTuves.removePacksResponse.@return);
                      }
                      else
                      {
                          var getDescriptionAnswerResponse = _tvCableIntegrationBl.GetDescriptionAnswer(responseRemovePacksWsTuves.removePacksResponse.@return.ToString());
                          objRemovePacksResponse.ResultCode = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                          objRemovePacksResponse.ResultDescription = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                          objRemovePacksResponse.Answer = responseRemovePacksWsTuves.removePacksResponse.answer;
                          objRemovePacksResponse.ReturnValue = Convert.ToString(responseRemovePacksWsTuves.removePacksResponse.@return);                      
                      }
                      ///////////////////
                  }
                  else {
                      objRemovePacksResponse.ResultCode = validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                      objRemovePacksResponse.ResultDescription = validateUserResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                      objRemovePacksResponse.Answer = string.Empty;
                      objRemovePacksResponse.ReturnValue = string.Empty;                      
                  }
                ////
                  _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.RemovePacks, 
                      "RESPONSE: [ProcessId: " + timeStampProcess + "] [Answer: " + objRemovePacksResponse.Answer + ". ReturnValue: " + objRemovePacksResponse.ReturnValue + "]" + 
                      " [ResultCode: " + objRemovePacksResponse.ResultCode + "]" + " [ResultDescription: " + objRemovePacksResponse.ResultDescription + "]", Util.ErrorTypeEnum.TraceType);
                ////
                return objRemovePacksResponse;
            }
            catch (Exception ex)
            {
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.RemovePacks, "EXCEPTION: [ProcessId: " + timeStampProcess + "]" + ex.ToString(), Util.ErrorTypeEnum.ErrorType);
                objRemovePacksResponse = new RemovePacksResponse();
                objRemovePacksResponse.ResultCode = Constants.DefaultErrorCode;
                objRemovePacksResponse.ResultDescription = Constants.DefaultErrorDescription;
                objRemovePacksResponse.Answer = string.Empty;
                objRemovePacksResponse.ReturnValue = string.Empty;
                return objRemovePacksResponse;
            }
        }

        /// <summary>
        /// Este comando permite reenviar las claves criptográficas a un equipo previamente instalada y habilitada.
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="smartCards"></param>
        /// <param name="userId"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="notes"></param>
        /// <param name="customerId"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        public ReSendKeyResponse ReSendKey(string userName, string password, string sequence, string smartCards, string userId, string referenceNumber, string notes, 
            string customerId, string zone)
        {
            var objReSendKeyResponse = new ReSendKeyResponse();
            timeStampProcess = string.Empty;
            timeStampProcess = _util.GenerateTimeStamp();   
            try
            {
                //CF: Escribimos en el Log para verificar los datos recibidos.
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.ReSendKey, 
                "REQUEST: [ProcessId: " + timeStampProcess + "] [UserName: " + userName + ". Password: " + password + ". Sequence: " + sequence + ". SmartCards: " + smartCards + ". UserId: " + userId + ". ReferenceNumber: "
                + referenceNumber + ". Notes: " + notes + ". CustomerId: " + customerId + ". Zone: " + zone + "]", Util.ErrorTypeEnum.TraceType);

               // var passwordEncriptado = EncriptaMD5(password);
               var validateUserResponse = _tvCableIntegrationBl.VerificaLoginUser(userName, password, Constants.ReSendKey);

               if (validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString().Equals(Constants.DefaultOkCode))
               {
                   _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.ReSendKey,
                          "VALIDATE USER RESPONSE: [ProcessId: " + timeStampProcess + "] [" + validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString() + "]", Util.ErrorTypeEnum.TraceType);
                    
                   var objReSendKey = new reSendKey
                   {
                       username = _configuration.UserName,
                       password = _configuration.Password,
                       sequence = int.Parse(sequence),
                       smartcard = Convert.ToDouble(smartCards),
                       user_id = userId,
                       reference_number = referenceNumber,
                       notes = notes,
                       customer_id = customerId,
                       zone = zone
                   };

                   var objreSendKeyRequest = new reSendKeyRequest();
                   objreSendKeyRequest.reSendKey = objReSendKey;

                   var responseReSendKeyWsTuves = _wrTuvesService.reSendKey(objreSendKeyRequest);

                   _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.ReSendKey, 
                       "RESPONSE TUVES: [ProcessId: " + timeStampProcess + "] [Answer: " + responseReSendKeyWsTuves.reSendKeyResponse.answer + ". ReturnValue: " + Convert.ToString(responseReSendKeyWsTuves.reSendKeyResponse.@return) + "]", Util.ErrorTypeEnum.TraceType);

                      ///////////////
                       //CF: Verifica si el resultado es igual a 0, caso contrario, devolvemos el resultado del error generico y error generado por Tuves
                   if (responseReSendKeyWsTuves.reSendKeyResponse.@return.ToString().Equals(Constants.DefaultOkCode))
                   {
                       objReSendKeyResponse.ResultCode = Constants.DefaultOkCode;
                       objReSendKeyResponse.ResultDescription = Constants.DefaultOkDescription;
                       objReSendKeyResponse.Answer = responseReSendKeyWsTuves.reSendKeyResponse.answer;
                       objReSendKeyResponse.ReturnValue = Convert.ToString(responseReSendKeyWsTuves.reSendKeyResponse.@return);
                   }
                   else 
                   {
                       var getDescriptionAnswerResponse = _tvCableIntegrationBl.GetDescriptionAnswer(responseReSendKeyWsTuves.reSendKeyResponse.@return.ToString());
                       objReSendKeyResponse.ResultCode = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                       objReSendKeyResponse.ResultDescription = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                       objReSendKeyResponse.Answer = responseReSendKeyWsTuves.reSendKeyResponse.answer;
                       objReSendKeyResponse.ReturnValue = Convert.ToString(responseReSendKeyWsTuves.reSendKeyResponse.@return);                     
                   }
                   ///////////////////////                  
               }
               else {
                   objReSendKeyResponse.ResultCode = validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                   objReSendKeyResponse.ResultDescription = validateUserResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                   objReSendKeyResponse.Answer = string.Empty;
                   objReSendKeyResponse.ReturnValue = string.Empty;                   
               }
                ////
               _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.ReSendKey, 
                   "RESPONSE: [ProcessId: " + timeStampProcess + "] [Answer: " + objReSendKeyResponse.Answer + ". ReturnValue: " + objReSendKeyResponse.ReturnValue + "]" + 
                   " [ResultCode: " + objReSendKeyResponse.ResultCode + "]" + " [ResultDescription: " + objReSendKeyResponse.ResultDescription + "]", Util.ErrorTypeEnum.TraceType);
                ////
                return objReSendKeyResponse;
            }
            catch (Exception ex)
            {
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.ReSendKey, "EXCEPTION: [ProcessId: " + timeStampProcess + "] " + ex.ToString(), Util.ErrorTypeEnum.ErrorType);
                objReSendKeyResponse = new ReSendKeyResponse();
                objReSendKeyResponse.ResultCode = Constants.DefaultErrorCode;
                objReSendKeyResponse.ResultDescription = Constants.DefaultErrorDescription;
                objReSendKeyResponse.Answer = String.Empty;
                objReSendKeyResponse.ReturnValue = string.Empty;
                return objReSendKeyResponse;
            }
        }

        /// <summary>
        /// Este comando permite cambiar el código de control de acceso de la smartcard previamente instalada y habilitado. 
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="smartCards"></param>
        /// <param name="pin"></param>
        /// <param name="userId"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="notes"></param>
        /// <param name="customerId"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        public UpdatePinResponse UpdatePin(string userName, string password, string sequence, string smartCards, string pin, string userId, string referenceNumber, 
            string notes, string customerId, string zone)
        {
            var objUpdatePinResponse = new UpdatePinResponse();
            timeStampProcess = string.Empty;
            timeStampProcess = _util.GenerateTimeStamp();   
            try
            {
                //CF: Escribimos en el Log para verificar los datos recibidos.
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.UpdatePin, 
                    "REQUEST: [ProcessId: " + timeStampProcess + "] [UserName: " + userName + ". Password: " + password + ". Sequence: " + sequence + ". SmartCards: " + smartCards + ". Pin: " + pin +  
                    ". UserId: " + userId + ". ReferenceNumber: " + referenceNumber + ". Notes: " + notes + ". CustomerId: " + customerId + ". Zone: " + zone + "]", Util.ErrorTypeEnum.TraceType);

                  // var passwordEncriptado = EncriptaMD5(password);
                   var validateUserResponse = _tvCableIntegrationBl.VerificaLoginUser(userName, password, Constants.UpdatePin);

                   if (validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString().Equals(Constants.DefaultOkCode))
                   {
                       _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.UpdatePin,
                          "VALIDATE USER RESPONSE: [ProcessId: " + timeStampProcess + "] [" + validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString() + "]", Util.ErrorTypeEnum.TraceType);

                       var objUpdatePin = new updatePIN
                       {
                           username = _configuration.UserName,
                           password = _configuration.Password,
                           sequence = int.Parse(sequence),
                           smartcard = Convert.ToDouble(smartCards),
                           pin = pin,
                           user_id = userId,
                           reference_number = referenceNumber,
                           notes = notes,
                           customer_id = customerId,
                           zone = zone
                       };
                       var objupdatePinRequest = new updatePINRequest();
                       objupdatePinRequest.updatePIN = objUpdatePin;

                       var responseUpdatePinWsTuves = _wrTuvesService.updatePIN(objupdatePinRequest);

                       _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.UpdatePin, 
                           "RESPONSE TUVES: [ProcessId: " + timeStampProcess + "] [Answer: " + responseUpdatePinWsTuves.updatePINResponse.answer + ". ReturnValue: " + Convert.ToString(responseUpdatePinWsTuves.updatePINResponse.@return) + "]", Util.ErrorTypeEnum.TraceType);

                       ///////////////
                       //CF: Verifica si el resultado es igual a 0, caso contrario, devolvemos el resultado del error generico y error generado por Tuves
                       if (responseUpdatePinWsTuves.updatePINResponse.@return.ToString().Equals(Constants.DefaultOkCode))
                       {
                               objUpdatePinResponse.ResultCode = Constants.DefaultOkCode;
                               objUpdatePinResponse.ResultDescription = Constants.DefaultOkDescription;
                               objUpdatePinResponse.Answer = responseUpdatePinWsTuves.updatePINResponse.answer;
                               objUpdatePinResponse.ReturnValue = Convert.ToString(responseUpdatePinWsTuves.updatePINResponse.@return);
                       }
                       else
                       {
                            var getDescriptionAnswerResponse = _tvCableIntegrationBl.GetDescriptionAnswer(responseUpdatePinWsTuves.updatePINResponse.@return.ToString());
                            objUpdatePinResponse.ResultCode = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                            objUpdatePinResponse.ResultDescription = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                            objUpdatePinResponse.Answer = responseUpdatePinWsTuves.updatePINResponse.answer;
                            objUpdatePinResponse.ReturnValue = Convert.ToString(responseUpdatePinWsTuves.updatePINResponse.@return);                         
                       }
                       //////////////////////////
                   }
                   else {
                       objUpdatePinResponse.ResultCode = validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                       objUpdatePinResponse.ResultDescription = validateUserResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                       objUpdatePinResponse.Answer = string.Empty;
                       objUpdatePinResponse.ReturnValue = string.Empty;            
                   }
                ///////
                   _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.UpdatePin, 
                       "RESPONSE: [ProcessId: " + timeStampProcess + "] [Answer: " + objUpdatePinResponse.Answer + ". ReturnValue: " + objUpdatePinResponse.ReturnValue + "]" 
                       + " [ResultCode: " + objUpdatePinResponse.ResultCode + "]" + " [ResultDescription: " + objUpdatePinResponse.ResultDescription + "]", Util.ErrorTypeEnum.TraceType);
                ///////
                return objUpdatePinResponse;
            }
            catch (Exception ex)
            {
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.UpdatePin, "EXCEPTION: [ProcessId: " + timeStampProcess + "] " + ex.ToString(), Util.ErrorTypeEnum.ErrorType);
                objUpdatePinResponse = new UpdatePinResponse();
                objUpdatePinResponse.ResultCode = Constants.DefaultErrorCode;
                objUpdatePinResponse.ResultDescription = Constants.DefaultErrorDescription;
                objUpdatePinResponse.Answer = string.Empty;
                objUpdatePinResponse.ReturnValue = string.Empty;
                return objUpdatePinResponse;
            }
        }

        /// <summary>
        /// Este comando permite enviar un mensaje a la Pantalla de la Televisión conectada al SetTopBox del cliente final.
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="smartCards"></param>
        /// <param name="message"></param>
        /// <param name="userId"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="notes"></param>
        /// <param name="customerId"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        public SendMessageResponse SendMessage(string userName, string password, string sequence, string smartCards, string message, string userId, string referenceNumber, 
            string notes, string customerId, string zone)
        {
            var objSendMessageResponse = new SendMessageResponse();
            timeStampProcess = string.Empty;
            timeStampProcess = _util.GenerateTimeStamp();   
            try
            {
                //CF: Escribimos en el Log para verificar los datos recibidos.
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.SendMessage, 
                    "REQUEST: [ProcessId: " + timeStampProcess + "] [UserName: " + userName + ". Password: " + password + ". Sequence: " + sequence + ". SmartCards: " + smartCards + ". Message: " + message 
                    + ". UserId: " + userId + ". ReferenceNumber: " + referenceNumber + ". Notes: " + notes + ". CustomerId: " + customerId + ". Zone: " + zone + "]", Util.ErrorTypeEnum.TraceType);

               // var passwordEncriptado = EncriptaMD5(password);
                var validateUserResponse = _tvCableIntegrationBl.VerificaLoginUser(userName, password, Constants.SendMessage);

                if (validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString().Equals(Constants.DefaultOkCode))
                {
                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.SendMessage,
                          "VALIDATE USER RESPONSE: [ProcessId: " + timeStampProcess + "] [" + validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString() + "]", Util.ErrorTypeEnum.TraceType);

                    var objSendMessage = new sendMessage
                    {
                        username = _configuration.UserName,
                        password = _configuration.Password,
                        sequence = int.Parse(sequence),
                        smartcard = Convert.ToDouble(smartCards),
                        message = message,
                        user_id = userId,
                        reference_number = referenceNumber,
                        notes = notes,
                        customer_id = customerId,
                        zone = zone
                    };

                    var objsendMessageRequest = new sendMessageRequest();
                    objsendMessageRequest.sendMessage = objSendMessage;

                    var responseSendMessageWsTuves = _wrTuvesService.sendMessage(objsendMessageRequest);

                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.SendMessage, 
                        "RESPONSE TUVES: [ProcessId: " + timeStampProcess + "] [Answer: " + responseSendMessageWsTuves.sendMessageResponse.answer + ". ReturnValue: " + Convert.ToString(responseSendMessageWsTuves.sendMessageResponse.@return) + "]", Util.ErrorTypeEnum.TraceType);

                      ///////////////
                       //CF: Verifica si el resultado es igual a 0, caso contrario, devolvemos el resultado del error generico y error generado por Tuves
                    if (responseSendMessageWsTuves.sendMessageResponse.@return.ToString().Equals(Constants.DefaultOkCode))
                    {
                        objSendMessageResponse.ResultCode = Constants.DefaultOkCode;
                        objSendMessageResponse.ResultDescription = Constants.DefaultOkDescription;
                        objSendMessageResponse.Answer = responseSendMessageWsTuves.sendMessageResponse.answer;
                        objSendMessageResponse.ReturnValue = Convert.ToString(responseSendMessageWsTuves.sendMessageResponse.@return);
                    }
                    else
                    {
                        var getDescriptionAnswerResponse = _tvCableIntegrationBl.GetDescriptionAnswer(responseSendMessageWsTuves.sendMessageResponse.@return.ToString());
                        objSendMessageResponse.ResultCode = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                        objSendMessageResponse.ResultDescription = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                        objSendMessageResponse.Answer = responseSendMessageWsTuves.sendMessageResponse.answer;
                        objSendMessageResponse.ReturnValue = Convert.ToString(responseSendMessageWsTuves.sendMessageResponse.@return);                
                    }
                    ////////////////
                }
                else {
                    objSendMessageResponse.ResultCode = validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                    objSendMessageResponse.ResultDescription = validateUserResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                    objSendMessageResponse.Answer = string.Empty;
                    objSendMessageResponse.ReturnValue = string.Empty;               
                }
                //////
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.SendMessage, 
                    "RESPONSE: [ProcessId: " + timeStampProcess + "] [Answer: " + objSendMessageResponse.Answer + ". ReturnValue: " + objSendMessageResponse.ReturnValue + "]" + 
                    " [ResultCode: " + objSendMessageResponse.ResultCode + "]" + " [ResultDescription: " + objSendMessageResponse.ResultDescription + "]", Util.ErrorTypeEnum.TraceType);
                //////
                return objSendMessageResponse;

            }
            catch (Exception ex)
            {
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.SendMessage, "EXCEPTION: [ProcessId: " + timeStampProcess + "] " + ex.ToString(), Util.ErrorTypeEnum.ErrorType);
                objSendMessageResponse = new SendMessageResponse();
                objSendMessageResponse.ResultCode = Constants.DefaultErrorCode;
                objSendMessageResponse.ResultDescription = Constants.DefaultErrorDescription;
                objSendMessageResponse.Answer = string.Empty;
                objSendMessageResponse.ReturnValue = string.Empty;
                return objSendMessageResponse;
            }
        }

        /// <summary>
        /// Este comando enviará una orden al SetTopBox asociado a la SmartCard especificada en el comando que realice la actualización a la última versión del software.
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="smartCards"></param>
        /// <param name="userId"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="notes"></param>
        /// <param name="customerId"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        public UpdateFirmwareResponse UpdateFirmware(string userName, string password, string sequence, string smartCards, string userId, string referenceNumber, 
            string notes, string customerId, string zone)
        {
            var objUpdateFirmwareResponse = new UpdateFirmwareResponse();
            try
            {
                //CF: Escribimos en el Log para verificar los datos recibidos.
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.UpdateFirmware, 
                    "REQUEST: [ProcessId: " + timeStampProcess + "] [UserName: " + userName + ". Password: " + password + ". Sequence: " + sequence + ". SmartCards: " + smartCards + ". UserId: " + userId + ". ReferenceNumber: "
                + referenceNumber + ". Notes: " + notes + ". CustomerId: " + customerId + ". Zone: " + zone + "]", Util.ErrorTypeEnum.TraceType);

               // var passwordEncriptado = EncriptaMD5(password);
                var validateUserResponse = _tvCableIntegrationBl.VerificaLoginUser(userName, password, Constants.UpdateFirmware);

                if (validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString().Equals(Constants.DefaultOkCode))
                {
                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.UpdateFirmware,
                      "VALIDATE USER RESPONSE: [ProcessId: " + timeStampProcess + "] [" + validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString() + "]", Util.ErrorTypeEnum.TraceType);

                    var objUpdateFirmware = new updateFirmware
                    {
                        username = _configuration.UserName,
                        password = _configuration.Password,
                        sequence = int.Parse(sequence),
                        smartcard = Convert.ToDouble(smartCards),
                        user_id = userId,
                        reference_number = referenceNumber,
                        notes = notes,
                        customer_id = customerId,
                        zone = zone
                    };
                    var objUpdateFirmwareRequest = new updateFirmwareRequest();
                    objUpdateFirmwareRequest.updateFirmware = objUpdateFirmware;

                    var responseUpdateFirmwareWsTuves = _wrTuvesService.updateFirmware(objUpdateFirmwareRequest);

                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.UpdateFirmware,
                        "RESPONSE TUVES: [ProcessId: " + timeStampProcess + "] [Answer: " + responseUpdateFirmwareWsTuves.updateFirmwareResponse.answer + ". ReturnValue: " + Convert.ToString(responseUpdateFirmwareWsTuves.updateFirmwareResponse.@return) + "]", Util.ErrorTypeEnum.TraceType);

                    ///////////////
                    //CF: Verifica si el resultado es igual a 0, caso contrario, devolvemos el resultado del error generico y error generado por Tuves
                    if (responseUpdateFirmwareWsTuves.updateFirmwareResponse.@return.ToString().Equals(Constants.DefaultOkCode))
                    {
                        objUpdateFirmwareResponse.ResultCode = Constants.DefaultOkCode;
                        objUpdateFirmwareResponse.ResultDescription = Constants.DefaultOkDescription;
                        objUpdateFirmwareResponse.Answer = responseUpdateFirmwareWsTuves.updateFirmwareResponse.answer;
                        objUpdateFirmwareResponse.ReturnValue = Convert.ToString(responseUpdateFirmwareWsTuves.updateFirmwareResponse.@return);
                    }
                    else
                    {
                        var getDescriptionAnswerResponse = _tvCableIntegrationBl.GetDescriptionAnswer(responseUpdateFirmwareWsTuves.updateFirmwareResponse.@return.ToString());
                        objUpdateFirmwareResponse.ResultCode = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                        objUpdateFirmwareResponse.ResultDescription = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                        objUpdateFirmwareResponse.Answer = responseUpdateFirmwareWsTuves.updateFirmwareResponse.answer;
                        objUpdateFirmwareResponse.ReturnValue = Convert.ToString(responseUpdateFirmwareWsTuves.updateFirmwareResponse.@return);                      
                    }
                    ///////////                   
                }
                else {
                    objUpdateFirmwareResponse.ResultCode = validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                    objUpdateFirmwareResponse.ResultDescription = validateUserResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                    objUpdateFirmwareResponse.Answer = string.Empty;
                    objUpdateFirmwareResponse.ReturnValue = string.Empty;                    
                }
                ////
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.UpdateFirmware, 
                    "RESPONSE: [ProcessId: " + timeStampProcess + "] [Answer: " + objUpdateFirmwareResponse.Answer + ". ReturnValue: " + objUpdateFirmwareResponse.ReturnValue + "]" + 
                    " [ResultCode: " + objUpdateFirmwareResponse.ResultCode + "]" + " [ResultDescription: " + objUpdateFirmwareResponse.ResultDescription + "]", Util.ErrorTypeEnum.TraceType);
                ////
                return objUpdateFirmwareResponse;
            }
            catch (Exception ex)
            {
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.UpdateFirmware, "EXCEPTION: [ProcessId: " + timeStampProcess + "]" + ex.ToString(), Util.ErrorTypeEnum.ErrorType);
                objUpdateFirmwareResponse = new UpdateFirmwareResponse();
                objUpdateFirmwareResponse.ResultCode = Constants.DefaultErrorCode;
                objUpdateFirmwareResponse.ResultDescription = Constants.DefaultErrorDescription;
                objUpdateFirmwareResponse.Answer = string.Empty;
                objUpdateFirmwareResponse.ReturnValue = string.Empty;
                return objUpdateFirmwareResponse;
            }
        }

        /// <summary>
        /// Esta operación ejecutará, en el settopbox de la smartcard ingresada, un refresco de los canales.
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="smartCards"></param>
        /// <param name="userId"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="notes"></param>
        /// <param name="customerId"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        public ReScanChannelsResponse ReScanChannels(string userName, string password, string sequence, string smartCards, string userId, string referenceNumber, 
            string notes, string customerId, string zone)
        {
            var objReScanChannelsResponse = new ReScanChannelsResponse();
            timeStampProcess = string.Empty;
            timeStampProcess = _util.GenerateTimeStamp();   
            try{
                //CF: Escribimos en el Log para verificar los datos recibidos.
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.ReScanChannels, 
                    "REQUEST: [ProcessId: " + timeStampProcess + "] [UserName: " + userName + ". Password: " + password + ". Sequence: " + sequence + ". SmartCards: " + smartCards + ". UserId: " + userId + ". ReferenceNumber: "
                    + referenceNumber + ". Notes: " + notes + ". CustomerId: " + customerId + ". Zone: " + zone + "]", Util.ErrorTypeEnum.TraceType);

               // var passwordEncriptado = EncriptaMD5(password);
                var validateUserResponse = _tvCableIntegrationBl.VerificaLoginUser(userName, password, Constants.ReScanChannels);

                if (validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString().Equals(Constants.DefaultOkCode))
                {
                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.ReScanChannels,
                    "VALIDATE USER RESPONSE: [ProcessId " + timeStampProcess + "] [" + validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString() + "]", Util.ErrorTypeEnum.TraceType);

                    var objReScanChannels = new rescanChannels
                    {
                        username = _configuration.UserName,
                        password = _configuration.Password,
                        sequence = int.Parse(sequence),
                        smartcard = Convert.ToDouble(smartCards),
                        user_id = userId,
                        reference_number = referenceNumber,
                        notes = notes,
                        customer_id = customerId,
                        zone = zone
                    };
                    var objReScanChannelsRequest = new rescanChannelsRequest();
                    objReScanChannelsRequest.rescanChannels = objReScanChannels;

                    var responseReScanChannelsWsTuves = _wrTuvesService.rescanChannels(objReScanChannelsRequest);

                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.ReScanChannels, 
                        "RESPONSE TUVES: [ProcessId: " + timeStampProcess + "] [Answer: " + responseReScanChannelsWsTuves.rescanChannelsResponse.answer + ". ReturnValue: " + Convert.ToString(responseReScanChannelsWsTuves.rescanChannelsResponse.@return) + "]", Util.ErrorTypeEnum.TraceType);

                    ///////////////
                    //CF: Verifica si el resultado es igual a 0, caso contrario, devolvemos el resultado del error generico y error generado por Tuves
                    if (responseReScanChannelsWsTuves.rescanChannelsResponse.@return.ToString().Equals(Constants.DefaultOkCode))
                    {
                        objReScanChannelsResponse.ResultCode = Constants.DefaultOkCode;
                        objReScanChannelsResponse.ResultDescription = Constants.DefaultOkDescription;
                        objReScanChannelsResponse.Answer = responseReScanChannelsWsTuves.rescanChannelsResponse.answer;
                        objReScanChannelsResponse.ReturnValue = Convert.ToString(responseReScanChannelsWsTuves.rescanChannelsResponse.@return);
                    }
                    else
                    {
                        var getDescriptionAnswerResponse = _tvCableIntegrationBl.GetDescriptionAnswer(responseReScanChannelsWsTuves.rescanChannelsResponse.@return.ToString());
                        objReScanChannelsResponse.ResultCode = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                        objReScanChannelsResponse.ResultDescription = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                        objReScanChannelsResponse.Answer = responseReScanChannelsWsTuves.rescanChannelsResponse.answer;
                        objReScanChannelsResponse.ReturnValue = Convert.ToString(responseReScanChannelsWsTuves.rescanChannelsResponse.@return);     
                    }
                    /////////////////                    
                }
                else {
                    objReScanChannelsResponse.ResultCode = validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                    objReScanChannelsResponse.ResultDescription = validateUserResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                    objReScanChannelsResponse.Answer = string.Empty;
                    objReScanChannelsResponse.ReturnValue = string.Empty;                    
                }
                ////
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.ReScanChannels, 
                    "RESPONSE: [ProcessId: " + timeStampProcess + "] [Answer: " + objReScanChannelsResponse.Answer + ". ReturnValue: " + objReScanChannelsResponse.ReturnValue + "]" + 
                    " [ResultCode: " + objReScanChannelsResponse.ResultCode + "]" + " [ResultDescription: " + objReScanChannelsResponse.ResultDescription + "]", Util.ErrorTypeEnum.TraceType);
                ////
                return objReScanChannelsResponse;
            }
            catch (Exception ex) {
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.ReScanChannels, "EXCEPTION: [ProcessId: " + timeStampProcess + "] " + ex.ToString(), Util.ErrorTypeEnum.ErrorType);
                objReScanChannelsResponse = new ReScanChannelsResponse();
                objReScanChannelsResponse.ResultCode = Constants.DefaultErrorCode;
                objReScanChannelsResponse.ResultDescription = Constants.DefaultErrorDescription;
                objReScanChannelsResponse.Answer = string.Empty;
                objReScanChannelsResponse.ReturnValue = string.Empty;
                return objReScanChannelsResponse;
            }
        }

        /// <summary>
        /// Esta operación agregará, en el settopbox de la smartcard ingresada, una nueva portadora de señales.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="sequence"></param>
        /// <param name="smartCards"></param>
        /// <param name="carrier"></param>
        /// <param name="userId"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="notes"></param>
        /// <param name="customerId"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        public AddCarrierResponse AddCarrier(string userName, string password, string sequence, string smartCards, string carrier, string userId, string referenceNumber, 
            string notes, string customerId, string zone) {
            
            var objAddCarrierResponse = new AddCarrierResponse();
            timeStampProcess = string.Empty;
            timeStampProcess = _util.GenerateTimeStamp();   
            try
            {
                //CF: Escribimos en el Log para verificar los datos recibidos.
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.AddCarrier, 
                    "REQUEST: [ProcessId: " + timeStampProcess + "] [UserName: " + userName + ". Password: " + password + ". Sequence: " + sequence + ". SmartCards: " + smartCards + ". Carrier: " + carrier + ". UserId: " + userId + ". ReferenceNumber: "
                    + referenceNumber + ". Notes: " + notes + ". CustomerId: " + customerId + ". Zone: " + zone + "]", Util.ErrorTypeEnum.TraceType);

               // var passwordEncriptado = EncriptaMD5(password);
                var validateUserResponse = _tvCableIntegrationBl.VerificaLoginUser(userName, password, Constants.AddCarrier);

                if (validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString().Equals(Constants.DefaultOkCode))
                {
                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.AddCarrier,
                     "VALIDATE USER RESPONSE: [ProcessId: " + timeStampProcess + "] [" + validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString() + "]", Util.ErrorTypeEnum.TraceType);

                    var objAddCarrier = new addCarrier
                    {
                        username = _configuration.UserName,
                        password = _configuration.Password,
                        sequence = int.Parse(sequence),
                        smartcard = Convert.ToDouble(smartCards),
                        carrier = int.Parse(carrier),
                        user_id = userId,
                        reference_number = referenceNumber,
                        notes = notes,
                        customer_id = customerId,
                        zone = zone
                    };
                    var objAddCarrierRequest = new addCarrierRequest();
                    objAddCarrierRequest.addCarrier = objAddCarrier;

                    var responseAddCarrierWsTuves = _wrTuvesService.addCarrier(objAddCarrierRequest);

                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.AddCarrier, 
                        "RESPONSE TUVES: [ProcessId: " + timeStampProcess + "] [Answer: " + responseAddCarrierWsTuves.addCarrierResponse.answer + ". ReturnValue: " + Convert.ToString(responseAddCarrierWsTuves.addCarrierResponse.@return) + "]", Util.ErrorTypeEnum.TraceType);

                    ///////////////
                    //CF: Verifica si el resultado es igual a 0, caso contrario, devolvemos el resultado del error generico y error generado por Tuves
                    if (responseAddCarrierWsTuves.addCarrierResponse.@return.ToString().Equals(Constants.DefaultOkCode))
                    {
                        objAddCarrierResponse.ResultCode = Constants.DefaultOkCode;
                        objAddCarrierResponse.ResultDescription = Constants.DefaultOkDescription;
                        objAddCarrierResponse.Answer = responseAddCarrierWsTuves.addCarrierResponse.answer;
                        objAddCarrierResponse.ReturnValue = Convert.ToString(responseAddCarrierWsTuves.addCarrierResponse.@return);
                    }
                    else
                    {
                        var getDescriptionAnswerResponse = _tvCableIntegrationBl.GetDescriptionAnswer(responseAddCarrierWsTuves.addCarrierResponse.@return.ToString());
                        objAddCarrierResponse.ResultCode = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                        objAddCarrierResponse.ResultDescription = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                        objAddCarrierResponse.Answer = responseAddCarrierWsTuves.addCarrierResponse.answer;
                        objAddCarrierResponse.ReturnValue = Convert.ToString(responseAddCarrierWsTuves.addCarrierResponse.@return);                    
                    }
                 //////////////
                }
                else 
                {
                    objAddCarrierResponse.ResultCode = validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                    objAddCarrierResponse.ResultDescription = validateUserResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                    objAddCarrierResponse.Answer = string.Empty;
                    objAddCarrierResponse.ReturnValue = string.Empty;
                }             
                ////
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.AddCarrier, 
                    "RESPONSE: [ProcessId: " + timeStampProcess + "] [Answer: " + objAddCarrierResponse.Answer + ". ReturnValue: " + objAddCarrierResponse.ReturnValue + "]" 
                    + " [ResultCode: " + objAddCarrierResponse.ResultCode + "]" + " [ResultDescription: " + objAddCarrierResponse.ResultDescription + "]", Util.ErrorTypeEnum.TraceType);
                ////
                return objAddCarrierResponse;
            }
            catch(Exception ex) {
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.AddCarrier, "EXCEPTION: [ProcessId: " + timeStampProcess + "] " + ex.ToString(), Util.ErrorTypeEnum.ErrorType);
                objAddCarrierResponse = new AddCarrierResponse();
                objAddCarrierResponse.ResultCode = Constants.DefaultErrorCode;
                objAddCarrierResponse.ResultDescription = Constants.DefaultErrorDescription;
                objAddCarrierResponse.Answer = string.Empty;
                objAddCarrierResponse.ReturnValue = string.Empty;
                return objAddCarrierResponse;
            }
        }

        /// <summary>
        /// Esta operación enviará por pantalla del televisor conectado al settopbox de la smartcard ingresada, un mensaje con el número único de smartcard y el serial identificador del settopbox.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="sequence"></param>
        /// <param name="smartCards"></param>
        /// <param name="userId"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="notes"></param>
        /// <param name="customerId"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        public ViewSetTopBoxIDResponse ViewSetTopBoxID(string userName, string password, string sequence, string smartCards, string userId, string referenceNumber, 
            string notes, string customerId, string zone) {
            
            var objViewSetTopBoxIdResponse = new ViewSetTopBoxIDResponse();
            timeStampProcess = string.Empty;
            timeStampProcess = _util.GenerateTimeStamp();   
            try
            {
                //CF: Escribimos en el Log para verificar los datos recibidos.
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.ViewSetTopBoxID, 
                    "REQUEST: [ProcessId: " + timeStampProcess + "] [UserName: " + userName + ". Password: " + password + ". Sequence: " + sequence + ". SmartCards: " + smartCards + ". UserId: " + userId + ". ReferenceNumber: "
                    + referenceNumber + ". Notes: " + notes + ". CustomerId: " + customerId + ". Zone: " + zone + "]", Util.ErrorTypeEnum.TraceType);

                //var passwordEncriptado = EncriptaMD5(password);
                var validateUserResponse = _tvCableIntegrationBl.VerificaLoginUser(userName, password, Constants.ViewSetTopBoxID);

                if (validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString().Equals(Constants.DefaultOkCode))
                {
                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.ViewSetTopBoxID,
                    "VALIDATE USER RESPONSE: [ProcessId: " + timeStampProcess + "] [" + validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString() + "]", Util.ErrorTypeEnum.TraceType);

                    var objViewSetTopBoxId = new viewSetTopBoxID
                    {
                        username = _configuration.UserName,
                        password = _configuration.Password,
                        sequence = int.Parse(sequence),
                        smartcard = Convert.ToDouble(smartCards),
                        user_id = userId,
                        reference_number = referenceNumber,
                        notes = notes,
                        customer_id = customerId,
                        zone = zone
                    };
                    var objViewSetTopBoxIdRequest = new viewSetTopBoxIDRequest();
                    objViewSetTopBoxIdRequest.viewSetTopBoxID = objViewSetTopBoxId;

                    var responseViewSetTopBoxIDWsTuves = _wrTuvesService.viewSetTopBoxID(objViewSetTopBoxIdRequest);

                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.ViewSetTopBoxID, 
                        "RESPONSE TUVES: [ProcessId: " + timeStampProcess + "] [Answer: " + responseViewSetTopBoxIDWsTuves.viewSetTopBoxIDResponse.answer + ". ReturnValue: " + Convert.ToString(responseViewSetTopBoxIDWsTuves.viewSetTopBoxIDResponse.@return) + "]", Util.ErrorTypeEnum.TraceType);

                    ///////////////
                    //CF: Verifica si el resultado es igual a 0, caso contrario, devolvemos el resultado del error generico y error generado por Tuves
                    if (responseViewSetTopBoxIDWsTuves.viewSetTopBoxIDResponse.@return.ToString().Equals(Constants.DefaultOkCode))
                    {
                        objViewSetTopBoxIdResponse.ResultCode = Constants.DefaultOkCode;
                        objViewSetTopBoxIdResponse.ResultDescription = Constants.DefaultOkDescription;
                        objViewSetTopBoxIdResponse.Answer = responseViewSetTopBoxIDWsTuves.viewSetTopBoxIDResponse.answer;
                        objViewSetTopBoxIdResponse.ReturnValue = Convert.ToString(responseViewSetTopBoxIDWsTuves.viewSetTopBoxIDResponse.@return);
                    }
                    else
                    {
                        var getDescriptionAnswerResponse = _tvCableIntegrationBl.GetDescriptionAnswer(responseViewSetTopBoxIDWsTuves.viewSetTopBoxIDResponse.@return.ToString());
                        objViewSetTopBoxIdResponse.ResultCode = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                        objViewSetTopBoxIdResponse.ResultDescription = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                        objViewSetTopBoxIdResponse.Answer = responseViewSetTopBoxIDWsTuves.viewSetTopBoxIDResponse.answer;
                        objViewSetTopBoxIdResponse.ReturnValue = Convert.ToString(responseViewSetTopBoxIDWsTuves.viewSetTopBoxIDResponse.@return);                      
                    }
                    /////////////                    
                }
                else
                {
                    objViewSetTopBoxIdResponse.ResultCode = validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                    objViewSetTopBoxIdResponse.ResultDescription = validateUserResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                    objViewSetTopBoxIdResponse.Answer = string.Empty;
                    objViewSetTopBoxIdResponse.ReturnValue = string.Empty;                
                }
                /////
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.ViewSetTopBoxID, 
                    "RESPONSE: [ProcessId: " + timeStampProcess + "] [Answer: " + objViewSetTopBoxIdResponse.Answer + ". ReturnValue: " + objViewSetTopBoxIdResponse.ReturnValue + "]" + 
                    " [ResultCode: " + objViewSetTopBoxIdResponse.ResultCode + "]" + " [ResultDescription: " + objViewSetTopBoxIdResponse.ResultDescription + "]", Util.ErrorTypeEnum.TraceType);
                /////            
                return objViewSetTopBoxIdResponse;
            }
            catch (Exception ex) {
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.ViewSetTopBoxID, "EXCEPTION: [ProcessId: " + timeStampProcess + "] " + ex.ToString(), Util.ErrorTypeEnum.ErrorType);
                objViewSetTopBoxIdResponse = new ViewSetTopBoxIDResponse();
                objViewSetTopBoxIdResponse.ResultCode = Constants.DefaultErrorCode;
                objViewSetTopBoxIdResponse.ResultDescription = Constants.DefaultErrorDescription;
                objViewSetTopBoxIdResponse.Answer = string.Empty;
                objViewSetTopBoxIdResponse.ReturnValue = string.Empty;
                return objViewSetTopBoxIdResponse;
            }
        
        }

        /// <summary>
        /// Esta operación borrará, en el settopbox de la smartcard ingresada, cualquier información ingresada por el usuario. 
        /// Restaura el settopbox a valores por defecto del último software instalado.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="sequence"></param>
        /// <param name="smartCards"></param>
        /// <param name="userId"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="notes"></param>
        /// <param name="customerId"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        public ResetSetTopBoxResponse ResetSetTopBox(string userName, string password, string sequence, string smartCards, string userId, string referenceNumber, 
            string notes, string customerId, string zone) {
            var objResetSetTopBoxResponse = new ResetSetTopBoxResponse();
            timeStampProcess = string.Empty;
            timeStampProcess = _util.GenerateTimeStamp();   
            try
            {
                //CF: Escribimos en el Log para verificar los datos recibidos.
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.ResetSetTopBox, 
                    "REQUEST: [ProcessId: " + timeStampProcess + "] [UserName: " + userName + ". Password: " + password + ". Sequence: " + sequence + ". SmartCards: " + smartCards + ". UserId: " + userId + ". ReferenceNumber: "
                    + referenceNumber + ". Notes: " + notes + ". CustomerId: " + customerId + ". Zone: " + zone + ".]", Util.ErrorTypeEnum.TraceType);

               // var passwordEncriptado = EncriptaMD5(password);
                var validateUserResponse = _tvCableIntegrationBl.VerificaLoginUser(userName, password, Constants.ResetSetTopBox);

                if (validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString().Equals(Constants.DefaultOkCode))
                {
                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.ResetSetTopBox,
                    "VALIDATE USER RESPONSE: [ProcessId: " + timeStampProcess + "] [" + validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString() + "]", Util.ErrorTypeEnum.TraceType);

                    var objResetSetTopBox = new resetSetTopBox
                    {
                        username = _configuration.UserName,
                        password = _configuration.Password,
                        sequence = int.Parse(sequence),
                        smartcard = Convert.ToDouble(smartCards),
                        user_id = userId,
                        reference_number = referenceNumber,
                        notes = notes,
                        customer_id = customerId,
                        zone = zone
                    };

                    var objResetSetTopBoxRequest = new resetSetTopBoxRequest();
                    objResetSetTopBoxRequest.resetSetTopBox = objResetSetTopBox;

                    var responseResetSetTopBoxWsTuves = _wrTuvesService.resetSetTopBox(objResetSetTopBoxRequest);

                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.ResetSetTopBox, 
                        "RESPONSE TUVES: [ProcessId: " + timeStampProcess + "] [Answer: " + responseResetSetTopBoxWsTuves.resetSetTopBoxResponse.answer + ". ReturnValue: " + Convert.ToString(responseResetSetTopBoxWsTuves.resetSetTopBoxResponse.@return) + "]", Util.ErrorTypeEnum.TraceType);

                     ///////////////
                    //CF: Verifica si el resultado es igual a 0, caso contrario, devolvemos el resultado del error generico y error generado por Tuves
                    if (responseResetSetTopBoxWsTuves.resetSetTopBoxResponse.@return.ToString().Equals(Constants.DefaultOkCode))
                    {
                        objResetSetTopBoxResponse.ResultCode = Constants.DefaultOkCode;
                        objResetSetTopBoxResponse.ResultDescription = Constants.DefaultOkDescription;
                        objResetSetTopBoxResponse.Answer = responseResetSetTopBoxWsTuves.resetSetTopBoxResponse.answer;
                        objResetSetTopBoxResponse.ReturnValue = Convert.ToString(responseResetSetTopBoxWsTuves.resetSetTopBoxResponse.@return);
                    }
                    else
                    {
                        var getDescriptionAnswerResponse = _tvCableIntegrationBl.GetDescriptionAnswer(responseResetSetTopBoxWsTuves.resetSetTopBoxResponse.@return.ToString());
                        objResetSetTopBoxResponse.ResultCode = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                        objResetSetTopBoxResponse.ResultDescription = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                        objResetSetTopBoxResponse.Answer = responseResetSetTopBoxWsTuves.resetSetTopBoxResponse.answer;
                        objResetSetTopBoxResponse.ReturnValue = Convert.ToString(responseResetSetTopBoxWsTuves.resetSetTopBoxResponse.@return);                      
                    }
                    //////////////////
                }
                else {
                    objResetSetTopBoxResponse.ResultCode = validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                    objResetSetTopBoxResponse.ResultDescription = validateUserResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                    objResetSetTopBoxResponse.Answer = string.Empty;
                    objResetSetTopBoxResponse.ReturnValue = string.Empty;                 
                }              
                /////////
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.ResetSetTopBox, 
                    "RESPONSE: [ProcessId: " + timeStampProcess + "] [Answer: " + objResetSetTopBoxResponse.Answer + ". ReturnValue: " + objResetSetTopBoxResponse.ReturnValue + "]" +
                    " [ResultCode: " + objResetSetTopBoxResponse.ResultCode + "]" + " [ResultDescription: " + objResetSetTopBoxResponse.ResultDescription + "]", Util.ErrorTypeEnum.TraceType);
                /////////
                return objResetSetTopBoxResponse;
            }
            catch (Exception ex) {
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.ResetSetTopBox, "EXCEPTION: [ProcessId: " + timeStampProcess + "] " + ex.ToString(), Util.ErrorTypeEnum.ErrorType);
                objResetSetTopBoxResponse = new ResetSetTopBoxResponse();
                objResetSetTopBoxResponse.ResultCode = Constants.DefaultErrorCode;
                objResetSetTopBoxResponse.ResultDescription = Constants.DefaultErrorDescription;
                objResetSetTopBoxResponse.Answer = string.Empty;
                objResetSetTopBoxResponse.ReturnValue = string.Empty;
                return objResetSetTopBoxResponse;
            }
        }

        /// <summary>
        /// Esta operación cambia, en el settopbox de la smartcard ingresada, el código de zona. Luego procede a reescanear las señales en el settopbox.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="sequence"></param>
        /// <param name="smartCards"></param>
        /// <param name="code"></param>
        /// <param name="userId"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="notes"></param>
        /// <param name="customerId"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        public UpdateCodeZoneResponse UpdateCodeZone(string userName, string password, string sequence, string smartCards, string code, string userId, 
            string referenceNumber, string notes, string customerId, string zone) {
            var objUpdateCodeZoneResponse = new UpdateCodeZoneResponse();
            timeStampProcess = string.Empty;
            timeStampProcess = _util.GenerateTimeStamp();   
            try
            {
                //CF: Escribimos en el Log para verificar los datos recibidos.
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.UpdateCodeZone, 
                    "REQUEST: [ProcessId: " + timeStampProcess + "] [UserName: " + userName + ". Password: " + password + ". Sequence: " + sequence + ". SmartCards: " + smartCards + ". Code: " + code + ". UserId: " + userId + ". ReferenceNumber: "
                    + referenceNumber + ". Notes: " + notes + ". CustomerId: " + customerId + ". Zone: " + zone + "]", Util.ErrorTypeEnum.TraceType);

                //var passwordEncriptado = EncriptaMD5(password);
                var validateUserResponse = _tvCableIntegrationBl.VerificaLoginUser(userName, password, Constants.UpdateCodeZone);

                if (validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString().Equals(Constants.DefaultOkCode))
                {
                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.UpdateCodeZone,
                    "VALIDATE USER RESPONSE: [ProcessId: " + timeStampProcess + "] [" + validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString() + "]", Util.ErrorTypeEnum.TraceType);

                    var objUpdateCodeZone = new updateCodeZone
                    {
                        username = _configuration.UserName,
                        password = _configuration.Password,
                        sequence = int.Parse(sequence),
                        smartcard = Convert.ToDouble(smartCards),
                        code = code,
                        user_id = userId,
                        reference_number = referenceNumber,
                        notes = notes,
                        customer_id = customerId,
                        zone = zone
                    };

                    var objUpdateCodeZoneRequest = new updateCodeZoneRequest();
                    objUpdateCodeZoneRequest.updateCodeZone = objUpdateCodeZone;

                    var responseUpdateCodeZoneWsTuves = _wrTuvesService.updateCodeZone(objUpdateCodeZoneRequest);

                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.UpdateCodeZone, 
                    "RESPONSE TUVES: [ProcessId: " + timeStampProcess + "] [Answer: " + responseUpdateCodeZoneWsTuves.updateCodeZoneResponse.answer + ". ReturnValue: " + Convert.ToString(responseUpdateCodeZoneWsTuves.updateCodeZoneResponse.@return) + "]", Util.ErrorTypeEnum.TraceType);

                    ///////////////
                    //CF: Verifica si el resultado es igual a 0, caso contrario, devolvemos el resultado del error generico y error generado por Tuves
                    if (responseUpdateCodeZoneWsTuves.updateCodeZoneResponse.@return.ToString().Equals(Constants.DefaultOkCode))
                    {
                        objUpdateCodeZoneResponse.ResultCode = Constants.DefaultOkCode;
                        objUpdateCodeZoneResponse.ResultDescription = Constants.DefaultOkDescription;
                        objUpdateCodeZoneResponse.Answer = responseUpdateCodeZoneWsTuves.updateCodeZoneResponse.answer;
                        objUpdateCodeZoneResponse.ReturnValue = Convert.ToString(responseUpdateCodeZoneWsTuves.updateCodeZoneResponse.@return);
                    }
                    else
                    {
                        var getDescriptionAnswerResponse = _tvCableIntegrationBl.GetDescriptionAnswer(responseUpdateCodeZoneWsTuves.updateCodeZoneResponse.@return.ToString());
                        objUpdateCodeZoneResponse.ResultCode = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                        objUpdateCodeZoneResponse.ResultDescription = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                        objUpdateCodeZoneResponse.Answer = responseUpdateCodeZoneWsTuves.updateCodeZoneResponse.answer;
                        objUpdateCodeZoneResponse.ReturnValue = Convert.ToString(responseUpdateCodeZoneWsTuves.updateCodeZoneResponse.@return);                       
                    }                   
                    /////////////////
                }
                else
                {
                    objUpdateCodeZoneResponse.ResultCode = validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                    objUpdateCodeZoneResponse.ResultDescription = validateUserResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                    objUpdateCodeZoneResponse.Answer = string.Empty;
                    objUpdateCodeZoneResponse.ReturnValue = string.Empty;                
                }
                //////////////
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.UpdateCodeZone, 
                    "RESPONSE: [ProcessId: " + timeStampProcess + "] [Answer: " + objUpdateCodeZoneResponse.Answer + ". ReturnValue: " + objUpdateCodeZoneResponse.ReturnValue + "]" + 
                    " [ResultCode: " + objUpdateCodeZoneResponse.ResultCode + "]" + " [ResultDescription: " + objUpdateCodeZoneResponse.ResultDescription + "]", Util.ErrorTypeEnum.TraceType);
                //////////////
                return objUpdateCodeZoneResponse;

            }
            catch (Exception ex) {
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.UpdateCodeZone, "EXCEPTION: [ProcessId: " + timeStampProcess + "]" + ex.ToString(), Util.ErrorTypeEnum.ErrorType);
                objUpdateCodeZoneResponse = new UpdateCodeZoneResponse();
                objUpdateCodeZoneResponse.ResultCode = Constants.DefaultErrorCode;
                objUpdateCodeZoneResponse.ResultDescription = Constants.DefaultErrorDescription;
                objUpdateCodeZoneResponse.Answer = string.Empty;
                objUpdateCodeZoneResponse.ReturnValue = string.Empty;
                return objUpdateCodeZoneResponse;
            }
        }

        /// <summary>
        /// Esta operación generará, en el settopbox de la smartcard ingresada, un proceso de rebooteo. El proceso no borra ajustes.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="sequence"></param>
        /// <param name="smartCards"></param>
        /// <param name="userId"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="notes"></param>
        /// <param name="customerId"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        public RebootSetTopBoxResponse RebootSetTopBox(string userName, string password, string sequence, string smartCards, string userId, string referenceNumber, 
            string notes, string customerId, string zone) {
            
            var objRebootSetTopBoxResponse = new RebootSetTopBoxResponse();
            timeStampProcess = string.Empty;
            timeStampProcess = _util.GenerateTimeStamp();   
            try
            {
                //CF: Escribimos en el Log para verificar los datos recibidos.
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.RebootSetTopBox, 
                    "REQUEST: [ProcessId: " + timeStampProcess + "] [UserName: " + userName + ". Password: " + password + ". Sequence: " + sequence + ". SmartCards: " + smartCards + ". UserId: " + userId + ". ReferenceNumber: "
                    + referenceNumber + ". Notes: " + notes + ". CustomerId: " + customerId + ". Zone: " + zone + "]", Util.ErrorTypeEnum.TraceType);

               // var passwordEncriptado = EncriptaMD5(password);
                var validateUserResponse = _tvCableIntegrationBl.VerificaLoginUser(userName, password, Constants.RebootSetTopBox);

                if (validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString().Equals(Constants.DefaultOkCode))
                {
                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.RebootSetTopBox,
                   "VALIDATE USER RESPONSE: [ProcessId: " + timeStampProcess + "] [" + validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString() + "]", Util.ErrorTypeEnum.TraceType);

                    var objRebootSetTopBox = new rebootSetTopBox
                    {
                        username = _configuration.UserName,
                        password = _configuration.Password,
                        sequence = int.Parse(sequence),
                        smartcard = Convert.ToDouble(smartCards),
                        user_id = userId,
                        reference_number = referenceNumber,
                        notes = notes,
                        customer_id = customerId,
                        zone = zone
                    };

                    var objRebootSetTopBoxRequest = new rebootSetTopBoxRequest();
                    objRebootSetTopBoxRequest.rebootSetTopBox = objRebootSetTopBox;

                    var responseRebootSetTopBoxWsTuves = _wrTuvesService.rebootSetTopBox(objRebootSetTopBoxRequest);

                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.RebootSetTopBox, 
                    "RESPONSE TUVES: [ProcessId: " + timeStampProcess + "] [Answer: " + responseRebootSetTopBoxWsTuves.rebootSetTopBoxResponse.answer + ". ReturnValue: " + Convert.ToString(responseRebootSetTopBoxWsTuves.rebootSetTopBoxResponse.@return) + "]" , Util.ErrorTypeEnum.TraceType);

                    ///////////////
                    //CF: Verifica si el resultado es igual a 0, caso contrario, devolvemos el resultado del error generico y error generado por Tuves
                    if (responseRebootSetTopBoxWsTuves.rebootSetTopBoxResponse.@return.ToString().Equals(Constants.DefaultOkCode))
                    {
                        objRebootSetTopBoxResponse.ResultCode = Constants.DefaultOkCode;
                        objRebootSetTopBoxResponse.ResultDescription = Constants.DefaultOkDescription;
                        objRebootSetTopBoxResponse.Answer = responseRebootSetTopBoxWsTuves.rebootSetTopBoxResponse.answer;
                        objRebootSetTopBoxResponse.ReturnValue = Convert.ToString(responseRebootSetTopBoxWsTuves.rebootSetTopBoxResponse.@return);
                    }
                    else
                    {
                        var getDescriptionAnswerResponse = _tvCableIntegrationBl.GetDescriptionAnswer(responseRebootSetTopBoxWsTuves.rebootSetTopBoxResponse.@return.ToString());
                        objRebootSetTopBoxResponse.ResultCode = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                        objRebootSetTopBoxResponse.ResultDescription = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                        objRebootSetTopBoxResponse.Answer = responseRebootSetTopBoxWsTuves.rebootSetTopBoxResponse.answer;
                        objRebootSetTopBoxResponse.ReturnValue = Convert.ToString(responseRebootSetTopBoxWsTuves.rebootSetTopBoxResponse.@return);                    
                    }                    
                    ////////////////////                  
                }
                else 
                {
                    objRebootSetTopBoxResponse.ResultCode = validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                    objRebootSetTopBoxResponse.ResultDescription = validateUserResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                    objRebootSetTopBoxResponse.Answer = string.Empty;
                    objRebootSetTopBoxResponse.ReturnValue = string.Empty;                 
                }            
                ////////////////
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.RebootSetTopBox, 
                    "RESPONSE: [ProcessId: " + timeStampProcess + "] [Answer: " + objRebootSetTopBoxResponse.Answer + ". ReturnValue: " + objRebootSetTopBoxResponse.ReturnValue + "]" +
                    " [ResultCode: " + objRebootSetTopBoxResponse.ResultCode + "]" + " [ResultDescription: " + objRebootSetTopBoxResponse.ResultDescription + "]", Util.ErrorTypeEnum.TraceType);
                ///////////////
                return objRebootSetTopBoxResponse;
            }
            catch(Exception ex) {
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.RebootSetTopBox, "EXCEPTION: [ProcessId: " + timeStampProcess + "] " + ex.ToString(), Util.ErrorTypeEnum.ErrorType);
                objRebootSetTopBoxResponse = new RebootSetTopBoxResponse();
                objRebootSetTopBoxResponse.ResultCode = Constants.DefaultErrorCode;
                objRebootSetTopBoxResponse.ResultDescription = Constants.DefaultErrorDescription;
                objRebootSetTopBoxResponse.Answer = string.Empty;
                objRebootSetTopBoxResponse.ReturnValue = string.Empty;
                return objRebootSetTopBoxResponse;
            }
        }

        /// <summary>
        /// Esta operación actualiza, de forma forzada en el settopbox de la smartcard ingresada, el software a la última versión oficial.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="sequence"></param>
        /// <param name="smartCards"></param>
        /// <param name="userId"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="notes"></param>
        /// <param name="customerId"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        public ForceUpdateFirmwareResponse ForceUpdateFirmware(string userName, string password, string sequence, string smartCards, string userId, string referenceNumber, 
            string notes, string customerId, string zone) {
            var objForceUpdateFirmwareResponse = new ForceUpdateFirmwareResponse();
            timeStampProcess = string.Empty;
            timeStampProcess = _util.GenerateTimeStamp();   
            try
            {
                //CF: Escribimos en el Log para verificar los datos recibidos.
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.ForceUpdateFirmware, 
                    "REQUEST: [ProcessId: " + timeStampProcess + "] [UserName: " + userName + ". Password: " + password + ". Sequence: " + sequence + ". SmartCards: " + smartCards + ". UserId: " + userId + ". ReferenceNumber: "
                    + referenceNumber + ". Notes: " + notes + ". CustomerId: " + customerId + ". Zone: " + zone + "]", Util.ErrorTypeEnum.TraceType);

                //var passwordEncriptado = EncriptaMD5(password);
                var validateUserResponse = _tvCableIntegrationBl.VerificaLoginUser(userName, password, Constants.ForceUpdateFirmware);

                if (validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString().Equals(Constants.DefaultOkCode))
                {
                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.ForceUpdateFirmware,
                   "VALIDATE USER RESPONSE: [ProcessId: " + timeStampProcess + "] [" + validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString() + "]", Util.ErrorTypeEnum.TraceType);

                    var objForceUpdateFirmware = new forceUpdateFirmware
                    {
                        username = _configuration.UserName,
                        password = _configuration.Password,
                        sequence = int.Parse(sequence),
                        smartcard = Convert.ToDouble(smartCards),
                        user_id = userId,
                        reference_number = referenceNumber,
                        notes = notes,
                        customer_id = customerId,
                        zone = zone
                    };

                    var objForceUpdateFirmwareRequest = new forceUpdateFirmwareRequest();
                    objForceUpdateFirmwareRequest.forceUpdateFirmware = objForceUpdateFirmware;

                    var responseForceUpdateFirmwareWsTuves = _wrTuvesService.forceUpdateFirmware(objForceUpdateFirmwareRequest);

                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.ForceUpdateFirmware, 
                    "RESPONSE TUVES: [ProcessId: " + timeStampProcess + "] [Answer: " + responseForceUpdateFirmwareWsTuves.forceUpdateFirmwareResponse.answer + ". ReturnValue: " + Convert.ToString(responseForceUpdateFirmwareWsTuves.forceUpdateFirmwareResponse.@return) + "]", Util.ErrorTypeEnum.TraceType);

                    ///////////////
                    //CF: Verifica si el resultado es igual a 0, caso contrario, devolvemos el resultado del error generico y error generado por Tuves
                    if (responseForceUpdateFirmwareWsTuves.forceUpdateFirmwareResponse.@return.ToString().Equals(Constants.DefaultOkCode))
                    {
                        objForceUpdateFirmwareResponse.ResultCode = Constants.DefaultOkCode;
                        objForceUpdateFirmwareResponse.ResultDescription = Constants.DefaultOkDescription;
                        objForceUpdateFirmwareResponse.Answer = responseForceUpdateFirmwareWsTuves.forceUpdateFirmwareResponse.answer;
                        objForceUpdateFirmwareResponse.ReturnValue = Convert.ToString(responseForceUpdateFirmwareWsTuves.forceUpdateFirmwareResponse.@return);
                    }
                    else
                    {
                        var getDescriptionAnswerResponse = _tvCableIntegrationBl.GetDescriptionAnswer(responseForceUpdateFirmwareWsTuves.forceUpdateFirmwareResponse.@return.ToString());
                        objForceUpdateFirmwareResponse.ResultCode = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                        objForceUpdateFirmwareResponse.ResultDescription = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                        objForceUpdateFirmwareResponse.Answer = responseForceUpdateFirmwareWsTuves.forceUpdateFirmwareResponse.answer;
                        objForceUpdateFirmwareResponse.ReturnValue = Convert.ToString(responseForceUpdateFirmwareWsTuves.forceUpdateFirmwareResponse.@return);                       
                    }
                   /////////////////////////////
                }
                else
                {
                    objForceUpdateFirmwareResponse.ResultCode = validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                    objForceUpdateFirmwareResponse.ResultDescription = validateUserResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                    objForceUpdateFirmwareResponse.Answer = string.Empty;
                    objForceUpdateFirmwareResponse.ReturnValue = string.Empty;                            
                }
                ////////
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.ForceUpdateFirmware, 
                    "RESPONSE: [ProcessId: " + timeStampProcess + "] [Answer: " + objForceUpdateFirmwareResponse.Answer + ". ReturnValue: " + objForceUpdateFirmwareResponse.ReturnValue + "]" +
                    " [ResultCode: " + objForceUpdateFirmwareResponse.ResultCode + "]" + " [ResultDescription: " + objForceUpdateFirmwareResponse.ResultDescription + "]", Util.ErrorTypeEnum.TraceType);
                ////////

                return objForceUpdateFirmwareResponse;

            }
            catch (Exception ex)
            {
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.ForceUpdateFirmware, "EXCEPTION: [ProcessId: " + timeStampProcess + "]" + ex.ToString(), Util.ErrorTypeEnum.ErrorType);
                objForceUpdateFirmwareResponse = new ForceUpdateFirmwareResponse();
                objForceUpdateFirmwareResponse.ResultCode = Constants.DefaultErrorCode;
                objForceUpdateFirmwareResponse.ResultDescription = Constants.DefaultErrorDescription;
                objForceUpdateFirmwareResponse.Answer = string.Empty;
                objForceUpdateFirmwareResponse.ReturnValue = string.Empty;
                return objForceUpdateFirmwareResponse;
            }
        }

        /// <summary>
        /// Esta operación cambia, en el settopbox de la smartcard ingresada, la clave de control parental por la especificada en el campo pin.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="sequence"></param>
        /// <param name="smartCards"></param>
        /// <param name="pin"></param>
        /// <param name="userId"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="notes"></param>
        /// <param name="customerId"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        public UpdateControlParentallResponse UpdateControlParentall(string userName, string password, string sequence, string smartCards, string pin, 
            string userId, string referenceNumber, string notes, string customerId, string zone) {
            var objUpdateControlParentallResponse = new UpdateControlParentallResponse();
            timeStampProcess = string.Empty;
            timeStampProcess = _util.GenerateTimeStamp();   
            try
            {
                //CF: Escribimos en el Log para verificar los datos recibidos.
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.UpdateControlParentall, 
                    "REQUEST: [ProcessId: " + timeStampProcess + "] [UserName: " + userName + ". Password: " + password + ". Sequence: " + sequence + ". SmartCards: " + smartCards + ". Pin: " + pin + ". UserId: " + userId + ". ReferenceNumber: "
                    + referenceNumber + ". Notes: " + notes + ". CustomerId: " + customerId + ". Zone: " + zone + "]", Util.ErrorTypeEnum.TraceType);

                //var passwordEncriptado = EncriptaMD5(password);
                var validateUserResponse = _tvCableIntegrationBl.VerificaLoginUser(userName, password, Constants.UpdateControlParentall);

                if (validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString().Equals(Constants.DefaultOkCode))
                {
                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.UpdateControlParentall,
                   "VALIDATE USER RESPONSE: [ProcessId: " + timeStampProcess + "] [" + validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString() + "]", Util.ErrorTypeEnum.TraceType);

                    var objUpdateControlParentall = new updateControlParental
                    {
                        username = _configuration.UserName,
                        password = _configuration.Password,
                        sequence = int.Parse(sequence),
                        smartcard = Convert.ToDouble(smartCards),
                        pin = pin,
                        user_id = userId,
                        reference_number = referenceNumber,
                        notes = notes,
                        customer_id = customerId,
                        zone = zone
                    };

                    var objUpdateControlParentallRequest = new updateControlParentalRequest();
                    objUpdateControlParentallRequest.updateControlParental = objUpdateControlParentall;

                    var responseUpdateControlParentallWsTuves = _wrTuvesService.updateControlParental(objUpdateControlParentallRequest);

                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.UpdateControlParentall, 
                    "RESPONSE TUVES: [ProcessId: " + timeStampProcess + "] [Answer: " + responseUpdateControlParentallWsTuves.updateControlParentalResponse.answer + ". ReturnValue: " + Convert.ToString(responseUpdateControlParentallWsTuves.updateControlParentalResponse.@return) + "]", Util.ErrorTypeEnum.TraceType);

                    ///////////////
                    //CF: Verifica si el resultado es igual a 0, caso contrario, devolvemos el resultado del error generico y error generado por Tuves
                    if (responseUpdateControlParentallWsTuves.updateControlParentalResponse.@return.ToString().Equals(Constants.DefaultOkCode))
                    {
                        objUpdateControlParentallResponse.ResultCode = Constants.DefaultOkCode;
                        objUpdateControlParentallResponse.ResultDescription = Constants.DefaultOkDescription;
                        objUpdateControlParentallResponse.Answer = responseUpdateControlParentallWsTuves.updateControlParentalResponse.answer;
                        objUpdateControlParentallResponse.ReturnValue = Convert.ToString(responseUpdateControlParentallWsTuves.updateControlParentalResponse.@return);
                    }
                    else 
                    {
                        var getDescriptionAnswerResponse = _tvCableIntegrationBl.GetDescriptionAnswer(responseUpdateControlParentallWsTuves.updateControlParentalResponse.@return.ToString());
                        objUpdateControlParentallResponse.ResultCode = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                        objUpdateControlParentallResponse.ResultDescription = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                        objUpdateControlParentallResponse.Answer = responseUpdateControlParentallWsTuves.updateControlParentalResponse.answer;
                        objUpdateControlParentallResponse.ReturnValue = Convert.ToString(responseUpdateControlParentallWsTuves.updateControlParentalResponse.@return);                   
                    }                     
                    ///////////////////
                }
                else
                {
                    objUpdateControlParentallResponse.ResultCode = validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                    objUpdateControlParentallResponse.ResultDescription = validateUserResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                    objUpdateControlParentallResponse.Answer = string.Empty;
                    objUpdateControlParentallResponse.ReturnValue = string.Empty;               
                }
                /////////
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.UpdateControlParentall, 
                    "RESPONSE: [ProcessId: " + timeStampProcess + "] [Answer: " + objUpdateControlParentallResponse.Answer + ". ReturnValue: " + objUpdateControlParentallResponse.ReturnValue + "]" +" [ResultCode: " + objUpdateControlParentallResponse.ResultCode + "]" +
                     " [ResultDescription: " + objUpdateControlParentallResponse.ResultDescription + "]", Util.ErrorTypeEnum.TraceType);
                /////////
                return objUpdateControlParentallResponse;
            }
            catch (Exception ex)
            {
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.UpdateControlParentall, "EXCEPTION: [ProcessId: " + timeStampProcess + "] " + ex.ToString(), Util.ErrorTypeEnum.ErrorType);
                objUpdateControlParentallResponse = new UpdateControlParentallResponse();
                objUpdateControlParentallResponse.ResultCode = Constants.DefaultErrorCode;
                objUpdateControlParentallResponse.ResultDescription = Constants.DefaultErrorDescription;
                objUpdateControlParentallResponse.Answer = string.Empty;
                objUpdateControlParentallResponse.ReturnValue = string.Empty;
                return objUpdateControlParentallResponse;
            }
        }

        /// <summary>
        /// Esta operación permite cambiar, en el settopbox de la smartcard ingresada, el password default para el acceso al menú de instalación.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="sequence"></param>
        /// <param name="smartCards"></param>
        /// <param name="pin"></param>
        /// <param name="userId"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="notes"></param>
        /// <param name="customerId"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        public UpdateMenuPassResponse UpdateMenuPass(string userName, string password, string sequence, string smartCards, string pin, string userId, 
            string referenceNumber, string notes, string customerId, string zone) {
            var objUpdateMenuPassResponse = new UpdateMenuPassResponse();
            timeStampProcess = string.Empty;
            timeStampProcess = _util.GenerateTimeStamp();   
            try
            {

                //CF: Escribimos en el Log para verificar los datos recibidos.
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.UpdateMenuPass, 
                    "REQUEST: [ProcessId: " + timeStampProcess + "] [UserName: " + userName + ". Password: " + password + ". Sequence: " + sequence + ". SmartCards: " + smartCards + ". Pin: " + pin + ". UserId: " + userId + ". ReferenceNumber: "
                    + referenceNumber + ". Notes: " + notes + ". CustomerId: " + customerId + ". Zone: " + zone + "]", Util.ErrorTypeEnum.TraceType);

                //var passwordEncriptado = EncriptaMD5(password);
                var validateUserResponse = _tvCableIntegrationBl.VerificaLoginUser(userName, password, Constants.UpdateMenuPass);

                if (validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString().Equals(Constants.DefaultOkCode))
                {
                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.UpdateMenuPass,
                   "VALIDATE USER RESPONSE: [ProcessId: " + timeStampProcess + "] [" + validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString() + "]", Util.ErrorTypeEnum.TraceType);

                    var objUpdateMenuPass = new updateMenuPass
                    {
                        username = _configuration.UserName,
                        password = _configuration.Password,
                        sequence = int.Parse(sequence),
                        smartcard = Convert.ToDouble(smartCards),
                        pin = pin,
                        user_id = userId,
                        reference_number = referenceNumber,
                        notes = notes,
                        customer_id = customerId,
                        zone = zone
                    };

                    var objUpdateMenuPassRequest = new updateMenuPassRequest();
                    objUpdateMenuPassRequest.updateMenuPass = objUpdateMenuPass;

                    var responseUpdateMenuPassWsTuves = _wrTuvesService.updateMenuPass(objUpdateMenuPassRequest);

                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.UpdateMenuPass, 
                        "RESPONSE TUVES: [ProcessId: " + timeStampProcess + "] [Answer: " + responseUpdateMenuPassWsTuves.updateMenuPassResponse.answer + ". ReturnValue: " + Convert.ToString(responseUpdateMenuPassWsTuves.updateMenuPassResponse.@return) + "]", Util.ErrorTypeEnum.TraceType);

                    ///////////////
                    //CF: Verifica si el resultado es igual a 0, caso contrario, devolvemos el resultado del error generico y error generado por Tuves
                    if (responseUpdateMenuPassWsTuves.updateMenuPassResponse.@return.ToString().Equals(Constants.DefaultOkCode))
                    {
                        objUpdateMenuPassResponse.ResultCode = Constants.DefaultOkCode;
                        objUpdateMenuPassResponse.ResultDescription = Constants.DefaultOkDescription;
                        objUpdateMenuPassResponse.Answer = responseUpdateMenuPassWsTuves.updateMenuPassResponse.answer;
                        objUpdateMenuPassResponse.ReturnValue = Convert.ToString(responseUpdateMenuPassWsTuves.updateMenuPassResponse.@return);                       
                    }
                    else
                    {
                        var getDescriptionAnswerResponse = _tvCableIntegrationBl.GetDescriptionAnswer(responseUpdateMenuPassWsTuves.updateMenuPassResponse.@return.ToString());
                        objUpdateMenuPassResponse.ResultCode = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                        objUpdateMenuPassResponse.ResultDescription = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                        objUpdateMenuPassResponse.Answer = responseUpdateMenuPassWsTuves.updateMenuPassResponse.answer;
                        objUpdateMenuPassResponse.ReturnValue = Convert.ToString(responseUpdateMenuPassWsTuves.updateMenuPassResponse.@return);                        
                    }                    
                    /////////////////
                }
                else
                {
                    objUpdateMenuPassResponse.ResultCode = validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                    objUpdateMenuPassResponse.ResultDescription = validateUserResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                    objUpdateMenuPassResponse.Answer = string.Empty;
                    objUpdateMenuPassResponse.ReturnValue = string.Empty;                 
                } 
                ////////////////////
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.UpdateMenuPass, 
                    "RESPONSE: [ProcessId: " + timeStampProcess + "] [Answer: " + objUpdateMenuPassResponse.Answer + ". ReturnValue: " + objUpdateMenuPassResponse.ReturnValue + "]" +
                    " [ResultCode: " + objUpdateMenuPassResponse.ResultCode + "]" + " [ResultDescription: " + objUpdateMenuPassResponse.ResultDescription + "]", Util.ErrorTypeEnum.TraceType);
                ////////////////////
                return objUpdateMenuPassResponse;
            }
            catch (Exception ex)
            {
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.UpdateMenuPass, "EXCEPTION: [ProcessId: " + timeStampProcess + "] " + ex.ToString(), Util.ErrorTypeEnum.ErrorType);
                objUpdateMenuPassResponse = new UpdateMenuPassResponse();
                objUpdateMenuPassResponse.ResultCode = Constants.DefaultErrorCode;
                objUpdateMenuPassResponse.ResultDescription = Constants.DefaultErrorDescription;
                objUpdateMenuPassResponse.Answer = string.Empty;
                objUpdateMenuPassResponse.ReturnValue = string.Empty;
                return objUpdateMenuPassResponse;
            }
        
        }

        /// <summary>
        /// Permite conocer si el Webservice de TuVes está online.
        /// </summary>
        /// <param name="userName">Usuario</param>
        /// <param name="password">Clave</param>
        /// <returns></returns>
        public EstatusResponse Status(string userName, string password)
        {
            var responseStatus = new EstatusResponse();
            timeStampProcess = string.Empty;
            timeStampProcess = _util.GenerateTimeStamp();   
            //string context = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
            try
            {
                //CF: Escribimos en el log los datos a enviar
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.StatusMetodo, "REQUEST: [ProcessId: " + timeStampProcess + "] [Username: " + userName + ". Password: " + password + "]", Util.ErrorTypeEnum.TraceType);
                
                //   var passwordEncriptado = EncriptaMD5(password);
                var validateUserResponse = _tvCableIntegrationBl.VerificaLoginUser(userName, password, Constants.StatusMetodo);                

                //CF: Comparamos el valor recibido por parte del SP, donde 0 significa que tiene valores permitidos para ejecutar la función
                if (validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString().Equals(Constants.DefaultOkCode))
                {
                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.StatusMetodo,
                    "VALIDATE USER RESPONSE: [ProcessId: " + timeStampProcess + "] [" + validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString() + "]", Util.ErrorTypeEnum.TraceType);

                    var objStatusRequest = new statusRequest();
                    var statusRequest = new status { username = _configuration.UserName, password = _configuration.Password };
                    objStatusRequest.status = statusRequest;

                    var responseStatusWsTuves = _wrTuvesService.status(objStatusRequest);

                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.StatusMetodo,
                        "RESPONSE TUVES: [ProcessId: " + timeStampProcess + "] [Status_webservice: " + responseStatusWsTuves.statusResponse.status_webservice + ". Description: " + responseStatusWsTuves.statusResponse.description + "]", Util.ErrorTypeEnum.TraceType);

                    ///////////////
                    //CF: Verifica si el resultado es igual a 1, caso contrario, devolvemos el resultado del error generico y error generado por Tuves
                    if (responseStatusWsTuves.statusResponse.status_webservice.ToString().Equals(Constants.DefaultOkCodeStatus)) //1
                    {
                        responseStatus.ResultCode = Constants.DefaultOkCodeStatus;
                        responseStatus.ResultDescription = Constants.DefaultOkDescription;
                        responseStatus.StatusWebservice = responseStatusWsTuves.statusResponse.status_webservice;
                        responseStatus.Description = responseStatusWsTuves.statusResponse.description;                      
                    }
                    else
                    {                       
                        responseStatus.ResultCode = Constants.DefaultErrorCode;
                        responseStatus.ResultDescription = Constants.DefaultErrorDescription;
                        responseStatus.StatusWebservice = responseStatusWsTuves.statusResponse.status_webservice;
                        responseStatus.Description = responseStatusWsTuves.statusResponse.description;                        
                    }                  
                    ////////
                }
                else 
                {                    
                    // Obtengo el mensaje del codigo de error para agregar al objeto de respuesta
                    responseStatus.ResultCode = validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                    responseStatus.ResultDescription = validateUserResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                    responseStatus.StatusWebservice = string.Empty;
                    responseStatus.Description = string.Empty;                  
                }
                ////////////////////
                
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.StatusMetodo, 
                    "RESPONSE: [ProcessId: " + timeStampProcess + "] [Status_webservice: " + responseStatus.ResultCode + ". Description: " + responseStatus.ResultDescription + "]" +
                    " [Status_webservice_CAS: " + responseStatus.StatusWebservice + "]" + " [Description_CAS: " + responseStatus.Description + "]", Util.ErrorTypeEnum.TraceType);
                ////////////////////
                return responseStatus;
            }
            catch (Exception ex)
            {
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.StatusMetodo, "EXCEPTION: [ProcessId: " + timeStampProcess + "]" + ex.ToString(), Util.ErrorTypeEnum.ErrorType);
                responseStatus = new EstatusResponse
                               {
                                   ResultCode = Constants.DefaultErrorCode,
                                   ResultDescription = Constants.DefaultErrorDescription,
                                   StatusWebservice = string.Empty,
                                   Description = string.Empty
                               };
                return responseStatus;
            }
        }

        /// <summary>
        /// Esta operación permite obtener, en un horario preestablecido para la acción, toda la información de smartcards y settopbox cargados por el webservice.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public EGetSmartCardsInfoResponse GetSmartCardsInfo(string userName, string password)
        {
            var ObjGetSmartCardInfoResponse = new EGetSmartCardsInfoResponse();
            timeStampProcess = string.Empty;
            timeStampProcess = _util.GenerateTimeStamp();   
            try
            {
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.GetSmartCardsInfo, 
                    "REQUEST: [ProcessId: " + timeStampProcess + "] [Username: " + userName + ". Password: " + password + "]", Util.ErrorTypeEnum.TraceType);

                //var passwordEncriptado = EncriptaMD5(password);
                var validateUserResponse = _tvCableIntegrationBl.VerificaLoginUser(userName, password, Constants.GetSmartCardsInfo);

                if (validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString().Equals(Constants.DefaultCodeCasOkCode))
                {
                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.GetSmartCardsInfo,
                    "VALIDATE USER RESPONSE: [ProcessId: " + timeStampProcess + "] [" + validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString() + "]", Util.ErrorTypeEnum.TraceType);

                    var objGetSmartCardInfoRequest = new getSmartcardsInfoRequest();
                    var getSmartCardInfoRequest = new getSmartcardsInfo { username = _configuration.UserName, password = _configuration.Password };
                    objGetSmartCardInfoRequest.getSmartcardsInfo = getSmartCardInfoRequest;

                    var responseGetSmartCardsInfoWsTuves = _wrTuvesService.getSmartcardsInfo(objGetSmartCardInfoRequest);

                    _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.GetSmartCardsInfo, 
                        "RESPONSE TUVES: [ProcessId: " + timeStampProcess + "] [@Return: " + responseGetSmartCardsInfoWsTuves.getSmartcardsInfoResponse.@return + ". GetSmartCardInfo: " + responseGetSmartCardsInfoWsTuves.getSmartcardsInfoResponse + "]", Util.ErrorTypeEnum.TraceType);            

                    ///////////////
                    //CF: Verifica si el resultado es igual a 0, caso contrario, devolvemos el resultado del error generico y error generado por Tuves
                    if (responseGetSmartCardsInfoWsTuves.getSmartcardsInfoResponse.@return.ToString().Equals(Constants.DefaultOkCode))
                    {
                        ObjGetSmartCardInfoResponse.ResultCode = Constants.DefaultOkCode;
                        ObjGetSmartCardInfoResponse.ResultDescription = Constants.DefaultOkDescription;
                        ObjGetSmartCardInfoResponse.getSmartcardsInfoResponse = responseGetSmartCardsInfoWsTuves.getSmartcardsInfoResponse;                    
                    }
                    else
                    {
                        var getDescriptionAnswerResponse = _tvCableIntegrationBl.GetDescriptionAnswer(responseGetSmartCardsInfoWsTuves.getSmartcardsInfoResponse.@return.ToString());
                        ObjGetSmartCardInfoResponse.ResultCode = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                        ObjGetSmartCardInfoResponse.ResultDescription = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                        ObjGetSmartCardInfoResponse.getSmartcardsInfoResponse = responseGetSmartCardsInfoWsTuves.getSmartcardsInfoResponse;                        
                    }                            
                    ////////                   
                }
                else
                {                                         
                    ObjGetSmartCardInfoResponse.ResultCode = validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                    ObjGetSmartCardInfoResponse.ResultDescription = validateUserResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                    ObjGetSmartCardInfoResponse.getSmartcardsInfoResponse = null;                               
                }              
                //////////
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.GetSmartCardsInfo, 
                    "RESPONSE: [ProcessId: " + timeStampProcess + "] [ResultCode: " + ObjGetSmartCardInfoResponse.ResultCode + ". ResultDescription: " + ObjGetSmartCardInfoResponse.ResultDescription + "]" +
                    " [GetSmartCardsInformation: " + ObjGetSmartCardInfoResponse.getSmartcardsInfoResponse + "]", Util.ErrorTypeEnum.TraceType);    
                //////////
                return ObjGetSmartCardInfoResponse;
            }
            catch (Exception ex) {
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.GetSmartCardsInfo, "EXCEPTION: [ProcessId: " + timeStampProcess + "] " + ex.ToString(), Util.ErrorTypeEnum.ErrorType);
                ObjGetSmartCardInfoResponse = new EGetSmartCardsInfoResponse
                {
                    ResultCode = Constants.DefaultErrorCode,
                    ResultDescription = Constants.DefaultErrorDescription,
                    getSmartcardsInfoResponse = null
                };
                return ObjGetSmartCardInfoResponse;
            }                  
        }

        /// <summary>
        /// Este comando permite obtener la información asociada a una SmartCard. La información entregada es la última registrada desde los comandos ingresados previamente.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="sequence"></param>
        /// <param name="smartCards"></param>
        /// <param name="userId"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="notes"></param>
        /// <param name="customerId"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        public EGetInformationResponse GetInformation(string userName, string password, string sequence, string smartCards, string userId, string referenceNumber, string notes, string customerId, string zone) {
            var objGetInformationResponse = new EGetInformationResponse();
            timeStampProcess = string.Empty;
            timeStampProcess = _util.GenerateTimeStamp();   
            try
            {
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.GetInformation, 
                    "REQUEST: [ProcessId: " + timeStampProcess + "] [Username: " + userName + ". Password: " + password + ". Sequence: " + sequence + ". SmartCards: " + smartCards + ". UserId: " 
                    + userId + ". ReferenceNumber: " + referenceNumber + ". Notes: " + notes + ". CustomerId: " + customerId + ". Zone: " + zone + "]", Util.ErrorTypeEnum.TraceType);

           //   var passwordEncriptado = EncriptaMD5(password);
              var validateUserResponse = _tvCableIntegrationBl.VerificaLoginUser(userName, password, Constants.GetInformation);

              if (validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString().Equals(Constants.DefaultCodeCasOkCode))
              {

                  _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.GetInformation,
                    "VALIDATE USER RESPONSE: [ProcessId: " + timeStampProcess + "] [" + validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString() + "]", Util.ErrorTypeEnum.TraceType);

                  var getInformationRequest = new getInformation
                  {
                      username = _configuration.UserName,
                      password = _configuration.Password,
                      sequence = int.Parse(sequence),
                      smartcard = Convert.ToDouble(smartCards),
                      user_id = userId,
                      reference_number = referenceNumber,
                      notes = notes,
                      customer_id = customerId,
                      zone = zone
                  };

                  var objGetInformationRequest = new getInformationRequest();
                  objGetInformationRequest.getInformation = getInformationRequest;

                  var responseGetInformationWsTuves = _wrTuvesService.getInformation(objGetInformationRequest);
                  _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.GetInformation, 
                      "RESPONSE TUVES: [ProcessId: " + timeStampProcess + "] [@Return: " + responseGetInformationWsTuves.getInformationResponse.@return + ". Answer: " + responseGetInformationWsTuves.getInformationResponse.answer +
                    ". GetInformation: " + responseGetInformationWsTuves.getInformationResponse + "]", Util.ErrorTypeEnum.TraceType);            

                  ///////////////
                  //CF: Verifica si el resultado es igual a 0, caso contrario, devolvemos el resultado del error generico y error generado por Tuves
                  if (responseGetInformationWsTuves.getInformationResponse.@return.ToString().Equals(Constants.DefaultOkCode))
                  {
                      objGetInformationResponse.ResultCode = Constants.DefaultOkCode;
                      objGetInformationResponse.ResultDescription = Constants.DefaultOkDescription;                      
                      objGetInformationResponse.getInformationResponse = responseGetInformationWsTuves.getInformationResponse;                     
                  }
                  else 
                  {
                      var getDescriptionAnswerResponse = _tvCableIntegrationBl.GetDescriptionAnswer(responseGetInformationWsTuves.getInformationResponse.@return.ToString());
                      objGetInformationResponse.ResultCode = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                      objGetInformationResponse.ResultDescription = getDescriptionAnswerResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                      objGetInformationResponse.getInformationResponse = responseGetInformationWsTuves.getInformationResponse;                      
                  }                 
                 /////////////////////
              }
              else
              {
                  objGetInformationResponse.ResultCode = validateUserResponse.Tables[0].Rows[0][Constants.ResultCode_ResultSP].ToString();
                  objGetInformationResponse.ResultDescription = validateUserResponse.Tables[0].Rows[0][Constants.ResultDescription_ResultSP].ToString();
                  objGetInformationResponse.getInformationResponse = null;                  
              }
                ///////
                  _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.GetInformation, 
                      "RESPONSE: [ProcessId: " + timeStampProcess + "] [ResultCode: " + objGetInformationResponse.ResultCode + ". ResultDescription: " + objGetInformationResponse.ResultDescription + "]" +
                      " [GetInformation: " + objGetInformationResponse.getInformationResponse + "]", Util.ErrorTypeEnum.TraceType);            
                //////
                return objGetInformationResponse;
            }
            catch (Exception ex) {
                _util.WriteLog(this.GetType().ToString() + Constants.SeparatorDataLog + Constants.GetInformation, "EXCEPTION: [ProcessId: " + timeStampProcess + "] " + ex.ToString(), Util.ErrorTypeEnum.ErrorType);
                objGetInformationResponse = new EGetInformationResponse{
                    ResultCode = Constants.DefaultErrorCode,
                    ResultDescription = Constants.DefaultErrorDescription,
                    getInformationResponse = null
                };
            return objGetInformationResponse;             
            }
        }

        //private string EncriptaMD5(string password){
        //    System.Security.Cryptography.MD5 md5;
        //    md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        //    Byte[] encodeBytes = md5.ComputeHash(ASCIIEncoding.Default.GetBytes(password));
        //    return System.Text.RegularExpressions.Regex.Replace(BitConverter.ToString(encodeBytes).ToLower(), @"-", "");
        //}
    }
}