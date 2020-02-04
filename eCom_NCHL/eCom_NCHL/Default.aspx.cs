using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eCom_NCHL
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Compute();
        }

        protected void btnNCHL_Click(object sender, EventArgs e)
        {
            Compute();
        }

        private void Compute()
        {
            string message = "MERCHANTID={merchant_id},APPID={app_id},APPNAME={app_name},TXNID=34,TXNDATE=08-10-2017,TXNCRNCY=NPR,TXNAMT=1000,REFERENCEID=1.2.4,REMARKS=123455,PARTICULARS=12345,TOKEN=TOKEN";


            string certPath = "E://Development/eCommerce/eCom_NCHL/eCom_NCHL/BDISP.pfx";
            string certPass = "";

            string token = Sign(message, certPath, certPass);
            


            Response.Redirect("/NCHL.aspx?token=" + token, false);
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


        }

        private string ConvertString(byte[] data)
        {
            char[] characters = data.Select(b => (char)b).ToArray();
            return new string(characters);
        }
    }
}