using Java.Lang;
using Xamarin.Forms;
using System.Globalization;
using System.Net;
using DineDash.Droid.Renderer;
using DineDash.Interface;

[assembly: Dependency(typeof(IsNetworkAvailableImplement))]
namespace DineDash.Droid.Renderer
{
    public class IsNetworkAvailableImplement : INetworkAvailable
    {
        public IsNetworkAvailableImplement()
        {
        }

        public bool IsNetworkAvailable()
        {
            try
            {
                string url = "https://www.google.com";

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.Timeout = 10000;
                using (var response = (HttpWebResponse)request.GetResponse())
                    return true;
            }
            catch
            {
                return false;
            }
        }
    }
}