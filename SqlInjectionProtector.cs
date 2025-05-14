using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Calyx_Solutions
{
    public class SqlInjectionProtector
    {

        // SQL Injection pattern to check dangerous inputs
        private static readonly string[] SqlInjectionPatterns_ =
        {
        "--", ";--", ";", "@@", "char", "nchar", "varchar", "nvarchar",
        "alter", "begin", "cast", "create", "cursor", "declare", "delete", "drop",
        "end", "exec", "execute", "fetch", "insert", "kill", "select", 
        "sysobjects", "syscolumns", "table", "update", "'", "\"", "''", "`",
         "union",  "xor", "sleep", "benchmark",
        "load_file", "into outfile", "grant", "revoke", "truncate", 
        "information_schema", "database()", "concat"
        };


        private static  string securityValues = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Jwt")["validatevalues"].ToString();
        private static string[] SqlInjectionPatterns = securityValues.Split(',');

        public static bool ReadJsonElementValues(JsonElement jsonElement)
        {
            // If the element is an object, enumerate its properties
            if (jsonElement.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in jsonElement.EnumerateObject())
                {
                   
                    var value =  Convert.ToString(property.Value);
                    var keyName = Convert.ToString(property.Name);
                    if (value != null && ContainsSqlInjection(value) && keyName != "sourceComment"  && keyName != "comment" && keyName != "apiBranchDetails" && keyName != "password" && keyName != "address1" 
                        && keyName != "address2" && keyName != "street" && keyName != "houseNumber" && keyName != "addressLine")
                    {
                        return false; // SQL injection pattern detected
                    }
                }
            }
            return true;
        }
        //Start Parth added for return isValid along with field name
        #region return isValid along with field name
        public static (bool isValid, string fieldName) ReadJsonElementValues_SingleElement(JsonElement jsonElement)
        {
            // If the element is an object, enumerate its properties
            if (jsonElement.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in jsonElement.EnumerateObject())
                {

                    var value = Convert.ToString(property.Value);
                    var keyName = Convert.ToString(property.Name);
                    if (value != null && ContainsSqlInjection(value))
                    {
                        return (false, keyName); // SQL injection pattern detected
                    }
                }
            }
            return (true, string.Empty);
        }
        # endregion return isValid along with field name
        //End Parth added return isValid along with field name

        public static bool ReadJsonElementValuesScript(JsonElement jsonElement)
        {
            // If the element is an object, enumerate its properties
            if (jsonElement.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in jsonElement.EnumerateObject())
                {                    
                    var value = Convert.ToString(property.Value);

                    if (value != null && ContainsScriptSqlInjection(value))
                    {
                        return false; // SQL injection pattern detected
                    }
                }
            }
            return true;
        }

        // JsonObject Read
        public static bool ReadJsonObjectValues(JsonObject jsonObject)
        {
            // Iterate through each key-value pair in the JsonObject
            foreach (var kvp in jsonObject)
            {                
                var value = Convert.ToString(  kvp.Value );
                var keyName = Convert.ToString(kvp.Key);
                if (value != null && ContainsSqlInjection(value) && keyName != "madeThisTransferLabel")
                {
                    return false; // SQL injection pattern detected
                }
            }
            return true;
        }

        public static bool ReadJsonObjectValuesScript(JsonObject jsonObject)
        {
            // Iterate through each key-value pair in the JsonObject
            foreach (var kvp in jsonObject)
            {
                var value = Convert.ToString(kvp.Value);

                if (value != null && ContainsScriptSqlInjection(value))
                {
                    return false; // SQL injection pattern detected
                }
            }
            return true;
        }


        /// <summary>
        /// Validates an object's properties for SQL injection patterns.
        /// </summary>
        /// <param name="obj">The object to validate.</param>
        /// <returns>True if valid, otherwise false.</returns>
        public static bool ValidateObjectForSqlInjection(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            // Get all string properties of the object
            var stringProperties = obj.GetType()
                                      .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                      .Where(p => p.PropertyType == typeof(string));

            // Validate each string property
            foreach (var property in stringProperties)
            {
                var value = property.GetValue(obj) as string;
                var keyName = property.Name as string;
                
                if (value != null && ContainsSqlInjection(value) && keyName != "Comments")
                {
                    return false; // SQL injection pattern detected
                }
            }

            return true; // No SQL injection patterns found
        }

        /// <summary>
        /// Checks if a string contains SQL injection patterns.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>True if SQL injection patterns are detected, otherwise false.</returns>
        private static bool ContainsSqlInjection(string input)
        {
            return SqlInjectionPatterns.Any(pattern => input.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0);
        }


        //*************** For script injections

        private static readonly string[] DangerousPatterns = new[]
         {
        "<script", "javascript:", "onload=", "onerror=", "eval\\(", "<iframe",
        "<img", "alert\\(", "<embed", "<object", "<svg", "expression\\(", "vbscript:",
            "onabort=", "onbeforeunload=", "onblur=", "onchange=", "onclick=", "ondblclick=",
        "onfocus=", "onkeydown=", "onkeypress=", "onkeyup=", "onmousedown=", "onmousemove=",
        "onmouseout=", "onmouseover=", "onmouseup=", "onresize=", "onscroll=", "onsubmit=", "onunload=",    
        "innerHTML", "document.write", "window.location", "setTimeout", "setInterval",
        "XMLHttpRequest", "fetch\\(",    
        "<base href=", "<form action=", "<link rel=", "style=", "<meta http-equiv=\"refresh\"",    
        "data:", "file:", "ftp:", "tel:", "sms:",    
        "<textarea>", "<base>", "<form>", "<details>", "<video>", "<audio>", "<source>",
        "<track>", "<noscript>", "<marquee>",    
        "%3Cscript%3E", "&#x3C;script&#x3E;",    
        "expression\\(", "url(javascript:)", "behavior:", "@import",    
        "&#x000A;", "&#x000D;", "&lt;script&gt;",    
        "window.open", "localStorage", "sessionStorage", "eval(document.cookie)", "window.parent",    
        "%00", "data:text/html;base64,", "<svg><script>alert(1)</script></svg>"
        };

        public static bool ValidateObjectForScriptSqlInjection(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            // Get all string properties of the object
            var stringProperties = obj.GetType()
                                      .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                      .Where(p => p.PropertyType == typeof(string));

            // Validate each string property
            foreach (var property in stringProperties)
            {
                var value = property.GetValue(obj) as string;

                if (value != null && ContainsScriptSqlInjection(value))
                {
                    return false; // SQL injection pattern detected
                }
            }

            return true; // No SQL injection patterns found
        }

        private static bool ContainsScriptSqlInjection(string input)
        {
            return DangerousPatterns.Any(pattern => input.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0);
        }


    }
}
