﻿using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace I520.Fremont.Services
{
    internal class ClientAssertionCertificate : IClientAssertionCertificate
    {
        private X509Certificate2 _certificate;

        public ClientAssertionCertificate(string clientId, X509Certificate2 certificate)
        {
            ClientId = clientId;
            _certificate = certificate;
        }

        public string ClientId { get; private set; }

        public string Thumbprint
        {
            get
            {
                return Base64UrlEncoder.Encode(_certificate.GetCertHash());
            }
        }

        public byte[] Sign(string message)
        {
            using (var key = _certificate.GetRSAPrivateKey())
            {
                return key.SignData(Encoding.UTF8.GetBytes(message), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }
    }
}