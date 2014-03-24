using System.Web.Services;

namespace TvCable.Integration.Service
{
    /// <summary>
    /// Summary description for TvCableIntegrationServiceInternal
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class TvCableIntegrationServiceInternal : System.Web.Services.WebService
    {
        TvCableIntegrationService _objServiceIntegration = new TvCableIntegrationService();

        [WebMethod]
        public string Install(string userName, string password, string sequence, string smartCards, string codeSettopBox, string brandSettopBox,
            string modelSettopBox, string firmwareSettopBox, string pack1, string pack2, string pack3, string masterCard, string userId, string referenceNumber, string notes,
            string customerId, string zone)
        {
            var numPacks = 0;
            var posPack = 0;
            //Code ERA
            var objSettopBox = new SettopBox
            {
                Code = codeSettopBox,
                Brand = brandSettopBox,
                Model = modelSettopBox,
                Firmware = firmwareSettopBox
            };
            if (!string.IsNullOrEmpty(pack1))
                numPacks++;
            if (!string.IsNullOrEmpty(pack2))
                numPacks++;
            if (!string.IsNullOrEmpty(pack3))
                numPacks++;

            var objPacks = new string[numPacks];

            if (!string.IsNullOrEmpty(pack1))
            {
                objPacks[posPack] = pack1;
                posPack++;
            }
            if (!string.IsNullOrEmpty(pack2))
            {
                objPacks[posPack] = pack2;
                posPack++;
            }
            if (!string.IsNullOrEmpty(pack3))
            {
                objPacks[posPack] = pack3;
                posPack++;
            }

            var resultStatus = _objServiceIntegration.Install(userName, password, sequence, smartCards, objSettopBox, objPacks, masterCard, userId, referenceNumber, notes, customerId, zone);

            return "ResultCode: " + resultStatus.ResultCode + ". ResultDescription: " + resultStatus.ResultDescription +
                ". Answer: " + resultStatus.Answer + ". ReturnValue: " + resultStatus.ReturnValue;                   
        }

        [WebMethod]
        public string Disconnect(string userName, string password, string sequence, string smartCards, string userId, string referenceNumber, 
            string notes, string customerId, string zone)
        {
            var resultStatus = _objServiceIntegration.Disconnect(userName, password, sequence, smartCards, userId, referenceNumber, notes, customerId, zone);
            return "ResultCode: " + resultStatus.ResultCode + ". ResultDescription: " + resultStatus.ResultDescription +
              ". Answer: " + resultStatus.Answer + ". ReturnValue: " + resultStatus.ReturnValue;                   
        }

        [WebMethod]
        public string Reconnect(string userName, string password, string sequence, string smartCards, string pack1, string pack2, string pack3, string userId,
            string referenceNumber, string notes, string customerId, string zone) { 
        
            var numPacks = 0;
            var posPack = 0;

            if (!string.IsNullOrEmpty(pack1))
                numPacks++;
            if (!string.IsNullOrEmpty(pack2))
                numPacks++;
            if (!string.IsNullOrEmpty(pack3))
                numPacks++;

            var objPacks = new string[numPacks];

            if (!string.IsNullOrEmpty(pack1)){
                objPacks[posPack] = pack1;
                posPack++;
            }

            if (!string.IsNullOrEmpty(pack2)){
                objPacks[posPack] = pack2;
                posPack++;
            }
            if (!string.IsNullOrEmpty(pack3)){
                objPacks[posPack] = pack3;
                posPack++;
            }

            var resultStatus = _objServiceIntegration.Reconnect(userName, password, sequence, smartCards, objPacks, userId, referenceNumber, notes, customerId, zone);

            return "ResultCode: " + resultStatus.ResultCode + ". ResultDescription: " + resultStatus.ResultDescription +
            ". Answer: " + resultStatus.Answer + ". ReturnValue: " + resultStatus.ReturnValue;                             

        }

        [WebMethod]
        public string Uninstall(string userName, string password, string sequence, string smartCards, string userId, string referenceNumber, 
            string notes, string customerId, string zone) {
            var resultStatus = _objServiceIntegration.Uninstall(userName, password, sequence, smartCards, userId, referenceNumber, notes, customerId, zone);
            return "ResultCode: " + resultStatus.ResultCode + ". ResultDescription: " + resultStatus.ResultDescription +
            ". Answer: " + resultStatus.Answer + ". ReturnValue: " + resultStatus.ReturnValue;                          
        }

        [WebMethod]
        public string AddPacks(string userName, string password, string sequence, string smartCards, string pack1, string pack2, string pack3, 
            string userId, string referenceNumber, string notes, string customerId, string zone) {
            var numPacks = 0;
            var posPack = 0;

            if (!string.IsNullOrEmpty(pack1))
                numPacks++;
            if (!string.IsNullOrEmpty(pack2))
                numPacks++;
            if (!string.IsNullOrEmpty(pack3))
                numPacks++;

            var objPacks = new string[numPacks];

            if (!string.IsNullOrEmpty(pack1))
            {
                objPacks[posPack] = pack1;
                posPack++;
            }

            if (!string.IsNullOrEmpty(pack2))
            {
                objPacks[posPack] = pack2;
                posPack++;
            }
            if (!string.IsNullOrEmpty(pack3))
            {
                objPacks[posPack] = pack3;
                posPack++;
            }

            var resultStatus = _objServiceIntegration.AddPacks(userName, password, sequence, smartCards, objPacks, userId, referenceNumber, notes, customerId, zone);

            return "ResultCode: " + resultStatus.ResultCode + ". ResultDescription: " + resultStatus.ResultDescription +
           ". Answer: " + resultStatus.Answer + ". ReturnValue: " + resultStatus.ReturnValue;                                
        }

        [WebMethod]
        public string RemovePacks(string userName, string password, string sequence, string smartCards, string pack1, string pack2, string pack3, 
            string userId, string referenceNumber, string notes, string customerId, string zone) {
            var numPacks = 0;
            var posPack = 0;

            if (!string.IsNullOrEmpty(pack1))
                numPacks++;
            if (!string.IsNullOrEmpty(pack2))
                numPacks++;
            if (!string.IsNullOrEmpty(pack3))
                numPacks++;

            var objPacks = new string[numPacks];

            if (!string.IsNullOrEmpty(pack1))
            {
                objPacks[posPack] = pack1;
                posPack++;
            }

            if (!string.IsNullOrEmpty(pack2))
            {
                objPacks[posPack] = pack2;
                posPack++;
            }
            if (!string.IsNullOrEmpty(pack3))
            {
                objPacks[posPack] = pack3;
                posPack++;
            }

            var resultStatus = _objServiceIntegration.RemovePacks(userName, password, sequence, smartCards, objPacks, userId, referenceNumber, notes, customerId, zone);

            return "ResultCode: " + resultStatus.ResultCode + ". ResultDescription: " + resultStatus.ResultDescription +
           ". Answer: " + resultStatus.Answer + ". ReturnValue: " + resultStatus.ReturnValue;                   
        }

        [WebMethod]
        public string ReSendKey(string userName, string password, string sequence, string smartCards, string userId, string referenceNumber, 
            string notes, string customerId, string zone){
            var resultStatus = _objServiceIntegration.ReSendKey(userName, password, sequence, smartCards, userId, referenceNumber, notes, customerId, zone);
            return "ResultCode: " + resultStatus.ResultCode + ". ResultDescription: " + resultStatus.ResultDescription +
           ". Answer: " + resultStatus.Answer + ". ReturnValue: " + resultStatus.ReturnValue;                                      
        }

        [WebMethod]
        public string UpdatePin(string userName, string password, string sequence, string smartCards, string pin, string userId, string referenceNumber, 
            string notes, string customerId, string zone) {
            var resultStatus = _objServiceIntegration.UpdatePin(userName, password, sequence, smartCards, pin, userId, referenceNumber, notes, customerId, zone);
               return "ResultCode: " + resultStatus.ResultCode + ". ResultDescription: " + resultStatus.ResultDescription +
              ". Answer: " + resultStatus.Answer + ". ReturnValue: " + resultStatus.ReturnValue;                      
        }

        [WebMethod]
        public string SendMessage(string userName, string password, string sequence, string smartCards, string message, string userId, 
            string referenceNumber, string notes, string customerId, string zone) {
            var resultStatus = _objServiceIntegration.SendMessage(userName, password, sequence, smartCards, message, userId, referenceNumber, notes, customerId, zone);
            return "ResultCode: " + resultStatus.ResultCode + ". ResultDescription: " + resultStatus.ResultDescription +
           ". Answer: " + resultStatus.Answer + ". ReturnValue: " + resultStatus.ReturnValue;                   
        }

        [WebMethod]
        public string UpdateFirmware(string userName, string password, string sequence, string smartCards, string userId, string referenceNumber, 
            string notes, string customerId, string zone) {
            var resultStatus = _objServiceIntegration.UpdateFirmware(userName, password, sequence, smartCards, userId, referenceNumber, notes, customerId, zone);
            return "ResultCode: " + resultStatus.ResultCode + ". ResultDescription: " + resultStatus.ResultDescription +
           ". Answer: " + resultStatus.Answer + ". ReturnValue: " + resultStatus.ReturnValue;                   
        }

        [WebMethod]
        public string ReScanChannels(string userName, string password, string sequence, string smartCards, string userId, string referenceNumber, 
            string notes, string customerId, string zone) {
            var resultStatus = _objServiceIntegration.ReScanChannels(userName, password, sequence, smartCards, userId, referenceNumber, notes, customerId, zone);
            return "ResultCode: " + resultStatus.ResultCode + ". ResultDescription: " + resultStatus.ResultDescription +
           ". Answer: " + resultStatus.Answer + ". ReturnValue: " + resultStatus.ReturnValue;                   
        }

        [WebMethod]
        public string AddCarrier(string userName, string password, string sequence, string smartCards, string carrier, string userId,
            string referenceNumber, string notes, string customerId, string zone) {
            var resultStatus = _objServiceIntegration.AddCarrier(userName, password, sequence, smartCards, carrier, userId, referenceNumber, notes, customerId, zone);
            return "ResultCode: " + resultStatus.ResultCode + ". ResultDescription: " + resultStatus.ResultDescription +
           ". Answer: " + resultStatus.Answer + ". ReturnValue: " + resultStatus.ReturnValue;                   
        }

        [WebMethod]
        public string ViewSetTopBoxID(string userName, string password, string sequence, string smartCards, string userId, 
            string referenceNumber, string notes, string customerId, string zone){
            var resultStatus = _objServiceIntegration.ViewSetTopBoxID(userName, password, sequence, smartCards, userId, referenceNumber, notes, customerId, zone);
            return "ResultCode: " + resultStatus.ResultCode + ". ResultDescription: " + resultStatus.ResultDescription +
           ". Answer: " + resultStatus.Answer + ". ReturnValue: " + resultStatus.ReturnValue;                   
        }

        [WebMethod]
        public string ResetSetTopBox(string userName, string password, string sequence, string smartCards, string userId, string referenceNumber, 
            string notes, string customerId, string zone) {
            var resultStatus = _objServiceIntegration.ResetSetTopBox(userName, password, sequence, smartCards, userId, referenceNumber, notes, customerId, zone);
            return "ResultCode: " + resultStatus.ResultCode + ". ResultDescription: " + resultStatus.ResultDescription +
            ". Answer: " + resultStatus.Answer + ". ReturnValue: " + resultStatus.ReturnValue;                   
        }

        [WebMethod]
        public string UpdateCodeZone(string userName, string password, string sequence, string smartCards, string code, string userId, 
            string referenceNumber, string notes, string customerId, string zone) {
            var resultStatus = _objServiceIntegration.UpdateCodeZone(userName, password, sequence, smartCards, code, userId, referenceNumber, notes, customerId, zone);
            return "ResultCode: " + resultStatus.ResultCode + ". ResultDescription: " + resultStatus.ResultDescription +
            ". Answer: " + resultStatus.Answer + ". ReturnValue: " + resultStatus.ReturnValue;                   
        }
        
        [WebMethod]
        public string RebootSetTopBox(string userName, string password, string sequence, string smartCards, string userId, 
            string referenceNumber, string notes, string customerId, string zone) {
            var resultStatus = _objServiceIntegration.RebootSetTopBox(userName, password, sequence, smartCards, userId, referenceNumber, notes, customerId, zone);
            return "ResultCode: " + resultStatus.ResultCode + ". ResultDescription: " + resultStatus.ResultDescription +
           ". Answer: " + resultStatus.Answer + ". ReturnValue: " + resultStatus.ReturnValue;                   
        }

        [WebMethod]
        public string ForceUpdateFirmware(string userName, string password, string sequence, string smartCards, string userId, string referenceNumber, 
            string notes, string customerId, string zone) {
            var resultStatus = _objServiceIntegration.ForceUpdateFirmware(userName, password, sequence, smartCards, userId, referenceNumber, notes, customerId, zone);
            return "ResultCode: " + resultStatus.ResultCode + ". ResultDescription: " + resultStatus.ResultDescription +
           ". Answer: " + resultStatus.Answer + ". ReturnValue: " + resultStatus.ReturnValue;                   
        }

        [WebMethod]
        public string UpdateControlParentall(string userName, string password, string sequence, string smartCards, string pin, string userId, 
            string referenceNumber, string notes, string customerId, string zone) {
            var resultStatus = _objServiceIntegration.UpdateControlParentall(userName, password, sequence, smartCards, pin, userId, referenceNumber, notes, customerId, zone);
            return "ResultCode: " + resultStatus.ResultCode + ". ResultDescription: " + resultStatus.ResultDescription +
           ". Answer: " + resultStatus.Answer + ". ReturnValue: " + resultStatus.ReturnValue;                   
        }

        [WebMethod]
        public string UpdateMenuPass(string userName, string password, string sequence, string smartCards, string pin, string userId, 
            string referenceNumber, string notes, string customerId, string zone) {
            var resultStatus = _objServiceIntegration.UpdateMenuPass(userName, password, sequence, smartCards, pin, userId, referenceNumber, notes, customerId, zone);
            return "ResultCode: " + resultStatus.ResultCode + ". ResultDescription: " + resultStatus.ResultDescription +
           ". Answer: " + resultStatus.Answer + ". ReturnValue: " + resultStatus.ReturnValue;                   
        }

        [WebMethod]
        public string Status(string usuario, string password)
        {
            var resultStatus = _objServiceIntegration.Status(usuario, password);
            return "ResultCode: " + resultStatus.ResultCode + ". ResultDescription: " + resultStatus.ResultDescription +
                   ". StatusWebservice: " + resultStatus.StatusWebservice + ". ResultDescription: " + resultStatus.Description;
        }       

        [WebMethod]
        public string GetSmartCardsInfo(string usuario, string password) {
            var resultStatus = _objServiceIntegration.GetSmartCardsInfo(usuario, password);
            return "ResultCode: " + resultStatus.ResultCode + ". ResultDescription: " + resultStatus.ResultDescription +
                   ". getSmartCardsInfo: " + resultStatus.getSmartcardsInfoResponse;                

        }

        [WebMethod]
        public string GetInformation(string userName, string password, string sequence, string smartCards, string userId, string referenceNumber, 
            string notes, string customerId, string zone) {
            var resultStatus = _objServiceIntegration.GetInformation(userName, password, sequence, smartCards, userId, referenceNumber, notes, customerId, zone);
            return "ResultCode: " + resultStatus.ResultCode + ". ResultDescription: " + resultStatus.ResultDescription +
                ". GetInformation: " + resultStatus.getInformationResponse;            
        }

        //[WebMethod]
        //public string EncriptaMD5(string password){ 
        //    return "Resultdado: " + _objServideIntegration.EncriptaMD5(password);
        //}

    }
}
