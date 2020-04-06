using System;
using System.Security.Cryptography;
using System.Text;

using CoreExtensions;

namespace DistSysACW.Models
{
    public static class ProtectedRepository
    {
        private static readonly RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

        public static String PublicKey => ProtectedRepository.RSA.ToXmlStringCore22();

        public static String SignMessage(String message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof (message));
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