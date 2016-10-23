using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace E5186_NetworkSwitch
{
    class E5186APIHelper
    {      

        public static String checkData(String data)
        {

            //TODO: Better checkData with regex
            data = data.Replace("response", "request");

            if (!data.StartsWith("<request>"))
            {
                data = "<request>" + data;
            }
            if (!data.EndsWith("</request>"))
            {
                data = data + "</request>";
            }

            if (!data.StartsWith("<?xml version \"1.0\" encoding=\"UTF-8\"?>"))
            {
                data = "<?xml version \"1.0\" encoding=\"UTF-8\"?>" + data;
            }

            return data;
        }

        public static String checkUrl(String url, IPAddress ipAddress)
        {
            if (url.StartsWith("/"))
            {
                url = "http://" + ipAddress + url;
            }
            url = url.ToLower();
            if (!url.StartsWith("http://" + ipAddress + "/api") && !url.StartsWith("http://" + ipAddress + "/config") && !url.StartsWith("http://" + ipAddress + "/html"))
            {
                throw new Exception("No acceptable URL defined!");
            }

            return url;
        }

        public static String encryptSHA256Managed(String stringToHash)
        {
            HashAlgorithm arg_12_0 = new SHA256CryptoServiceProvider();
            byte[] array = Encoding.ASCII.GetBytes(stringToHash);
            array = arg_12_0.ComputeHash(array);
            string text = "";
            byte[] array2 = array;
            checked
            {
                for (int i = 0; i < array2.Length; i++)
                {
                    byte b = array2[i];
                    text += b.ToString("x2");
                }
                return text;
            }
        }




    }
}
