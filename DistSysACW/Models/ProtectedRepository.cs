using System;
using System.Security.Cryptography;

using CoreExtensions;

namespace DistSysACW.Models
{
    public static class ProtectedRepository
    {
        private static readonly RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

        public static String PublicKey => ProtectedRepository.RSA.ToXmlStringCore22();
    }
}