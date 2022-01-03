using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace LGUVirtualOffice.Log
{
    public class Loghttp
    {
       
        public static void Post(string url,string apikey,string json) 
        {

            try
            {
                Task task = Task.Run(() =>
                {
                    byte[] postBytes = Encoding.GetEncoding("utf-8").GetBytes(json);
                    HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.Headers.Add("x-api-key", apikey);
                    request.ContentLength = postBytes.Length;
                    LogUtil.LogInfo(request.Headers);
                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(postBytes, 0, postBytes.Length);
                    requestStream.Close();
                    WebResponse response = request.GetResponse();
                    StreamReader stream = new StreamReader(response.GetResponseStream());
                    LogUtil.LogDebug(stream.ReadToEnd());
                });
            }
            catch (System.Exception e)
            {
                LogUtil<SentryLog>.LogException(e);
            }
            
        }
    }
}