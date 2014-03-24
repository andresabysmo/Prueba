
namespace TvCable.Integration.DTO
{
    public class Constants
    {
        #region Generales
        public const string DefaultCodeCasOkCode = "0";
        //
        public const string DefaultOkCode = "0";
        public const string DefaultOkDescription = "Petición Enviada";
        //
        public const string DefaultOkCodeStatus = "1";        
        //
        public const string DefaultErrorCode = "9";
        public const string DefaultErrorDescription = "Transaction Error";        
        //
        public const string SeparatorDataLog = " - ";
        public const string ResultCode_ResultSP = "RESULT_CODE";
        public const string ResultDescription_ResultSP = "RESULT_DESCRIPTION";
        #endregion

        #region Nombre de los metodos
        public const string StatusMetodo = "status";
        public const string InstallMetodo = "install";
        public const string Disconnect = "disconnect";
        public const string Reconnect = "reconnect";
        public const string Uninstall = "uninstall";
        public const string AddPacks = "addPacks";
        public const string RemovePacks = "removePacks";
        public const string ReSendKey = "reSendKey";
        public const string UpdatePin = "updatePIN";
        public const string SendMessage = "sendMessage";
        public const string UpdateFirmware = "updateFirmware";

        public const string ReScanChannels = "rescanChannels";
        public const string AddCarrier = "addCarrier";
        public const string ViewSetTopBoxID = "viewSetTopBoxID";
        public const string ResetSetTopBox = "resetSetTopBox";
        public const string UpdateCodeZone = "updateCodeZone";
        public const string RebootSetTopBox = "rebootSetTopBox";
        public const string ForceUpdateFirmware = "forceUpdateFirmware";
        public const string UpdateControlParentall = "updateControlParental";
        public const string UpdateMenuPass = "updateMenuPass";

        public const string GetSmartCardsInfo = "getSmartcardsInfo";
        public const string GetInformation = "getInformation";

        #endregion

        #region Procedimientos almacenados
        public const string SpValidateUserLogin = "ValidateUserLogin";
        public const string SpGetDescriptionAnswer = "GetDescriptionAnswer";
        #endregion
    }
}
