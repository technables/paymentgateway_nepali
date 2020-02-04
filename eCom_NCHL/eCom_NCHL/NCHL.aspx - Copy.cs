using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;


namespace eCom_NCHL
{
    public partial class NCHL : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Compute();
        }

        private void Compute()
        {
            string message = "MERCHANTID=22,APPID=MER-22-APP-1,APPNAME=Brain Digit Pvt Ltd,TXNID=159690820507240602,TXNDATE=24-07-2018,TXNCRNCY=NPR,TXNAMT=10000,REFERENCEID=159690,REMARKS= Topup-test,PARTICULARS=Topup-Test,TOKEN=TOKEN";
            // string message = "MERCHANTID=22,APPID=MER-22-APP-1,APPNAME=Brain Digit Pvt Ltd,TXNID=802745,TXNDATE=08-10-2017,TXNCRNCY=NPR,TXNAMT=1000,REFERENCEID=1.2.4,REMARKS=123455,PARTICULARS=12345,TOKEN=TOKEN";
            string certPath = "E://Development/eCommerce/eCom_NCHL/eCom_NCHL/BDISP.pfx";
            string certPass = "123";

            //string token = getSignature(message, certPath, certPass);
            string token = getSignature(message, certPath, certPass);
            TOKEN.Value = token;

        }


        /// <summary>
        /// generate hash using certificate
        /// </summary>
        /// <param name="text">text to hash</param>
        /// <param name="certSubject">name of certificate</param>
        /// <returns>hashed string</returns>
        string Sign(string stringToHash, string pfxCertificate, string pfxPassword)
        {

            var crypt = new SHA256Managed();
            string digest = String.Empty;
            byte[] data = Encoding.UTF8.GetBytes(stringToHash);
            byte[] crypto = crypt.ComputeHash(data);

            foreach (byte theByte in crypto)
            {
                digest += theByte.ToString("x2");
            }

            X509Certificate2 cert = new X509Certificate2(pfxCertificate, pfxPassword, X509KeyStorageFlags.Exportable);
            RSACryptoServiceProvider csp = null;
            if (cert != null)
            {
                csp = cert.PrivateKey as RSACryptoServiceProvider;
            }

            if (csp == null)
            {
                throw new Exception("No valid cert was found");
            }

            csp.ImportParameters(csp.ExportParameters(true));
            byte[] signatureByte = csp.SignData(data, "SHA256");
            string signature = Convert.ToBase64String(signatureByte);

            return signature;

            //OVZwF7CJK0dv3RUs5QEFnuj/8CDk88jkskVFHI/vG3T5ihIv9IUnqxactRElvUyUQ0zrejFrMlvKkQY0olC1x74Oep5P6TmFfVIYwDE8KGjoxnlxcIrtl3P6DYX9xH1InyCKSsKcj8tZthFarQEeWAWe8bkHxb0H9MzZoEGjxbkdbj4o4dzaIF3X5V+q7jHNwKKlRkNOprgjRU3V86sWpTqqNqKxVWXR6wstyIfqaA6BduWv/JnqfhJDC/scyN7HMiEppMt9ttFWfW5WbLNN0f1RG7GbWzH6mZjwOOTf38IbaIsAUY1/9A1ta8LtTkfflYo3V+S4LAUD6gnYQol5tQ==
            //


        }


        private static string getSignature(string message, string publicKeyLocation, string pfxPassword)
        {
            string concateStr = message;// "MERCHANTID=10,APPID=MER-10-APP-12,APPNAME=Test,TXNID=159690820507240602,TXNDATE=24-07-2018,TXNCRNCY=NPR,TXNAMT=10000,REFERENCEID=159690,REMARKS= Topup-test,PARTICULARS=Topup-Test,TOKEN=TOKEN"
            byte[] buffer = Encoding.UTF8.GetBytes(concateStr);
            HashAlgorithm hash = SHA256.Create();
            byte[] hashValue = hash.ComputeHash(buffer);

            string digest = Convert.ToBase64String(hashValue);

            //Console.WriteLine("SHA256 hash computed::" + Convert.ToBase64String(hashValue));

            string signature = Convert.ToBase64String(signContent(hashValue, publicKeyLocation, pfxPassword));

            return signature;

        }

        private static byte[] signContent(byte[] hashValue, string publicKeyLocation, string pfxPassword)
        {
            X509Certificate2 publicCert = new X509Certificate2(publicKeyLocation, pfxPassword, X509KeyStorageFlags.Exportable);
            X509Certificate2 privateCert = null;
            X509Store store = new X509Store(StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            foreach (X509Certificate2 cert in store.Certificates)
            {
                if (cert.GetCertHashString() == publicCert.GetCertHashString())
                    privateCert = cert;
            }
            RSACryptoServiceProvider key = new RSACryptoServiceProvider();
            key.FromXmlString(privateCert.PrivateKey.ToXmlString(true));
            byte[] signature = key.SignHash(hashValue, CryptoConfig.MapNameToOID("SHA256"));
            key = (RSACryptoServiceProvider)publicCert.PublicKey.Key;
            if (!key.VerifyHash(hashValue, CryptoConfig.MapNameToOID("SHA256"), signature))
                throw new CryptographicException();
            string signatureString = ("Digital Signature Computed::" + Convert.ToBase64String(signature));
            return signature;
        }
    }
}