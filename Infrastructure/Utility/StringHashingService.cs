using Application.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Utility
{
    public class StringHashingService : IStringHashingService
    {
        public string CreateDESStringHash(string input)
        {
            var hashmd5 = new MD5CryptoServiceProvider();
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(input);

            byte[] keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(input));
            hashmd5.Clear();
            TripleDESCryptoServiceProvider TripleDesProvider = new TripleDESCryptoServiceProvider();
            TripleDesProvider.Key = keyArray;
            TripleDesProvider.Mode = CipherMode.ECB;
            TripleDesProvider.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = TripleDesProvider.CreateEncryptor();

            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        public string DecodeDESStringHash(string input)
        {
            try
            {
                TripleDESCryptoServiceProvider TripleDesProvider = new TripleDESCryptoServiceProvider();
                var hashmd5 = new MD5CryptoServiceProvider();
                byte[] toEncryptArray = Convert.FromBase64String(input);

                byte[] keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(input));

                hashmd5.Clear();

                TripleDesProvider.Key = keyArray;
                TripleDesProvider.Mode = CipherMode.ECB;
                TripleDesProvider.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = TripleDesProvider.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                TripleDesProvider.Clear();

                return Encoding.UTF8.GetString(resultArray);
            }
            catch
            {
                return string.Empty;
            }
        }
    }

}
