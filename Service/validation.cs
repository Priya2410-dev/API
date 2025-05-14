using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calyx_Solutions.Service
{
    public class validation
    {
        public static string validate(string str, int isnumeric, int isChar, int isAphaNumeric, int ismobNumber, int isEmail, int isPassword, int isCharspace, int isAphaNumericSpace, int isAphaNumericSpace_address)
        {
            try { if (str != null) { str = str.Trim(); } }
            catch { }
            string msg = "";
            string error = "";
            if (str != "" && str != null)
            {
                if (isnumeric == 0)
                {
                    Regex regex = new Regex(@"^-?[0-9]+$");
                    if (regex.IsMatch(str) == false)
                    { error = "false"; }
                }
                else if (isCharspace == 0)
                {
                    Regex regex = new Regex(@"^([a-zA-Z]+\s)*[a-zA-Z]+$");
                    if (regex.IsMatch(str) == false)
                    { error = "false"; }
                }
                else if (isChar == 0)
                {
                    Regex regex_alpha = new Regex(@"^[a-zA-Z]*$");
                    if (regex_alpha.IsMatch(str) == false)
                    { error = "false"; }
                }
                else if (isAphaNumeric == 0)
                {
                    Regex regex_alpha = new Regex(@"^[a-zA-Z0-9]*$");
                    if (regex_alpha.IsMatch(str) == false)
                    { error = "false"; }
                }
                else if (isAphaNumericSpace == 0)
                {
                    Regex regex_alpha = new Regex(@"^[a-zA-Z0-9 ]*$");
                    if (regex_alpha.IsMatch(str) == false)
                    { error = "false"; }
                }
                else if (isAphaNumericSpace_address == 0)
                {
                    Regex regex_alpha = new Regex(@"^([a-zA-Z0-9,./\'-@& ])*$");
                    if (regex_alpha.IsMatch(str) == false)
                    { error = "false"; }
                }
                else if (ismobNumber == 0)
                {
                    Regex regex_phone_number = new Regex(@"^[0-9][0-9,\.]+$");
                    if (regex_phone_number.IsMatch(str) == false)
                    { error = "false"; }
                }
                else if (isEmail == 0)
                {
                    Regex regex_email = new Regex(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z");
                    if (regex_email.IsMatch(str) == false)
                    { error = "false"; }
                }
                else if (isPassword == 0)
                {
                    Regex regex_email = new Regex(@"/^(?=.*\d)(?=.*[A-Z])(?=.*[^a-zA-Z0-9])(?!.*\s).{8,12}$/;");
                    if (regex_email.IsMatch(str) == false)
                    { error = "false"; }
                }
                //else if (IsPostCode == 0)
                //{
                //    Regex regex_postcode = new Regex(@"/^([Gg][Ii][Rr] 0[Aa]{2})|((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9][A-Za-z]?))))\s?[0-9][A-Za-z]{2})$/;");
                //    if (regex_postcode.IsMatch(str) == false)
                //    { error = "false"; }
                //}

            }
            return error;
        }

        public static string validate_accnm(string str, int isacc_nm)
        {
            try { str = str.Trim(); }
            catch { }
            string msg = "";
            string error = "";
            if (str != "" && str != null)
            {
                if (isacc_nm == 0)
                {
                    //Regex regex = new Regex(@"^[a-zA-Z0-9-_,. ]+$");
                    //Regex regex = new Regex(@'^[a-zA-Z0-9-&@_,.() ]+$');
                    if (str.Contains("=") || str.Contains("#"))//str.Contains("'") ||
                    {
                        error = "false";
                    }
                    else
                    {
                        Regex regex = new Regex(@"^[a-zA-Z0-9-_,. ()']+$"); //(@"^[a-zA-Z0-9-_,. ]+$");
                        if (regex.IsMatch(str) == false)
                        { error = "false"; }
                    }

                }
            }
            return error;
        }

        public static string validate_curcode(string str)
        {
            try { str = str.Trim(); }
            catch { }
            string msg = "";
            string error = "";
            if (str != "")
            {
                //Regex regex = new Regex(@"^[a-zA-Z0-9-_,. ]+$");
                //Regex regex = new Regex(@'^[a-zA-Z0-9-&@_,.() ]+$');
                if (str.Contains("'") || str.Contains("=") || str.Contains("#"))
                {
                    error = "false";
                }
                else if (str.Length > 10)
                {
                    error = "false";
                }
                else
                {
                    Regex regex = new Regex(@"^[a-zA-Z]+$");
                    if (regex.IsMatch(str) == false)
                    { error = "false"; }
                }
            }
            return error;
        }
    }
}
