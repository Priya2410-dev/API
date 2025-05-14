using Calyx_Solutions.Service;
using System.Collections.Concurrent;

namespace Calyx_Solutions.Service
{
    public class srvCaptcha
    {
        private static string _generatedCaptcha = string.Empty;
        private static readonly Random _random = new Random();
        private static readonly ConcurrentDictionary<string, string> _captchaStorage = new ConcurrentDictionary<string, string>();
        //Generate Captcha with username
        public (string captcha, string userName) GenerateCaptcha_withUserName(string userName)
        {
            const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";

            var captcha = string.Empty;
            captcha += uppercaseChars[_random.Next(uppercaseChars.Length)];
            captcha += lowercaseChars[_random.Next(lowercaseChars.Length)];
            captcha += digits[_random.Next(digits.Length)];

            var allChars = uppercaseChars + lowercaseChars + digits;
            for (int i = captcha.Length; i < 6; i++)
            {
                captcha += allChars[_random.Next(allChars.Length)];
            }

            string shuffledCaptcha = ShuffleCaptcha(captcha);

            // Store CAPTCHA safely using ConcurrentDictionary
            _captchaStorage[userName] = shuffledCaptcha;

            return (shuffledCaptcha, userName);
        }

        //Verify Captcha with username
        public bool VerifyCaptcha_withUserName(string userName, string userInput)
        {
            if (_captchaStorage.TryGetValue(userName, out var storedCaptcha))
            {
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(storedCaptcha))
                {
                    return false;
                }
                return userInput == storedCaptcha;
            }

            return false; // If no CAPTCHA is stored for the user
        }

        //Generate Captcha without username
        public string GenerateCaptcha()
        {
            const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";

            var captcha = string.Empty;
            captcha += uppercaseChars[_random.Next(uppercaseChars.Length)];
            captcha += lowercaseChars[_random.Next(lowercaseChars.Length)];
            captcha += digits[_random.Next(digits.Length)];

            var allChars = uppercaseChars + lowercaseChars + digits;
            for (int i = captcha.Length; i < 6; i++)
            {
                captcha += allChars[_random.Next(allChars.Length)];
            }

            _generatedCaptcha = ShuffleCaptcha(captcha);
            return _generatedCaptcha;
        }

        public bool VerifyCaptcha(string userInput)
        {
            if (string.IsNullOrEmpty(userInput) || string.IsNullOrEmpty(_generatedCaptcha))
            {
                return false;
            }
            return userInput == _generatedCaptcha;
        }

        private string ShuffleCaptcha(string captcha)
        {
            return new string(captcha.ToCharArray().OrderBy(_ => _random.Next()).ToArray());
        }
    }
}