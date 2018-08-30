using System;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SA_EF
{
    public class X509EncDec
    {
        private static X509Certificate2 certificate;

        public X509EncDec(string certificateName)
        {
            if (certificate == null) certificate = getCertificate(certificateName);
        }

        private X509Certificate2 getCertificate(string certificateName)
        {
            X509Store my = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            my.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection collection =
                my.Certificates.Find(X509FindType.FindBySubjectName, certificateName, false);
            if (collection.Count == 1)
            {
                return collection[0];
            }
            else if (collection.Count > 1)
            {
                throw new Exception(string.Format("More than one certificate with name '{0}' found in store LocalMachine/My.", certificateName));
            }
            else
            {
                throw new Exception(string.Format("Certificate '{0}' not found in store LocalMachine/My.", certificateName));
            }
        }

        public string EncryptRsa(string input)
        {
            string output = string.Empty;

            RSACryptoServiceProvider csp = (RSACryptoServiceProvider)certificate.PublicKey.Key;
            {
                byte[] bytesData = Encoding.UTF8.GetBytes(input);
                byte[] bytesEncrypted = csp.Encrypt(bytesData, false);
                output = Convert.ToBase64String(bytesEncrypted);
            }
            return output;
        }

        public string DecryptRsa(string encrypted)
        {
            string text = string.Empty;
            RSACryptoServiceProvider csp = (RSACryptoServiceProvider)certificate.PrivateKey;
            {
                byte[] bytesEncrypted = Convert.FromBase64String(encrypted);
                byte[] bytesDecrypted = csp.Decrypt(bytesEncrypted, false);
                text = Encoding.UTF8.GetString(bytesDecrypted);
            }
            return text;
        }
    }
}