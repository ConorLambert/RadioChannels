using RadioChannels.Interfaces.WebApi;
using RadioChannels.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Xml;

namespace RadioChannels.Implementations
{
    public class XmlRequest : IRequest
    {
        HttpClient httpClient;

        public XmlRequest()
        {
            httpClient = new HttpClient();
            System.Net.ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.UseNagleAlgorithm = false;
            WebRequest.DefaultWebProxy = null;
        }

        public List<Channel> ApiRequestChannels(string requestUrl)
        {
            XmlDocument response = MakeRequest(requestUrl);
            return ProcessResponse(response);
        }

        private static XmlDocument MakeRequest(string requestUrl)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                request.Method = "GET";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(response.GetResponseStream());
                response.Dispose();
                return (xmlDoc);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                Console.Read();
                return null;
            }
        }

        private static List<Channel> ProcessResponse(XmlDocument response)
        {
            //Create namespace manager
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(response.NameTable);
            nsmgr.AddNamespace("ch", "urn:channels-schema");

            // XPath query which returns all <station .../> entries
            XmlNodeList stationElements = response.SelectNodes("//station");
            List<Channel> channels = new List<Channel>();

            foreach (XmlNode station in stationElements)    // for each station
            {
                XmlAttributeCollection details = station.Attributes;

                // Get Playlist information
                Channel channel = new Channel();
                if (details["id"] != null)
                {
                    channel.Id = details["id"].InnerText;
                    var playlistRequest = WebRequest.Create(@"http://yp.shoutcast.com/sbin/tunein-station.m3u?id=" + channel.Id);
                    using (var playlistResponse = playlistRequest.GetResponse())
                    using (var content = playlistResponse.GetResponseStream())
                    using (var reader = new StreamReader(content))
                    {
                        channel.Playlist = reader.ReadToEnd();
                    }
                }

                if (details["name"] != null)
                    channel.Name = details["name"].InnerText;
                if (details["mt"] != null)
                    channel.MediaType = details["mt"].InnerText;
                if (details["br"] != null)
                    channel.BitRate = details["br"].InnerText;
                if (details["ct"] != null)
                    channel.CurrentTrack = details["ct"].InnerText;
                if (details["logo"] != null)
                    channel.Logo = details["logo"].InnerText;
                if (details["lc"] != null)
                    channel.LC = details["lc"].InnerText;
                if (details["genre"] != null)
                    channel.Genre = details["genre"].InnerText;

                channels.Add(channel);
            }

            return channels;
        }        
    }
}