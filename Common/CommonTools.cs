using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FormsNet.Common
{
    public class CommonTools
    {
        public static int GetTimeStamp()
        {
            return GetTimeStamp(DateTime.UtcNow.ToLocalTime());
        }

        public static string GetFormatTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static int GetTimeStamp(DateTime value)
        {
            TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
            return ((int)span.TotalSeconds);
        }
        public static string UrlEncode(string value)
        {
            var tmp = value;
            return HttpUtility.UrlEncode(tmp, System.Text.Encoding.GetEncoding("utf-8"));
            //return WebUtility.UrlEncode(tmp);
        }

        public static string UrlDecode(string value)
        {
            var tmp = value;
            return HttpUtility.UrlDecode(tmp);
            //return WebUtility.UrlDecode(tmp);
        }

        public static string Base64Decode(string txt)
        {
            return System.Text.Encoding.GetEncoding("utf-8").GetString(Convert.FromBase64String(txt));
        }

        public static string Base64Encode(string txt)
        {
            byte[] encodedBytes = System.Text.Encoding.GetEncoding("utf-8").GetBytes(txt);
            return Convert.ToBase64String(encodedBytes);
        }

        //產生 HMACSHA256 雜湊
        public static string ComputeHMACSHA256(string data, string key)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            using (var hmacSHA = new HMACSHA256(keyBytes))
            {
                var dataBytes = Encoding.UTF8.GetBytes(data);
                var hash = hmacSHA.ComputeHash(dataBytes, 0, dataBytes.Length);
                return BitConverter.ToString(hash).Replace("-", "").ToUpper();
            }
        }

        //AES 加密
        public static string AESEncrypt(string data, string key, string iv)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            //var ivBytes = Encoding.UTF8.GetBytes(iv);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] ivBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(iv));
            var dataBytes = Encoding.UTF8.GetBytes(data);
            using (var aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = ivBytes;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                var encryptor = aes.CreateEncryptor();
                var encrypt = encryptor
                    .TransformFinalBlock(dataBytes, 0, dataBytes.Length);
                return Convert.ToBase64String(encrypt);
            }
        }

        //AES 解密
        public static string AESDecrypt(string data, string key, string iv,
            CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            var ivBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(iv));

            return AESDecrypt(data, keyBytes, ivBytes, CipherMode.CBC, PaddingMode.PKCS7);
        }

        public static string AESDecrypt(string data, byte[] key, byte[] iv,
            CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingMode = PaddingMode.PKCS7)
        {

            var dataBytes = Convert.FromBase64String(data);
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = cipherMode;
                aes.Padding = paddingMode;
                var decryptor = aes.CreateDecryptor();
                var decrypt = decryptor
                    .TransformFinalBlock(dataBytes, 0, dataBytes.Length);
                return Encoding.UTF8.GetString(decrypt);
            }
        }

        public static string MD5Hash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "");
            }
        }

        public static string Userip_Get(HttpContext context)
        {
            string IP = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(context.Request.Headers["X-Forwarded-For"]))
                {
                    IP = context.Request.Headers["X-Forwarded-For"];
                }
                else if (!string.IsNullOrEmpty(context.Request.Headers["MS_HttpContext"]))
                {
                    IP = context.Request.Headers["MS_HttpContext"];
                }
                else
                {
                    IP = context.Connection.RemoteIpAddress.ToString();

                    if (context.Connection.RemoteIpAddress.IsIPv4MappedToIPv6)
                    {
                        IP = context.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    }
                }

                if (!string.IsNullOrWhiteSpace(IP))
                {
                    string[] forwarded_ip_list = IP.ToString().Split(",");
                    IP = forwarded_ip_list.Length > 0 ? forwarded_ip_list[0] : "";
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //System.Net.IPHostEntry ips = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            //IP = ips.AddressList.ToList().Where(p => p.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).FirstOrDefault().ToString();
            return IP;
        }

        public static string GetParamValidErrorMessage(ModelStateDictionary modelState)
        {
            string error = "";

            IEnumerable<ModelError> allErrors = modelState.Values.SelectMany(v => v.Errors);
            foreach (var modelError in allErrors)
            {
                error += modelError.ErrorMessage + ",";
            }
            if (error.LastIndexOf(',') == error.Length - 1)
            {
                error = error.Remove(error.Length - 1);
            }

            return error;
        }

    }
}
