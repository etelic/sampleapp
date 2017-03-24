using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
namespace GeneratorBase.MVC.Models
{
    public class EncryptDecrypt
    {
        public string EncryptString(string Message)
        {	
            if (string.IsNullOrEmpty(Message))
                return null;
            string Passphrase = "Turanto";
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
            // Step 1. We hash the passphrase using MD5 
            // We use the MD5 hash generator as the result is a 128 bit byte array 
            // which is a valid length for the TripleDES encoder we use below
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));
            // Step 2. Create a new TripleDESCryptoServiceProvider object 
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
            // Step 3. Setup the encoder 
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;
            // Step 4. Convert the input string to a byte[] 
            byte[] DataToEncrypt = UTF8.GetBytes(Message);
            // Step 5. Attempt to encrypt the string 
            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information 
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }
            // Step 6. Return the encrypted string as a base64 encoded string 
            return Convert.ToBase64String(Results);
        }
        public string DecryptString(string Message)
        {   
            if (string.IsNullOrEmpty(Message))
                return null;
            string Passphrase = "Turanto";
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
            // Step 1. We hash the passphrase using MD5 
            // We use the MD5 hash generator as the result is a 128 bit byte array 
            // which is a valid length for the TripleDES encoder we use below
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));
            // Step 2. Create a new TripleDESCryptoServiceProvider object 
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
            // Step 3. Setup the decoder 
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;
            // Step 4. Convert the input string to a byte[] 
            byte[] DataToDecrypt = Convert.FromBase64String(Message);
            // Step 5. Attempt to decrypt the string 
            try
            {
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information 
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }
            // Step 6. Return the decrypted string in UTF8 format 
            return UTF8.GetString(Results);
        }
    }
    public class SendEmail
    {
        public void Notify(String ToID, String Body, String Subject)
        {
            DataSet ds = new DataSet();
           // ds.ReadXml(HttpContext.Current.Server.MapPath("~/App_Data/CompanyProfile.xml"));
            ds.ReadXml(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/CompanyProfile.xml"));
            
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                NetworkCredential cred = new NetworkCredential(dr["Email"].ToString(), Decryptdata(dr["SMTPPassword"].ToString()));
                mail.To.Add(ToID.TrimEnd(",".ToCharArray()).TrimEnd(";".ToCharArray()));
                mail.Subject = Subject;
                mail.From = new MailAddress(dr["Email"].ToString());
                mail.Body = Body;
                mail.IsBodyHtml = true;
                string client = dr["SMTPServer"].ToString();
                SmtpClient smtp = new SmtpClient(client);
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = Convert.ToBoolean(dr["SSL"]);
                smtp.Credentials = cred;
                try
                {
                    smtp.Send(mail);
                }
                catch (Exception ex) { }
            }
        }
        public string Decryptdata(string password)
        {
            string decryptpwd = string.Empty;
            UTF8Encoding encodepwd = new UTF8Encoding();
            Decoder Decode = encodepwd.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(password);
            int charCount = Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            decryptpwd = new String(decoded_char);
            return decryptpwd;
        }
    }
}