using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Net;
using System.Net.Http;
using CloudFlareUtilities;
using System.Threading.Tasks;
using BtrexDataCapture.Data;
using BtrexDataCapture.Interface.WebSocketSharpTransport;


namespace BtrexDataCapture.Interface
{
    public class BtrexWebSocket
    {
        public static BtrexWSwithCFUtil WSSharpTransport { get; set; }

        //public readonly static HubConnection hubConnection = new HubConnection("https://socket.bittrex.com/");
        //public static IHubProxy btrexHubProxy;

        public async Task SubscribeMarket(string delta)
        {
            await WSSharpTransport.HubProxy.Invoke("SubscribeToExchangeDeltas", delta);
            MarketQueryResponse marketQuery = WSSharpTransport.HubProxy.Invoke<MarketQueryResponse>("QueryExchangeState", delta).Result;

            marketQuery.MarketName = delta;
            await BtrexData.OpenMarket(marketQuery);
        }

        public async Task<List<MarketQueryResponse>> SubscribeMarketsList(List<string> deltas)
        {
            List<MarketQueryResponse> failedList = new List<MarketQueryResponse>();
            var subList = deltas.Select(d => WSSharpTransport.HubProxy.Invoke("SubscribeToExchangeDeltas", d)).ToArray();
            await Task.WhenAll(subList);

            foreach (string d in deltas)
            {                
                var delta = "BTC-" + d;

                Trace.Write("\rSUBBING: " + delta + "                       \r");
                await WSSharpTransport.HubProxy.Invoke("SubscribeToExchangeDeltas", delta);
                                
                //MarketQueryResponse marketQuery = await WSSharpTransport.HubProxy.Invoke<MarketQueryResponse>("QueryExchangeState", delta);
                //marketQuery.MarketName = delta;
                //bool opened = await BtrexData.TryOpenMarket(marketQuery);

                //if (!opened)
                //    failedList.Add(marketQuery);

            }

            return failedList;
        }



        public async Task Connect()
        {
            //Console.Write("Connecting Websocket...");
            const string userAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36 OPR/48.0.2685.52";

            var bittrexUri = new Uri("https://bittrex.com");
            var bittrexFeedUri = new Uri("https://socket.bittrex.com");

            //

            var feedHeaders = new Dictionary<string, string>();
            var cookieContainer = new CookieContainer();
            var httpClientHandler = new HttpClientHandler()
            {
                UseCookies = true,
                CookieContainer = cookieContainer
            };

            var clearanceHandler = new ClearanceHandler(httpClientHandler);
            var httpClient = new HttpClient(clearanceHandler);

            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);

            //

            var connConfig = new ConnectionConfiguration()
            {
                CookieContainer = cookieContainer,
                Headers = feedHeaders
            };

            feedHeaders.Add("User-Agent", userAgent);

            var config = new BittrexFeedConnectionConfiguration()
            {
                // NOTE: Not applicable: AccessToken = "",
                Connection = connConfig
            };

            WSSharpTransport = new BtrexWSwithCFUtil(bittrexFeedUri);

            //

            var request = new HttpRequestMessage(HttpMethod.Get, bittrexUri);
            var content = httpClient.SendAsync(request, CancellationToken.None).Result;

            //

            WSSharpTransport.Connection.CookieContainer = cookieContainer;

            await WSSharpTransport.Connect(config);
            //Console.WriteLine("\rWebsocket Connected.      ");
            
        }
    }



    public static class BtrexWS
    {
        public static BtrexWSwithCFUtil WSSharpTransport { get; set; }

        //public readonly static HubConnection hubConnection = new HubConnection("https://socket.bittrex.com/");
        //public static IHubProxy btrexHubProxy;

        public static async Task SubscribeMarket(string delta)
        {
            await WSSharpTransport.HubProxy.Invoke("SubscribeToExchangeDeltas", delta);
            MarketQueryResponse marketQuery = WSSharpTransport.HubProxy.Invoke<MarketQueryResponse>("QueryExchangeState", delta).Result;

            marketQuery.MarketName = delta;
            await BtrexData.OpenMarket(marketQuery);
        }

        public static async Task<List<MarketQueryResponse>> SubscribeMarketsList(List<string> deltas)
        {
            List<MarketQueryResponse> failedList = new List<MarketQueryResponse>();
            var subList = deltas.Select(d => WSSharpTransport.HubProxy.Invoke("SubscribeToExchangeDeltas", d)).ToArray();
            await Task.WhenAll(subList);

            foreach (string d in deltas)
            {
                var delta = "BTC-" + d;

                Trace.Write("\rSUBBING: " + delta + "                       \r");
                await WSSharpTransport.HubProxy.Invoke("SubscribeToExchangeDeltas", delta);

                MarketQueryResponse marketQuery = await WSSharpTransport.HubProxy.Invoke<MarketQueryResponse>("QueryExchangeState", delta);
                marketQuery.MarketName = delta;
                bool opened = await BtrexData.TryOpenMarket(marketQuery);

                if (!opened)
                    failedList.Add(marketQuery);

            }

            return failedList;
        }



        public static async Task Connect()
        {
            Console.Write("Connecting Websocket2...");
            const string userAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36 OPR/48.0.2685.52";

            var bittrexUri = new Uri("https://bittrex.com");
            var bittrexFeedUri = new Uri("https://socket.bittrex.com");

            //

            var feedHeaders = new Dictionary<string, string>();
            var cookieContainer = new CookieContainer();
            var httpClientHandler = new HttpClientHandler()
            {
                UseCookies = true,
                CookieContainer = cookieContainer
            };

            var clearanceHandler = new ClearanceHandler(httpClientHandler);
            var httpClient = new HttpClient(clearanceHandler);

            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);

            //

            var connConfig = new ConnectionConfiguration()
            {
                CookieContainer = cookieContainer,
                Headers = feedHeaders
            };

            feedHeaders.Add("User-Agent", userAgent);

            var config = new BittrexFeedConnectionConfiguration()
            {
                // NOTE: Not applicable: AccessToken = "",
                Connection = connConfig
            };

            WSSharpTransport = new BtrexWSwithCFUtil(bittrexFeedUri);

            //

            var request = new HttpRequestMessage(HttpMethod.Get, bittrexUri);
            var content = httpClient.SendAsync(request, CancellationToken.None).Result;

            //

            WSSharpTransport.Connection.CookieContainer = cookieContainer;

            await WSSharpTransport.Connect(config);
            Console.WriteLine("\rWebsocket2 Connected.      ");
            
        }
    }
        

}
