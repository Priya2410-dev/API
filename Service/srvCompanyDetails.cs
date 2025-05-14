using System.Data;
using System.Reflection;
using System.Web.Helpers;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Reflection;
using Calyx_Solutions.Controllers;
 
namespace Calyx_Solutions.Service
{
    public class srvCompanyDetails
    {
        private readonly HttpContext _userLoginController;
        public srvCompanyDetails(HttpContext context)
        {
            _userLoginController = context;
        }

        static int captcnt = 0;
          public List<Model.CompanyDetails> IsValidAPI(int Client_ID, string APIAccess_Code, string APIUser_ID)
          {

        List<Model.CompanyDetails> _lst = new List<Model.CompanyDetails>();
              //APIAccess_Code = APIAccess_Code.Replace(" ", "");
              string isInjected = CompanyInfo.testInjection(APIAccess_Code);
              int ij = 0;
              if (isInjected == "0")
              {
                  ij++;
              }
              isInjected = CompanyInfo.testInjection(Convert.ToString(Client_ID));
              if (isInjected == "0")
              {
                  ij++;
              }
              isInjected = CompanyInfo.testInjection(Convert.ToString(APIUser_ID));
              if (isInjected == "0")
              {
                  ij++;
              }
              if (ij == 0)
              {
                   
                  DataTable dt = CompanyInfo.get(Client_ID, _userLoginController );
                  Model.CompanyDetails _obj = new Model.CompanyDetails();
                  if (dt != null && dt.Rows.Count > 0)
                  {
                      if (dt.Rows[0]["APIUser_ID"] != DBNull.Value)
                      {
                          _obj.APIUser_ID = (dt.Rows[0]["APIUser_ID"].ToString());
                      }
                      if (dt.Rows[0]["APIAccessCode"] != DBNull.Value)
                      {
                          _obj.APIAccess_Code = (dt.Rows[0]["APIAccessCode"].ToString());
                      }

                      if (_obj.APIAccess_Code == APIAccess_Code && _obj.APIUser_ID == APIUser_ID)
                      {
                          _obj.ResponseCode = 0;
                      }
                      else if (_obj.APIAccess_Code == APIAccess_Code && _obj.APIUser_ID != APIUser_ID)
                      {
                          _obj.ResponseCode = 1;
                          _obj.ResponseMessage = "API User ID is Incorrect";
                      }
                      else if (_obj.APIAccess_Code != APIAccess_Code && _obj.APIUser_ID == APIUser_ID)
                      {
                          _obj.ResponseCode = 2;
                          _obj.ResponseMessage = "API Access Code is Incorrect";
                      }
                      else
                      {
                          _obj.ResponseCode = 2;
                          _obj.ResponseMessage = "Incorrect API Credentials";
                      }
                      _lst.Add(_obj);
                  }
              }
              return _lst;
          }
          public List<Model.CompanyDetails> getInfo(Model.Login obj)
          {
              captcnt++;
              List<Model.CompanyDetails> _lst = new List<Model.CompanyDetails>();
              string Client_ID1_regex = CompanyInfo.testInjection(Convert.ToString(obj.Client_ID));
              String Client_ID_regex = validation.validate(Convert.ToString(obj.Client_ID), 0, 1, 1, 1, 1, 1, 1, 1, 1);
              string base_cur_code = "true", base_cur_code1 = "1", base_cur_code_len = "1";
              if (obj.Base_Currency_code != "" && obj.Base_Currency_code != null)
              {
                  base_cur_code = validation.validate_curcode(Convert.ToString(obj.Base_Currency_code));
                  base_cur_code1 = CompanyInfo.testInjection(Convert.ToString(obj.Base_Currency_code));
                  if (obj.Base_Currency_code.Length != 3)
                  {
                      base_cur_code_len = "0";
                  }
              }
              Model.CompanyDetails _obj = new Model.CompanyDetails();
              if (Client_ID1_regex == "1" && Client_ID_regex != "false" && base_cur_code1 == "1" && base_cur_code != "false" && base_cur_code_len == "1")
              {
                  DataTable dt = (DataTable)CompanyInfo.GetBaseCurrencywisebankdetails(obj.Client_ID, obj.Base_Currency_code,0,0);

                  if (dt != null && dt.Rows.Count > 0)
                  {
                      _lst = ConvertDataTable<Model.CompanyDetails>(dt);
                  }
              }
              else
              {
                  string msg1 = "Validation Error";
                  string msg = "Validation Error Client_ID_regex- +" + Client_ID_regex + " base_cur_code - +" + base_cur_code;
                  int stattus = (int)CompanyInfo.InsertActivityLogDetailsSecurity(msg, Convert.ToInt32(obj.User_ID), 0, Convert.ToInt32(obj.User_ID), 1, "srvCompanyDetails", Convert.ToInt32(obj.Branch_ID), Convert.ToInt32(obj.Client_ID), "getInfo", 0, _userLoginController);
                  _obj.ResponseCode = 20;
                  _obj.ResponseMessage = msg1;
                  _lst.Add(_obj);
              }
              return _lst;
          }

          private static List<T> ConvertDataTable<T>(DataTable dt)
          {
              List<T> data = new List<T>();
              foreach (DataRow row in dt.Rows)
              {
                  T item = GetItem<T>(row);
                  data.Add(item);
              }
              return data;
          }
          private static T GetItem<T>(DataRow dr)
          {
              Type temp = typeof(T);
              T obj = Activator.CreateInstance<T>();

              foreach (DataColumn column in dr.Table.Columns)
              {
                  foreach (PropertyInfo pro in temp.GetProperties())
                  {
                      if (pro.Name == column.ColumnName)
                          pro.SetValue(obj, Convert.ChangeType(dr[column.ColumnName], pro.PropertyType), null);
                      else
                          continue;
                  }
              }
              return obj;
          }
      
    }

}
