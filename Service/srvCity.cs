using System.Data;

namespace Calyx_Solutions.Service
{
    public class srvCity
    {
        public List<Model.City> Select_City_Master(Model.City _Obj)
        {
            List<Model.City> _lst = new List<Model.City>();

            MySqlConnector.MySqlCommand _cmd = new MySqlConnector.MySqlCommand("sp_select_city_master");
            _cmd.CommandType = CommandType.StoredProcedure;
            //_cmd.Parameters.AddWithValue("_CountryId", _Obj.Country.Id);

            _cmd.Parameters.AddWithValue("_CountryId", _Obj.Country_ID);

            DataTable dt = db_connection.ExecuteQueryDataTableProcedure(_cmd);

            Model.City _obj = new Model.City();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    _obj = new Model.City();
                    // _obj.Country = new Model.Country();

                    if (row["City_ID"] != DBNull.Value)
                    {
                        _obj.Id = Convert.ToInt32(row["City_ID"].ToString());
                    }
                    if (row["City_Name"] != DBNull.Value)
                    {
                        _obj.Name = (row["City_Name"].ToString());
                    }

                    _lst.Add(_obj);
                }
            }
            return _lst;
        }
    }
}
