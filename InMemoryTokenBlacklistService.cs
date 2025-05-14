namespace Calyx_Solutions
{
    public interface ITokenBlacklistService
    {
        void BlacklistToken(string jti, DateTime expiry);
        bool IsTokenBlacklisted(string jti);
    }

    public class InMemoryTokenBlacklistService : ITokenBlacklistService
    {
        private readonly Dictionary<string, DateTime> _blacklist = new();

        public void BlacklistToken(string jti, DateTime expiry)
        {
            _blacklist[jti] = expiry;
        }

        public bool IsTokenBlacklisted(string jti)
        {
            try
            {
                if (_blacklist.TryGetValue(jti, out var expiry))
                {
                    if (DateTime.UtcNow < expiry) return true;
                    _blacklist.Remove(jti); // cleanup
                }
                
            }
            catch { }
            return false;
        }
    }
}
