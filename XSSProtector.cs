using System;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Calyx_Solutions
{
    public class XSSProtector
    {

        // List of potentially dangerous patterns
        private static readonly string[] DangerousPatterns = new[]
         {
        "<script", "javascript:", "onload=", "onerror=", "eval\\(", "<iframe",
        "<img", "alert\\(", "<embed", "<object", "<svg", "expression\\(", "vbscript:",
            "onabort=", "onbeforeunload=", "onblur=", "onchange=", "onclick=", "ondblclick=",
    "onfocus=", "onkeydown=", "onkeypress=", "onkeyup=", "onmousedown=", "onmousemove=",
    "onmouseout=", "onmouseover=", "onmouseup=", "onresize=", "onscroll=", "onsubmit=", "onunload=",

    // Inline Dangerous JavaScript Patterns
    "innerHTML", "document.write", "window.location", "setTimeout", "setInterval",
    "XMLHttpRequest", "fetch\\(",

    // Attributes for Malicious Injection
    "<base href=", "<form action=", "<link rel=", "style=", "<meta http-equiv=\"refresh\"",

    // URI Schemes
    "data:", "file:", "ftp:", "tel:", "sms:",

    // HTML Tags Commonly Exploited for XSS
    "<textarea>", "<base>", "<form>", "<details>", "<video>", "<audio>", "<source>",
    "<track>", "<noscript>", "<marquee>",

    // Encoding Tricks
    "%3Cscript%3E", "&#x3C;script&#x3E;",

    // CSS Injection
    "expression\\(", "url(javascript:)", "behavior:", "@import",

    // Malicious HTML Entities
    "&#x000A;", "&#x000D;", "&lt;script&gt;",

    // DOM-based Injection Vectors
    "window.open", "localStorage", "sessionStorage", "eval(document.cookie)", "window.parent",

    // Other Dangerous Inputs
    "%00", "data:text/html;base64,", "<svg><script>alert(1)</script></svg>"
    };


        /// <summary>
        /// Validates the input for XSS vulnerabilities.
        /// </summary>
        /// <param name="input">The user input to validate.</param>
        /// <returns>True if the input is safe; False if it contains potential XSS.</returns>
        public static bool IsInputSafe(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return true; // Empty input is considered safe

            // Check against each dangerous pattern
            foreach (var pattern in DangerousPatterns)
            {
                if (Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase))
                {
                    return false; // Input is not safe
                }
            }

            return true; // Input is safe
        }

        /// <summary>
        /// Validates all properties of a given object to check for XSS vulnerabilities.
        /// </summary>
        /// <param name="obj">The object to validate.</param>
        /// <returns>True if all properties are safe; False if any property contains potential XSS.</returns>
        public static bool IsObjectSafe(object obj)
        {
            if (obj == null)
                return true; // Null objects are considered safe

            // Check if the object is a collection or an array
            if (obj is IEnumerable collection)
            {
                foreach (var item in collection)
                {
                    if (!IsObjectSafe(item)) // Recursively check collection items
                    {
                        return false; // If any item is unsafe, return false
                    }
                }
            }

            // Check properties of the object
            PropertyInfo[] properties = obj.GetType().GetProperties();
            foreach (var property in properties)
            {
                // Check if the property is of string type
                if (property.PropertyType == typeof(string))
                {
                    string propertyValue = property.GetValue(obj) as string;
                    if (propertyValue != null && !IsInputSafe(propertyValue))
                    {
                        return false; // If any string property contains dangerous patterns, return false
                    }
                }
                else
                {
                    // If the property is a complex object, recursively check its properties
                    var propertyValue = property.GetValue(obj);
                    if (propertyValue != null && !IsObjectSafe(propertyValue))
                    {
                        return false;
                    }
                }
            }

            return true; // All properties are safe
        }

    }
}
