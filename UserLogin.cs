using Auth0.ManagementApi.Models;

namespace Calyx_Solutions
{
    public class UserLogin
    {
        public string userEmail { get; set; }
        public string userPassword { get; set; }

        public UserLogin(string useremail, string userpassword) {
            userEmail = useremail;
            userPassword = userpassword;
        }
        
    }
}
