using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using CoreExtensions;

namespace DistSysACW.Models
{
    public static class ProtectedRepository
    {
        private static readonly RSACryptoServiceProvider RSA;

        public static String PublicKey => ProtectedRepository.RSA.ToXmlStringCore22();

        static ProtectedRepository()
        {
            CspParameters parameters = new CspParameters()
            {
                Flags = CspProviderFlags.UseMachineKeyStore
            };

            ProtectedRepository.RSA = new RSACryptoServiceProvider(parameters);
        }

        public static String ConvertByteArrayToString(byte[] arr, bool dashes = false)
        {
            StringBuilder sb = new StringBuilder(arr.Length * 2);

            foreach (byte b in arr)
            {
                sb.AppendFormat("{0:x2}", b);

                if (dashes)
                {
                    sb.Append("-");
                }
            }

            if (dashes)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            return sb.ToString();
        }

        public static byte[] ConvertStringToByteArray(String data)
        {
            data = data.Replace("-", "");
            byte[] dataBytes = new byte[data.Length / 2];

            for (int i = 0; i < dataBytes.Length; i ++)
            {
                dataBytes[i] = Convert.ToByte(data.Substring(i * 2, 2), 16);
            }

            return dataBytes;
        }

        public static byte[] DecryptRsa(byte[] data, bool fOAEP = true) => ProtectedRepository.RSA.Decrypt(data, fOAEP);

        public static byte[] EncryptAes(String data, AesCryptoServiceProvider aes)
        {
            ICryptoTransform encryptor = aes.CreateEncryptor();

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(data);
                    }

                    return ms.ToArray();
                }
            }
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