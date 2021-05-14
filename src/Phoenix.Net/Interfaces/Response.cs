using System;
using System.Net;

namespace Phoenix.Net.Interfaces
{
    public class Response : IDisposable
    {
        public HttpWebResponse WebResponse { get; set; }
        public TimeSpan RequestLatency { get; set; }
        public Action<Response> PostRequestAction { get; set; }

        public void Dispose()
        {
            if (PostRequestAction != null)
            {
                PostRequestAction(this);
            }
            if (WebResponse != null)
            {
                WebResponse.Dispose();
            }
        }
    }
}
