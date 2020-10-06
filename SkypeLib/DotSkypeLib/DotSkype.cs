using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace SkypeLib
{
    public class DotSkype
    {

        const string LiveHost = "https://login.live.com/";
        const string SkypeHost = "https://edge.skype.com/";
        const string ApiUser = "https://api.skype.com/";
        const string ContactHost = "https://contacts.skype.com/";
        const string MsgHost = "https://client-s.gateway.messenger.live.com/v1/";

        private string _skypeToken = "";
        private string _endPoint = "";

        public string BuildSopa(string account, string password)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<Envelope xmlns='http://schemas.xmlsoap.org/soap/envelope/'");
            stringBuilder.AppendLine("xmlns:wsse='http://schemas.xmlsoap.org/ws/2003/06/secext'");
            stringBuilder.AppendLine("xmlns:wsp='http://schemas.xmlsoap.org/ws/2002/12/policy'");
            stringBuilder.AppendLine("xmlns:wsa='http://schemas.xmlsoap.org/ws/2004/03/addressing'");
            stringBuilder.AppendLine("xmlns:wst='http://schemas.xmlsoap.org/ws/2004/04/trust'");
            stringBuilder.AppendLine("xmlns:ps='http://schemas.microsoft.com/Passport/SoapServices/PPCRL'>");
            stringBuilder.AppendLine("<Header>");
            stringBuilder.AppendLine("<wsse:Security>");
            stringBuilder.AppendLine("<wsse:UsernameToken Id='user'>");
            stringBuilder.AppendLine("<wsse:Username>" + account + "</wsse:Username>");
            stringBuilder.AppendLine("<wsse:Password>" + password + "</wsse:Password>");
            stringBuilder.AppendLine("</wsse:UsernameToken>");
            stringBuilder.AppendLine("</wsse:Security>");
            stringBuilder.AppendLine("</Header>");
            stringBuilder.AppendLine("<Body>");
            stringBuilder.AppendLine("<ps:RequestMultipleSecurityTokens Id='RSTS'>");
            stringBuilder.AppendLine("<wst:RequestSecurityToken Id='RST0'>");
            stringBuilder.AppendLine("<wst:RequestType>http://schemas.xmlsoap.org/ws/2004/04/security/trust/Issue</wst:RequestType>");
            stringBuilder.AppendLine("<wsp:AppliesTo>");
            stringBuilder.AppendLine("<wsa:EndpointReference>");
            stringBuilder.AppendLine("<wsa:Address>wl.skype.com</wsa:Address>");
            stringBuilder.AppendLine("</wsa:EndpointReference>");
            stringBuilder.AppendLine("</wsp:AppliesTo>");
            stringBuilder.AppendLine("<wsse:PolicyReference URI='MBI_SSL'></wsse:PolicyReference>");
            stringBuilder.AppendLine("</wst:RequestSecurityToken>");
            stringBuilder.AppendLine("</ps:RequestMultipleSecurityTokens>");
            stringBuilder.AppendLine("</Body>");
            stringBuilder.AppendLine("</Envelope>");

            return stringBuilder.ToString();
        }

        public string SendSoapLogin(string account, string password)
        {
            var request = WebRequest.Create(LiveHost + "RST.srf") as HttpWebRequest;
            byte[] bytes;
            bytes = System.Text.Encoding.UTF8.GetBytes(BuildSopa(account, password));
            request.ContentType = "application/xml;";
            request.ContentLength = bytes.Length;
            request.Method = "POST";
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            HttpWebResponse response;
            response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = response.GetResponseStream();
                //new XmlReader(response)
                var xml = XDocument.Load(response.GetResponseStream());
                XNamespace aw = "http://schemas.xmlsoap.org/ws/2003/06/secext";
                var columns = xml.Descendants(aw + "BinarySecurityToken");

                var token = columns.FirstOrDefault()?.Value;
                string responseStr = new StreamReader(responseStream).ReadToEnd();
                //token = responseStr.Replace("<wsse:BinarySecurityToken Id=\"Compact0\">", "|").Replace("</wsse:BinarySecurityToken>", "|").Split('|')[1];

                return token;
            }
            return null;
        }
        public string ExchangeSkypeToken(string token)
        {
            var request = WebRequest.Create(SkypeHost + "rps/v1/rps/skypetoken") as HttpWebRequest;



            NameValueCollection postParams = System.Web.HttpUtility.ParseQueryString(string.Empty);
            postParams.Add("partner", "999");
            postParams.Add("access_token", token);
            postParams.Add("scopes", "client");


            byte[] bytes;
            bytes = Encoding.UTF8.GetBytes(postParams.ToString());
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytes.Length;
            request.Method = "POST";
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            HttpWebResponse response;
            response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = response.GetResponseStream();
                string responseStr = new StreamReader(responseStream).ReadToEnd();

                var Jdata = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(responseStr);
                _skypeToken = Jdata["skypetoken"].ToString();

                return _skypeToken;
            }
            return null;
        }

        public string GetRegisterToken()
        {
            var request = WebRequest.Create(MsgHost + "users/ME/endpoints") as HttpWebRequest;
            request.Method = "GET";
            request.Headers.Add("X-Skypetoken", _skypeToken);
            request.Headers.Add("Authentication", "skypetoken=" + _skypeToken);



            try
            {
                var response = request.GetResponse();

                if (!MsgHost.Contains(response.ResponseUri.Host))
                {
                    _endPoint = "https://" + response.ResponseUri.Host + "/v1/";
                }
                else
                {
                    _endPoint = MsgHost;
                }

                var registTokenSource = response.Headers["Set-RegistrationToken"].ToString();
                return registTokenSource;
            }
            catch (WebException webExcp)
            {
                return "";
            }
        }

        public string GetSkypeUserProfile()
        {
            HttpWebRequest httpWebRequest = WebRequest.Create(ApiUser + "users/self/profile") as HttpWebRequest;
            httpWebRequest.Method = "GET";
            httpWebRequest.Headers.Add("X-Skypetoken", _skypeToken);

            using (WebResponse response = httpWebRequest.GetResponse())
            {
                StreamReader streamReader = new StreamReader(response.GetResponseStream());
                string contentStringData = streamReader.ReadToEnd();

                var userProfile = JsonConvert.DeserializeObject<UserProfile>(contentStringData);
                streamReader.Close();


                if (userProfile.username.Contains("live:"))
                {
                    // userProfile.username = userProfile.username.Replace("live:","");
                }
                return userProfile.username;
            }
        }

        public ContactInfo GetSkypeUserContactInfoList(string skypeId)
        {
            string requestUriStringUL = ContactHost + "contacts/v2/users/" + skypeId + "/contacts";
            HttpWebRequest request = WebRequest.Create(requestUriStringUL) as HttpWebRequest;
            request.Method = "Get";
            request.Headers.Add("X-Skypetoken", _skypeToken);
            request.ContentType = "application/json; charset=UTF-8";


            //byte[] bytes;
            //bytes = Encoding.UTF8.GetBytes("28:"+skypeId);
            //request.ContentLength = bytes.Length;
            //Stream requestStream = request.GetRequestStream();
            //requestStream.Write(bytes, 0, bytes.Length);
            //requestStream.Close();

            HttpWebResponse response;
            response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                StreamReader streamReader = new StreamReader(response.GetResponseStream());
                string end = streamReader.ReadToEnd();
                streamReader.Close();
                return JsonConvert.DeserializeObject<ContactInfo>(end);
            }
            return null;


            //var friends = new ContactInfo();

            //using (WebResponse response = request.GetResponse())
            //{
            //    StreamReader streamReader = new StreamReader(response.GetResponseStream());
            //    string end = streamReader.ReadToEnd();
            //    streamReader.Close();
            //    return JsonConvert.DeserializeObject<ContactInfo>(end);
            //}

        }

        public ConversactionThread GetSkypeConversactionList(string backLink = "")
        {
            try
            {
                var url = "";

                if (string.IsNullOrEmpty(backLink))
                {
                    url = MsgHost + "users/ME/conversations" + "?view=msnp24Equivalent&startTime=0&targetType=Thread";
                }
                else
                {
                    url = backLink;
                }

                HttpWebRequest httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
                httpWebRequest.Method = "GET";
                httpWebRequest.Headers.Add("X-Skypetoken", _skypeToken);
                httpWebRequest.Headers.Add("Authentication", "skypetoken=" + _skypeToken);

                using (WebResponse response = httpWebRequest.GetResponse())
                {
                    StreamReader streamReader = new StreamReader(response.GetResponseStream());
                    string contentStringData = streamReader.ReadToEnd();

                    var conversactionThread = JsonConvert.DeserializeObject<ConversactionThread>(contentStringData);
                    streamReader.Close();
                    return conversactionThread;
                }
            }
            catch (Exception ex)
            {

                return null;


            }

        }

        public List<ConversactionItem> conversactionItems = new List<ConversactionItem>();

        public void QueryThread(string backLink)
        {
            DotSkype dotSkype = new DotSkype();
            var threadList = dotSkype.GetSkypeConversactionList(backLink);
            if (threadList == null)
            {
                Thread.Sleep(1 * 60 * 1000);
                QueryThread(backLink);
            }
            else
            {
                if (threadList._metadata.backwardLink != "")
                {
                    conversactionItems.AddRange(threadList.conversations);
                    QueryThread(threadList._metadata.backwardLink);
                }
                else
                {
                    conversactionItems.AddRange(threadList.conversations);
                }
            }
        }

        public void UploadFileToObject(string imageId, string imageLocalFilePath, string logPath, byte[] imageSrcByte)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("Host", "api.asm.skype.com");
                client.Headers.Add("Authorization", "skype_token " + _skypeToken);
                client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.108 Safari/537.36");
                client.Headers.Add("Content-Type", "application");
                var res = client.UploadData("https://api.asm.skype.com/v1/objects/" + imageId + "/content/imgpsh", "PUT", imageSrcByte);

            }
        }

        public void SendMultipleText(string registrationToken, string message, List<string> ids,int reTryCount =1)
        {
            int tryCount = 0;
            List<string> failIds = new List<string>();
            foreach (var item in ids)
            {
                try
                {
                    SendText(registrationToken, message, item);
                }
                catch (Exception)
                {
                    failIds.Add(item);
                }

            }

            if (failIds.Count > 0)
            {
                if(tryCount < reTryCount)
                {
                    SendMultipleText(registrationToken, message, failIds);
                }
                tryCount++;


            }

        }

        public void SendIImage(string registrationToken, string objectId, string userId, string imageName, string text)
        {

            var sourceTempalate = "<URIObject type=\"Picture.1\" uri=\"https://api.asm.skype.com/v1/objects/" + objectId + "\"" +
              " url_thumbnail=\"https://api.asm.skype.com/v1/objects/" + objectId + "/views/imgt1\"><Title /><Description />" +
              "<meta type=\"photo\" originalName=\"" + imageName + "\"/>" +
              "<OriginalName v=\"" + imageName + "\"/>" +
              "</URIObject>";


            //POST https://client-s.gateway.messenger.live.com/v1/users/ME/conversations/(string: id)/messages
            HttpWebRequest httpWebRequest = WebRequest.Create(MsgHost + "v1/users/ME/conversations/" + userId + "/messages") as HttpWebRequest;
            httpWebRequest.Method = "POST";
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:35.0) Gecko/20100101 Firefox/35.0";
            httpWebRequest.Accept = "application/json, text/javascript";
            httpWebRequest.Headers.Add("Accept-Language", "zh-tw,zh;q=0.8,en-us;q=0.5,en;q=0.3");
            httpWebRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
            httpWebRequest.Headers.Add("ClientInfo", "os=Windows; osVer=8.1; proc=Win32; lcid=en-us; deviceType=1; country=n/a; clientName=skype.com; clientVer=908/1.9.0.232//skype.com");
            httpWebRequest.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            httpWebRequest.Headers.Add("Pragma", "no-cache");
            httpWebRequest.Headers.Add("Expires", "0");
            httpWebRequest.Headers.Add("BehaviorOverride", "redirectAs404");
            httpWebRequest.Headers.Add("RegistrationToken", "registrationToken=" + registrationToken);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Referer = "https://web.skype.com/zh-Hant/";
            httpWebRequest.Headers.Add("Origin", "https://web.skype.com");
            httpWebRequest.KeepAlive = true;
            var str = "";
            if (string.IsNullOrEmpty(text))
            {
                str = sourceTempalate;
            }
            else
            {
                str = sourceTempalate + "\r\n" + text;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object)new Dictionary<string, object>()
            {
            {
                "content",
                (object)str
            },
            {
                "messagetype",
                (object) "RichText/UriObject"
            },
            {
                "contenttype",
                (object)str
            },
            {
                "clientmessageid",
                (object) DateTime.Now.Ticks
            }
            }));
            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            using (httpWebRequest.GetResponse())
            {

            }
        }

        public void SendText(string registrationToken, string message, string id, bool tryAgain = false)
        {
            HttpWebResponse response = null;
            try
            {

                //
                //POST https://client-s.gateway.messenger.live.com/v1/users/ME/conversations/(string: id)/messages

                HttpWebRequest httpWebRequest = WebRequest.Create(_endPoint + "users/ME/conversations/" + id + "/messages") as HttpWebRequest;
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("Expires", "0");
                httpWebRequest.Headers.Add("BehaviorOverride", "redirectAs404");
                httpWebRequest.Headers.Add("RegistrationToken", registrationToken);
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                httpWebRequest.KeepAlive = true;
                byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object)new Dictionary<string, object>()
            {
            {
                "content",
                (object)message
            },
            {
                "messagetype",
                (object) "Text"
            },
            {
                "contenttype",
                (object)"text"
            },
            {
                "clientmessageid",
                (object) DateTime.Now.Ticks
            }
            }));

                using (Stream requestStream = httpWebRequest.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }


                response = (HttpWebResponse)httpWebRequest.GetResponse();

                StreamReader streamReader = new StreamReader(response.GetResponseStream());
                string contentStringData = streamReader.ReadToEnd();


            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    response = (HttpWebResponse)e.Response;
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        if (!tryAgain)
                        {
                            GetRegisterToken();
                            SendText(registrationToken, message, id, true);
                        }
                    }
                }
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
        }


    }






}