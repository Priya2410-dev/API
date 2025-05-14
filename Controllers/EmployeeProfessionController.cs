using Microsoft.AspNetCore.Mvc;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calyx_Solutions.Controllers
{

    public class EmployeeProfessionController : ControllerBase
    {
        [HttpGet]
        [Route("EmployeeProfession")]
        public IActionResult EmployeeProfession(Model.Transaction obj)
        {
            try
            {
                DataTable dt = new DataTable();
                MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("sp_select_profession");
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.Parameters.AddWithValue("_Client_ID", obj.Client_ID);

                dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);
                List<Dictionary<string, object>> professions = new List<Dictionary<string, object>>();
                foreach (DataRow dr in dt.Rows)
                {
                    Dictionary<string, object> profession = new Dictionary<string, object>();
                    profession["employeeProfessionId"] = dr["ID"];
                    profession["employeeProfessionName"] = dr["Profession"]; ;
                    professions.Add(profession);
                }
                var jsonData = new { response = true, responseCode = "00", data = professions };
                return new JsonResult(jsonData);
            }
            catch (Exception ex)
            {
                var jsonData = new { response = false, responseCode = "02", data = "Invalid Client_ID" };
                return new JsonResult(jsonData);
            }
        }

        // GET api/<EmployeeProfessionController>/5
        [HttpGet("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<EmployeeProfessionController>
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<EmployeeProfessionController>/5
        [HttpPut("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<EmployeeProfessionController>/5
        [HttpDelete("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public void Delete(int id)
        {
        }
    }
}
