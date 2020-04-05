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
            SHA1 sha1Provider = new SHA1CryptoServiceProvider();
            byte[] signedMessage = ProtectedRepository.RSA.SignData(asciiMessage, sha1Provider);
            String[] signedHexMessage = new String[signedMessage.Length];
            
            for (int i = 0; i < signedMessage.Length; i ++)
            {
                signedHexMessage[i] = signedMessage[i].ToString("x2");
            }

            return String.Join('-', signedHexMessage);
        }
    }
}