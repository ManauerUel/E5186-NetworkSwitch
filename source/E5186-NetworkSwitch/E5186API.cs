using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace E5186_NetworkSwitch
{
    class E5186API
    {

        private CookieContainer cookieJar;
        private String token = null;
        private Thread connectionThread;

        private IPAddress ipaddress;
        public IPAddress IpAddress
        {
            get { return this.ipaddress; }
            set { this.ipaddress = value;  }
        }

        private String password;
        public String Password
        {
            get { return this.password; }
            set { this.password = value; }
        }

        private ConnectionType connected;
        public ConnectionType Connected
        {
            get { return this.connected; }
        }

        public MessageHandler messageHandler;
        public ConnectionHandler connectionHandler;
        


        public enum NET_MODE
        {
            AUTO = 0,
            EDGE = 1,
            HSDPA = 2,
            HSDPA_PREFFERED = 201,
            LTE = 3,
            LTE_PREFFERED = 302
        }


        public E5186API(IPAddress ipaddress, String password)
        {
            this.ipaddress = ipaddress;
            this.password = password;

            connectionHandler += delegate (ConnectionType type)
            {
                connected = type;
            };

        }

        ~E5186API()
        {
            disconnect();
        }


        // =================== PUBLIC STUFF =======================================================================>
        public NET_MODE? getNetMode()
        {
            try { 
                return (NET_MODE)Convert.ToInt32(getValue("/api/net/net-mode", "NetworkMode"));
            }
            catch (Exception ex)
            {
                messageHandler.Invoke(ex.Message, MessageType.ERROR);
                return null;
            }
        }

        public void setNetMode(NET_MODE mode)
        {
            try {
                setValue("/api/net/net-mode", "NetworkMode", "0" + Convert.ToString((int)mode));
            }
            catch (Exception ex)
            {
                messageHandler.Invoke(ex.Message, MessageType.ERROR);
            }
}

        public String getValue(String apiUrl, String fieldName)
        {
            try {
                String xmlResponse = doGET(apiUrl);
                XDocument xdoc = XDocument.Parse(xmlResponse);
                return xdoc.Descendants(fieldName).Single().Value;
            }
            catch (Exception ex)
            {
                messageHandler.Invoke(ex.Message, MessageType.ERROR);
                return null;
            }
        }

        public void setValue(String apiUrl, String fieldName, String newValue)
        {
            try {
                String xmlResponse = doGET(apiUrl);
                XDocument xdoc = XDocument.Parse(xmlResponse);
                xdoc.Descendants(fieldName).Single().Value = newValue;
                doPOST(apiUrl, xdoc.ToString());
            }
            catch (Exception ex)
            {
                messageHandler.Invoke(ex.Message, MessageType.ERROR);
            }
        }

        public void connectThreaded()
        {
            connectionThread = new Thread(connect);
            connectionThread.Start();
        }

        public void connect()
        {
            if (Connected == ConnectionType.CONNECTED) disconnect();

            try
            {
                messageHandler("Verbindung wird hergestellt ...", MessageType.MESSAGE);
                cookieJar = new CookieContainer();
                setInitialToken();
                authorizeUser(password);
                connectionHandler.Invoke(ConnectionType.CONNECTED);
                messageHandler("Mit Gerät Verbunden.", MessageType.MESSAGE);
            }
            catch (Exception ex)
            {
                messageHandler.Invoke(ex.Message, MessageType.ERROR);
            }
        }

        public void disconnect()
        {
            if (Connected == ConnectionType.DISCONNECTED) return;

            try { 
                deAuthorizeUser();
            }
            catch (Exception ex)
            {
                // well yes ...
            }

            token = null;
            connectionHandler.Invoke(ConnectionType.DISCONNECTED);
        }

        // =================== PRIVATE STUFF ==============================================================>
        private void setInitialToken()
        {
            //read index html
            String indexHtml;
            indexHtml = doGET("/html/home.htm");
            if (indexHtml == null) return;

            // get token via regex
            String token = null;
            foreach (Match match in Regex.Matches(indexHtml, "<meta name=\"csrf_token\" content=\"(.*?)\\\"/>"))
            {
                token = match.Groups[1].ToString();
            }

            if (token != null && token != "")
            {
                this.token = token;
            }
            else
            {
                token = null;
                throw new Exception("Token konnte nicht extrahiert werden!");
            }
        }

        private void deAuthorizeUser()
        {
            doPOST("/api/user/logout", "<Logout>1</Logout>", 1000);
        }

        private void authorizeUser(String password)
        {
            // create password
            String passSHA256 = E5186APIHelper.encryptSHA256Managed(password);
            byte[] passSHA256bytes = Encoding.UTF8.GetBytes(passSHA256);
            String userPlusPass = E5186APIHelper.encryptSHA256Managed("admin" + Convert.ToBase64String(passSHA256bytes) + this.token);
            byte[] userPlussPassBytes = Encoding.UTF8.GetBytes(userPlusPass);
            String endPassword = Convert.ToBase64String(userPlussPassBytes);

            // do login request
            String response = doPOST("/api/user/login", "<Username>admin</Username><Password>" + endPassword + "</Password><password_type>4</password_type>");
            if (response == null || !response.Contains("<response>OK</response>"))
            {
                throw new Exception("Benutzer-Authentifizierung fehlgeschlagen!");
            }

        }

        private String doGET(String apiUrl)
        {
            return doGET(apiUrl, 2000); //std 2sec
        }

        private String doGET(String apiUrl, int timeout)
        {
            // prepare and check url
            apiUrl = E5186APIHelper.checkUrl(apiUrl, this.ipaddress);

            // prepare and do request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
            request.Method = "GET";
            request.CookieContainer = cookieJar;
            request.Timeout = timeout; //ms

            try {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader responseReader = new StreamReader(responseStream))
                {
                    // is response from device ok?
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return responseReader.ReadToEnd();
                    }
                    else
                    {
                        throw new Exception("Das Gerät lieferte keien gültige Antwort!");
                    }
                }
            }
            catch (WebException ex)
            {
                connectionHandler.Invoke(ConnectionType.DISCONNECTED);
                throw new Exception("Keine Verbindung mit dem Gerät möglich!");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private String doPOST(String apiUrl, String data)
        {
            return doPOST(apiUrl, data, 10000); //std 10sec
        }

        private String doPOST(String apiUrl, String data, int timeout)
        {
            // prepare and check url/data
            apiUrl = E5186APIHelper.checkUrl(apiUrl, this.ipaddress);
            data = E5186APIHelper.checkData(data);

            // prepare and do request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
            request.Method = "POST";
            request.CookieContainer = this.cookieJar;
            request.Headers.Add("__RequestVerificationToken", token);
            request.Timeout = timeout; //ms

            try {
                using (Stream requestStream = request.GetRequestStream())
                using (StreamWriter requestWriter = new StreamWriter(requestStream, Encoding.UTF8))
                {
                    requestWriter.Write(data);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader responseReader = new StreamReader(responseStream))
                {

                    // is response from device ok?
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        this.token = response.GetResponseHeader("__RequestVerificationToken"); // save new Token
                        return responseReader.ReadToEnd();
                    }
                    else
                    {
                        throw new Exception("Das Gerät lieferte keien gültige Antwort!");
                    }
                }
            }
            catch (WebException ex)
            {
                connectionHandler.Invoke(ConnectionType.DISCONNECTED);
                throw new Exception("Keine Verbindung mit dem Gerät möglich!");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

}
