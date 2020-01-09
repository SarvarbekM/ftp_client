using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace Client
{
    class MyGlobalClass
    {
        private static string host;
        public static string Host
        {
            get { return host; }
            set
            {
                host = value;
                FullHost = "ftp://" + host + "/";
            }
        }
        public static string FullHost { get; private set; }
        public static string User { get; set; }
        public static string Password { get; set; }

        public static bool CheckInternet()
        {
            try
            {
                Ping myPing = new Ping();
                String host = "google.com";                
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return (reply.Status == IPStatus.Success);
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
