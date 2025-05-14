using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calyx_Solutions.Model.response
{
    public class WebResponse
    {
        public static string RESPONSE_STATUS_TYPE_SUCCESS = "success";
        public static string RESPONSE_STATUS_TYPE_TECHNICAL_ERROR = "technicalerror";
        public static string RESPONSE_STATUS_TYPE_VALIDATION_ERROR = "validationerror";

        public string RedirectUrl { get; set; }
        public int ResponseCode { get; set; }
        public string sStatusType { get; set; }
        public string sErrorExceptionText { get; set; }
        public string ResponseMessage { get; set; }
        public string sErrorMessageCode { get; set; }
        public bool hasError { get; set; }
        public string data { get; set; }
        public List<object> Data { set; get; }
        public object ObjData { set; get; }
        public WebResponse(string p_sStatusType)
        {
            sStatusType = p_sStatusType;
            if (p_sStatusType.Equals(RESPONSE_STATUS_TYPE_TECHNICAL_ERROR) || p_sStatusType.Equals(RESPONSE_STATUS_TYPE_VALIDATION_ERROR))
            {
                hasError = true;
            }
        }
    }
}
