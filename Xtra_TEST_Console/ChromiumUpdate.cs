using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace AAA_TEST_Console
{
    public static class ChromiumUpdate
    {
        const string postData = "app=MTkxMDA5";
        const string nameOfStable = "stable-sync-hibbiki";
        const string nameOfUngoogled = "stable-ungoogled-marmaduke";

        public static void GetApiData()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://chromium.woolyss.com/api/v4/");
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; rv:78.0) Gecko/20100101 Firefox/78.0";

                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(response.CharacterSet));

                string json = sr.ReadToEnd();
                sr.Close();

                var result = JsonConvert.DeserializeObject<Rootobject>(json);

                var jsonSmall = JsonConvert.SerializeObject(result);

                var version = result.win64.FirstOrDefault(w => w.tag == nameOfStable).version; // 138.0.7204.158

                var link = result.win64.FirstOrDefault(w => w.tag == nameOfStable).links.FirstOrDefault(l => l.label == "Archive").url;

                var TEST = true;

            }
            catch (Exception)
            {
                //  "No response"; 
                //   No internet connection or no API response
            }
        }
    }

    public class Rootobject
    {
        public win64[] win64 { get; set; }
    }

    public class win64
    {
        public string tag { get; set; }
        public string editor { get; set; }
        public string channel { get; set; }
        public string version { get; set; }
        public string revision { get; set; }
        public string timestamp { get; set; }
        public links[] links { get; set; }
    }

    public class links
    {
        public string label { get; set; }
        public string url { get; set; }
        public string repository { get; set; }
    }
}
