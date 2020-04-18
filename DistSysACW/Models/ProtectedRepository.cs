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