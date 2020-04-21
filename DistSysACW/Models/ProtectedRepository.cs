using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using CoreExtensions;

namespace DistSysACW.Models
{
    public static class ProtectedRepository
    {
        private static readonly RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

        public static String PublicKey => ProtectedRepository.RSA.ToXmlStringCore22();

        public static String ConvertByteArrayToString(byte[] arr)
        {
            StringBuilder sb = new StringBuilder(arr.Length * 2);

            foreach (byte b in arr)
            {
                sb.AppendFormat("{0:x2}", b);
            }

            return sb.ToString();
        }

        public static String Decrypt(String encryptedString)
        {
            byte[] encryptedBytes = Encoding.ASCII.GetBytes(encryptedString.Replace("-", ""));
            byte[] decryptedBytes = ProtectedRepository.RSA.Decrypt(encryptedBytes, false);
            return Encoding.ASCII.GetString(decryptedBytes);
        }

        public static String Encrypt(String decryptedString)
        {
            byte[] decryptedBytes = Encoding.ASCII.GetBytes(decryptedString);
            byte[] encryptedBytes = ProtectedRepository.RSA.Encrypt(decryptedBytes, false);
            return ProtectedRepository.ConvertByteArrayToString(encryptedBytes);
        }

        public static async Task<String> SignMessageAsync(UserContext context, String apiKey, String message, String name)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof (context));
            }

            if (message is null)
            {
                throw new ArgumentNullException(nameof (message));
            }

            if (!String.IsNullOrWhiteSpace(apiKey))
            {
                User user = UserDatabaseAccess.Get(context, apiKey);

                if (!(user is null))
                {
                    Log log = new Log(name);
                    user.Logs.Add(log);
                    context.Logs.Add(log);
                    await context.SaveChangesAsync();
                }
            }

            byte[] asciiMessage = Encoding.ASCII.GetBytes(message);
            byte[] signedMessage = ProtectedRepository.RSA.SignData(asciiMessage, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
            String[] signedHexMessage = new String[signedMessage.Length];
            
            for (int i = 0; i < signedMessage.Length; i ++)
            {
                signedHexMessage[i] = signedMessage[i].ToString("x2");
            }

            return String.Join('-', signedHexMessage);
        }
    }
}