using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;

namespace BrainBowTestWebAPI.Models
{
    public class RoviApi
    {
        private const string RoviAppId = "7fk63kqh6es89qh75gvn6b9a";
        private const string RoviAppSecretKey = "tPbdSg4AWG";

        public JObject GetMovieByKeyword(string keyword)
        {
            var oAuthSessionToken = GetAccessTokenFromCode( RoviAppId, RoviAppSecretKey );
            var offset = RandNumber(1, 100).ToString(CultureInfo.InvariantCulture);

            var roviSearchMovieByKeywordUrl =
                string.Format(
                    "http://api.rovicorp.com/search/v2.1/amgvideo/search?entitytype=movie&query=%2A&rep=1&filter=genreid%3A{0}&include=cast&size=20&offset={3}&language=en&country=US&format=json&apikey={1}&sig={2}",
                    keyword, RoviAppId, oAuthSessionToken, offset);

            var jo = JObject.Parse(GetHttpResponse(roviSearchMovieByKeywordUrl));
            return jo;
        }
        

        public JObject GetCelebByName(string keyword)
        {
            var oAuthSessionToken = GetAccessTokenFromCode(RoviAppId, RoviAppSecretKey);
            var offset = RandNumber(1, 100).ToString(CultureInfo.InvariantCulture);

            var roviSearchCelebByNameUrl =
                string.Format(
                    "http://api.rovicorp.com/search/v2.1/video/search?entitytype=credit&query={0}&rep=1&include=filmography&size=5&offset=0&language=en&country=US&format=json&apikey={1}&sig={2}",
                    keyword, RoviAppId, oAuthSessionToken, offset);

            var jo = JObject.Parse(GetHttpResponse(roviSearchCelebByNameUrl));
            return jo;
        }

        public JObject GetFilmographyByCelebId(string keyword)
        {
            var oAuthSessionToken = GetAccessTokenFromCode(RoviAppId, RoviAppSecretKey);
            var offset = RandNumber(1, 100).ToString(CultureInfo.InvariantCulture);

            var roviSearchFilmographyByCelebNameUrl =
                string.Format(
                    "http://api.rovicorp.com/data/v1.1/name/filmography?cosmoid={0}&count=0&offset=0&country=US&language=en&format=json&apikey={1}&sig={2}",
                    keyword, RoviAppId, oAuthSessionToken, offset);

            var jo = JObject.Parse(GetHttpResponse(roviSearchFilmographyByCelebNameUrl));
            return jo;
        }
        #region "Helper Methods"
        public static string GetHttpResponse(string searchUrl)
        {
            var results = string.Empty;
            try
            {
                var req = (HttpWebRequest)WebRequest.Create(searchUrl);
                var resp = (HttpWebResponse)req.GetResponse();

                var sr = new StreamReader(resp.GetResponseStream());
                results = sr.ReadToEnd();

                sr.Close();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("400"))
                {
                    results = string.Format("{{\"status\":\"ok\",\"code\":200,\"messages\":{0},\"build\":\"1.0\"}}", e.Message);
                }
            }
            return results;
        }
        public static int RandNumber(int low, int high)
        {
            var rndNum = new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), NumberStyles.HexNumber));

            var rnd = rndNum.Next(low, high);

            return rnd;
        }
        #endregion
        #region GetAccessToken
        public static string GetAccessTokenFromCode(string appId, string secretKey)
        {
            //get the timestamp value
            var timestamp = (DateTime.UtcNow - new DateTime( 1970, 1, 1, 0, 0, 0 )).TotalSeconds.ToString(CultureInfo.InvariantCulture);

            //grab just the integer portion
            timestamp = timestamp.Substring( 0, timestamp.IndexOf(".", StringComparison.Ordinal) );

            //set the API key (note that this is not a valid key!
            const string apikey = RoviAppId;

            //set the shared secret key
            const string secret = RoviAppSecretKey;

            //call the function to create the hash
            var sig = CreateMd5Hash( apikey + secret + timestamp );

            return sig;
        }

        //note, requires "using System.Security.Cryptography;"
        public static string CreateMd5Hash(string input)
        {
            // Use input string to calculate MD5 hash
            var md5 = MD5.Create( );
            var inputBytes = Encoding.ASCII.GetBytes( input );
            var hashBytes = md5.ComputeHash( inputBytes );

            // Convert the byte array to hexadecimal string
            var sb = new StringBuilder( );
            for (var i = 0; i < hashBytes.Length; i++)
            {
                sb.Append( hashBytes[i].ToString( "x2" ) );  //this will use lowercase letters, use "X2" instead of "x2" to get uppercase
            }
            return sb.ToString( );
        }
#endregion 

    }
}