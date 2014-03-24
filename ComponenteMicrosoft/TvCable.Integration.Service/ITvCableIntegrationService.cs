using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace TvCable.Integration.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ITvCableIntegrationService" in both code and config file together.
    [ServiceContract]
    public interface ITvCableIntegrationService
    {
        
        [OperationContract]
        InstallResponse Install(string userName, string password, string sequence, string smartCards, SettopBox settopBox, string[] packs, string masterCard, 
            string userId, string referenceNumber, string notes, string customerId, string zone);
        
        [OperationContract]
        DisconnectResponse Disconnect(string userName, string password, string sequence, string smartCards, string userId, string referenceNumber, string notes, 
            string customerId, string zone);

        [OperationContract]
        UpdateFirmwareResponse UpdateFirmware(string userName, string password, string sequence, string smartCards, string userId, string referenceNumber,
                                              string notes, string customerId, string zone);

        [OperationContract]
        SendMessageResponse SendMessage(string userName, string password, string sequence, string smartCards, string message, string userId,
                                        string referenceNumber, string notes, string customerId, string zone);

        [OperationContract]
        UpdatePinResponse UpdatePin(string userName, string password, string sequence, string smartCards, string pin, string userId,
                                    string referenceNumber, string notes, string customerId, string zone);

        [OperationContract]
        ReSendKeyResponse ReSendKey(string userName, string password, string sequence, string smartCards, string userId, string referenceNumber,
                                    string notes, string customerId, string zone);

        [OperationContract]
        RemovePacksResponse RemovePacks(string userName, string password, string sequence, string smartCards, string[] packs, string userId,
                                        string referenceNumber, string notes, string customerId, string zone);

        [OperationContract]
        AddPacksResponse AddPacks(string userName, string password, string sequence, string smartCards, string[] packs, string userId,
                                  string referenceNumber, string notes, string customerId, string zone);

        [OperationContract]
        UninstallResponse Uninstall(string userName, string password, string sequence, string smartCards, string userId,
                                    string referenceNumber, string notes, string customerId, string zone);

        [OperationContract]
        ReconnectResponse Reconnect(string userName, string password, string sequence, string smartCards, string[] packs, string userId, string referenceNumber, 
            string notes, string customerId, string zone);

        [OperationContract]
        EstatusResponse Status(string userName, string password);
    
        [OperationContract]
        ReScanChannelsResponse ReScanChannels(string userName, string password, string sequence, string smartCards, string userId, string referenceNumber, string notes, 
            string customerId, string zone);

        [OperationContract]
        AddCarrierResponse AddCarrier(string userName, string password, string sequence, string smartCards, string carrier, string userId, string referenceNumber, 
            string notes, string customerId, string zone);

        [OperationContract]
        ViewSetTopBoxIDResponse ViewSetTopBoxID(string userName, string password, string sequence, string smartCards, string userId, string referenceNumber, 
            string notes, string customerId, string zone);

        [OperationContract]
        ResetSetTopBoxResponse ResetSetTopBox(string userName, string password, string sequence, string smartCards, string userId, string referenceNumber, 
            string notes, string customerId, string zone);

        [OperationContract]
        UpdateCodeZoneResponse UpdateCodeZone(string userName, string password, string sequence, string smartCards, string code, string userId, 
            string referenceNumber, string notes, string customerId, string zone);

        [OperationContract]
        RebootSetTopBoxResponse RebootSetTopBox(string userName, string password, string sequence, string smartCards, string userId, string referenceNumber, 
            string notes, string customerId, string zone);

        [OperationContract]
        ForceUpdateFirmwareResponse ForceUpdateFirmware(string userName, string password, string sequence, string smartCards, string userId, 
            string referenceNumber, string notes, string customerId, string zone);

        [OperationContract]
        UpdateControlParentallResponse UpdateControlParentall(string userName, string password, string sequence, string smartCards, string pin, 
            string userId, string referenceNumber, string notes, string customerId, string zone);

        [OperationContract]
        UpdateMenuPassResponse UpdateMenuPass(string userName, string password, string sequence, string smartCards, string pin, string userId, 
            string referenceNumber, string notes, string customerId, string zone);

        [OperationContract]
        EGetSmartCardsInfoResponse GetSmartCardsInfo(string userName, string password);

        [OperationContract]
        EGetInformationResponse GetInformation(string userName, string password, string sequence, string smartCards, string userId, 
            string referenceNumber, string notes, string customerId, string zone);

    }


    [DataContract]
    public class EGetSmartCardsInfoResponse : ResponseWebTransaction
    {
        TvCable.Integration.Service.WRTuvesService.getSmartcardsInfoResponse objGetSmartcardsInfoResponse;
        /// <summary>
        /// Return
        /// </summary>
        [DataMember]
        public TvCable.Integration.Service.WRTuvesService.getSmartcardsInfoResponse getSmartcardsInfoResponse
        {
            get { return objGetSmartcardsInfoResponse; }
            set { objGetSmartcardsInfoResponse = value; }
        }
    }


    [DataContract]
    public class EGetInformationResponse : ResponseWebTransaction 
    {
        
        TvCable.Integration.Service.WRTuvesService.getInformationResponse  objGetInformationResponse;
        /// <summary>
        /// Return
        /// </summary>
        [DataMember]
        public TvCable.Integration.Service.WRTuvesService.getInformationResponse getInformationResponse 
        {
            get { return objGetInformationResponse; }
            set { objGetInformationResponse = value; }
        } 
    }

    [DataContract]
    public class UpdateMenuPassResponse : ResponseWebTransaction
    {
        string answer;
        /// <summary>
        /// Answer
        /// </summary>
        [DataMember]
        public string Answer
        {
            get { return answer; }
            set { answer = value; }
        }

        string returnValue;
        /// <summary>
        /// Return
        /// </summary>
        [DataMember]
        public string ReturnValue
        {
            get { return returnValue; }
            set { returnValue = value; }
        }
    }

    [DataContract]
    public class UpdateControlParentallResponse : ResponseWebTransaction
    {
        string answer;
        /// <summary>
        /// Answer
        /// </summary>
        [DataMember]
        public string Answer
        {
            get { return answer; }
            set { answer = value; }
        }

        string returnValue;
        /// <summary>
        /// Return
        /// </summary>
        [DataMember]
        public string ReturnValue
        {
            get { return returnValue; }
            set { returnValue = value; }
        }
    }

    [DataContract]
    public class ForceUpdateFirmwareResponse : ResponseWebTransaction
    {
        string answer;
        /// <summary>
        /// Answer
        /// </summary>
        [DataMember]
        public string Answer
        {
            get { return answer; }
            set { answer = value; }
        }

        string returnValue;
        /// <summary>
        /// Return
        /// </summary>
        [DataMember]
        public string ReturnValue
        {
            get { return returnValue; }
            set { returnValue = value; }
        }
    }


    [DataContract]
    public class RebootSetTopBoxResponse : ResponseWebTransaction
    {
        string answer;
        /// <summary>
        /// Answer
        /// </summary>
        [DataMember]
        public string Answer
        {
            get { return answer; }
            set { answer = value; }
        }

        string returnValue;
        /// <summary>
        /// Return
        /// </summary>
        [DataMember]
        public string ReturnValue
        {
            get { return returnValue; }
            set { returnValue = value; }
        }
    }

    [DataContract]
    public class UpdateCodeZoneResponse : ResponseWebTransaction
    {
        string answer;
        /// <summary>
        /// Answer
        /// </summary>
        [DataMember]
        public string Answer
        {
            get { return answer; }
            set { answer = value; }
        }

        string returnValue;
        /// <summary>
        /// Return
        /// </summary>
        [DataMember]
        public string ReturnValue
        {
            get { return returnValue; }
            set { returnValue = value; }
        }
    }

    [DataContract]
    public class ResetSetTopBoxResponse : ResponseWebTransaction {
        string answer;
        /// <summary>
        /// Answer
        /// </summary>
        [DataMember]
        public string Answer
        {
            get { return answer; }
            set { answer = value; }
        }

        string returnValue;
        /// <summary>
        /// Return
        /// </summary>
        [DataMember]
        public string ReturnValue
        {
            get { return returnValue; }
            set { returnValue = value; }
        }    
    }

    [DataContract]
    public class ViewSetTopBoxIDResponse : ResponseWebTransaction{
         string answer;
        /// <summary>
        /// Answer
        /// </summary>
        [DataMember]
        public string Answer {
            get { return answer; }
            set { answer = value; }
        }

        string returnValue;
        /// <summary>
        /// Return
        /// </summary>
        [DataMember]
        public string ReturnValue {
            get { return returnValue; }
            set { returnValue = value; }
        }    
    }

    [DataContract]
    public class AddCarrierResponse : ResponseWebTransaction{
            string answer;
        /// <summary>
        /// Answer
        /// </summary>
        [DataMember]
        public string Answer {
            get { return answer; }
            set { answer = value; }
        }

        string returnValue;
        /// <summary>
        /// Return
        /// </summary>
        [DataMember]
        public string ReturnValue {
            get { return returnValue; }
            set { returnValue = value; }
        }    
    }


    [DataContract]
    public class ReScanChannelsResponse : ResponseWebTransaction {
        string answer;
        /// <summary>
        /// Answer
        /// </summary>
        [DataMember]
        public string Answer {
            get { return answer; }
            set { answer = value; }
        }

        string returnValue;
        /// <summary>
        /// Return
        /// </summary>
        [DataMember]
        public string ReturnValue {
            get { return returnValue; }
            set { returnValue = value; }
        }
    }


    [DataContract]
    public class UpdateFirmwareResponse : ResponseWebTransaction
    {
        string answer;
        /// <summary>
        /// Answer
        /// </summary>
        [DataMember]
        public string Answer
        {
            get { return answer; }
            set { answer = value; }
        }

        string returnValue;
        /// <summary>
        /// Return
        /// </summary>
        [DataMember]
        public string ReturnValue
        {
            get { return returnValue; }
            set { returnValue = value; }
        }
    }

    [DataContract]
    public class SendMessageResponse : ResponseWebTransaction
    {
        string answer;
        /// <summary>
        /// Answer
        /// </summary>
        [DataMember]
        public string Answer
        {
            get { return answer; }
            set { answer = value; }
        }

        string returnValue;
        /// <summary>
        /// Return
        /// </summary>
        [DataMember]
        public string ReturnValue
        {
            get { return returnValue; }
            set { returnValue = value; }
        }
    }

    [DataContract]
    public class UpdatePinResponse : ResponseWebTransaction
    {
        string answer;
        /// <summary>
        /// Answer
        /// </summary>
        [DataMember]
        public string Answer
        {
            get { return answer; }
            set { answer = value; }
        }

        string returnValue;
        /// <summary>
        /// Return
        /// </summary>
        [DataMember]
        public string ReturnValue
        {
            get { return returnValue; }
            set { returnValue = value; }
        }
    }

    [DataContract]
    public class ReSendKeyResponse : ResponseWebTransaction
    {
        string answer;
        /// <summary>
        /// Answer
        /// </summary>
        [DataMember]
        public string Answer
        {
            get { return answer; }
            set { answer = value; }
        }

        string returnValue;
        /// <summary>
        /// Return
        /// </summary>
        [DataMember]
        public string ReturnValue
        {
            get { return returnValue; }
            set { returnValue = value; }
        }
    }

    [DataContract]
    public class RemovePacksResponse : ResponseWebTransaction
    {
        string answer;
        /// <summary>
        /// Answer
        /// </summary>
        [DataMember]
        public string Answer
        {
            get { return answer; }
            set { answer = value; }
        }

        string returnValue;
        /// <summary>
        /// Return
        /// </summary>
        [DataMember]
        public string ReturnValue
        {
            get { return returnValue; }
            set { returnValue = value; }
        }
    }

    [DataContract]
    public class AddPacksResponse : ResponseWebTransaction
    {
        string answer;
        /// <summary>
        /// Answer
        /// </summary>
        [DataMember]
        public string Answer
        {
            get { return answer; }
            set { answer = value; }
        }

        string returnValue;
        /// <summary>
        /// Return
        /// </summary>
        [DataMember]
        public string ReturnValue
        {
            get { return returnValue; }
            set { returnValue = value; }
        }
    }

    [DataContract]
    public class UninstallResponse : ResponseWebTransaction
    {
        string answer;
        /// <summary>
        /// Answer
        /// </summary>
        [DataMember]
        public string Answer
        {
            get { return answer; }
            set { answer = value; }
        }

        string returnValue;
        /// <summary>
        /// Return
        /// </summary>
        [DataMember]
        public string ReturnValue
        {
            get { return returnValue; }
            set { returnValue = value; }
        }
    }

    [DataContract]
    public class ReconnectResponse : ResponseWebTransaction
    {
        string answer;
        /// <summary>
        /// Answer
        /// </summary>
        [DataMember]
        public string Answer
        {
            get { return answer; }
            set { answer = value; }
        }

        string returnValue;
        /// <summary>
        /// Return
        /// </summary>
        [DataMember]
        public string ReturnValue
        {
            get { return returnValue; }
            set { returnValue = value; }
        }
    }

    [DataContract]
    public class DisconnectResponse : ResponseWebTransaction
    {
        string answer;
        /// <summary>
        /// Answer
        /// </summary>
        [DataMember]
        public string Answer
        {
            get { return answer; }
            set { answer = value; }
        }

        string returnValue;
        /// <summary>
        /// Return
        /// </summary>
        [DataMember]
        public string ReturnValue
        {
            get { return returnValue; }
            set { returnValue = value; }
        }
    }

    [DataContract]
    public class InstallResponse : ResponseWebTransaction
    {
        string answer;
        /// <summary>
        /// Answer
        /// </summary>
        [DataMember]
        public string Answer
        {
            get { return answer; }
            set { answer = value; }
        }

        string returnValue;
        /// <summary>
        /// Return
        /// </summary>
        [DataMember]
        public string ReturnValue
        {
            get { return returnValue; }
            set { returnValue = value; }
        }
    }


    /// <summary>
    /// Response for an Status
    /// </summary>
    [DataContract]
    public class EstatusResponse : ResponseWebTransaction
    {
        string statusWebservice;
        /// <summary>
        /// Status Webservice
        /// </summary>
        [DataMember]
        public string StatusWebservice
        {
            get { return statusWebservice; }
            set { statusWebservice = value; }
        }

        string description;
        /// <summary>
        /// Description Webservice
        /// </summary>
        [DataMember]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

    }

    /// <summary>
    /// Object SettopBox
    /// </summary>
    [DataContract]
    public class SettopBox
    {
        string code;
        /// <summary>
        /// Code
        /// </summary>
        [DataMember]
        public string Code
        {
            get { return code; }
            set { code = value; }
        }

        string brand;
        /// <summary>
        /// Brand
        /// </summary>
        [DataMember]
        public string Brand
        {
            get { return brand; }
            set { brand = value; }
        }

        string model;
        /// <summary>
        /// Model
        /// </summary>
        [DataMember]
        public string Model
        {
            get { return model; }
            set { model = value; }
        }

        string firmware;
        /// <summary>
        /// Firmware
        /// </summary>
        [DataMember]
        public string Firmware
        {
            get { return firmware; }
            set { firmware = value; }
        }
    }   

    /// <summary>
    /// Object SettopBox
    /// </summary>
    [DataContract]
    public class Packs
    {
        string paks;
        /// <summary>
        /// Paks
        /// </summary>
        [DataMember]
        public string Paks
        {
            get { return paks; }
            set { paks = value; }
        }
    }


    /// <summary>
    /// Generic Response for a transaction
    /// </summary>
    [DataContract]
    public class ResponseWebTransaction
    {
        string resultCode;
        /// <summary>
        /// Result code of transaction, zero (0) means successful transaction, any value different to zero means error
        /// </summary>
        [DataMember]
        public string ResultCode
        {
            get { return resultCode; }
            set { resultCode = value; }
        }

        string resultDescription;
        /// <summary>
        /// String representing a result description
        /// </summary>
        [DataMember]
        public string ResultDescription
        {
            get { return resultDescription; }
            set { resultDescription = value; }
        }
    }
}