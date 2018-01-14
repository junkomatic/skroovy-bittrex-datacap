﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Security.Cryptography;

namespace BtrexDataCapture.Interface
{
    public static class BtrexREST
    {
        
        private static HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("https://bittrex.com/api/v1.1/"),
            Timeout = TimeSpan.FromMinutes(3)
        };

        public static async Task<decimal> getUSD()
        {
            decimal rate = await GetCBsellPrice();
            return rate;
        }

        public static async Task<TickerResponse> GetTicker(string delta)
        {
            TickerResponse ticker = null;
            HttpResponseMessage response = await client.GetAsync("public/getticker?market=" + delta);
            if (response.IsSuccessStatusCode)
                ticker = await response.Content.ReadAsAsync<TickerResponse>();
            return ticker;
        }

        public static async Task<GetMarketsResponse> GetMarkets()
        {
            GetMarketsResponse marketsResponse = null;
            HttpResponseMessage response = await client.GetAsync("public/getmarkets");
            if (response.IsSuccessStatusCode)
                marketsResponse = await response.Content.ReadAsAsync<GetMarketsResponse>();
            return marketsResponse;
        }

        public static async Task<MarketSummary> GetMarketSummary(string delta = null)
        {
            string uri;
            if (delta == null)
                uri = "public/getmarketsummaries";
            else
                uri = "public/getmarketsummary?market=" + delta;

            MarketSummary summary = null;
            HttpResponseMessage response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                summary = await response.Content.ReadAsAsync<MarketSummary>();
            }
            return summary;
        }

        //public static async Task<MarketHistoryResponse> GetMarketHistory(string delta)
        //{
        //    MarketHistoryResponse marketHistory = null;
        //    HttpResponseMessage response = await client.GetAsync("");
        //    if (response.IsSuccessStatusCode)
        //        marketHistory = await response.Content.ReadAsAsync<MarketHistoryResponse>();

        //    return marketHistory;
        //}

        public static async Task<HistDataResponse> GetMarketHistoryV2(string delta, string period)
        {                        
            HistDataResponse history = new HistDataResponse();
            while (true)
            {
                try
                {
                    var mesg = new HttpRequestMessage()
                    {
                        RequestUri = new Uri("https://bittrex.com/Api/v2.0/pub/market/GetTicks?marketName=" + delta + "&tickInterval=" + period + "", UriKind.Absolute),
                        Method = HttpMethod.Get
                    };
                    HttpResponseMessage response = await client.SendAsync(mesg);

                    if (response.IsSuccessStatusCode)
                        history = await response.Content.ReadAsAsync<HistDataResponse>();
                    else if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable || history.result == null)
                    {
                        do
                        {

                            Thread.Sleep(500);
                            HttpRequestMessage mesgClone = new HttpRequestMessage()
                            {
                                RequestUri = new Uri("https://bittrex.com/Api/v2.0/pub/market/GetTicks?marketName=" + delta + "&tickInterval=" + period + "", UriKind.Absolute),
                                Method = HttpMethod.Get
                            };

                            response = await client.SendAsync(mesgClone);
                            history = await response.Content.ReadAsAsync<HistDataResponse>();

                        } while (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable || history.result == null);

                    }
                    else
                        Trace.WriteLine("FAIL:  " + response.ReasonPhrase);


                    history.MarketDelta = string.Empty;

                    if (history.result == null || history.MarketDelta == null || delta.Replace('-', '_') == null)
                        Trace.WriteLine("\r\nHIST NULL " + delta + " RESPONSE CODE: " + response.StatusCode + "\r\n\r\n");


                    history.MarketDelta = delta.Replace('-', '_');
                    if (period.ToUpper() != "ONEMIN" && history.result.Count > 0)
                        history.result.Remove(history.result.Last());
                    break;

                }
                catch (Exception e)
                {
                    //Trace.WriteLine("\r\n222HIST NULL " + delta + " RESPONSE CODE: " + response.StatusCode + "\r\n\r\n");

                }
            }
            return history;
        }
        
                     

        public static async Task<decimal> GetCBsellPrice()
        {
            HttpRequestMessage mesg = new HttpRequestMessage()
            {
                RequestUri = new Uri("https://api.coinbase.com/v2/prices/BTC-USD/sell", UriKind.Absolute),
                Method = HttpMethod.Get
            };

            mesg.Headers.Add("CB-VERSION", "2017-08-25");

            CBsellPriceResponse ticker = null;
            HttpResponseMessage response = await client.SendAsync(mesg);
            if (response.IsSuccessStatusCode)
                ticker = await response.Content.ReadAsAsync<CBsellPriceResponse>();
            else
                Trace.WriteLine("FAIL:  " + response.ReasonPhrase);

            return Convert.ToDecimal(ticker.data.amount);
        }


        public static async Task<List<string>> GetTopMarketsByBVwithETHdelta(int n)
        {
            MarketSummary markets = await BtrexREST.GetMarketSummary();
            Dictionary<string, decimal> topMarketsBTC = new Dictionary<string, decimal>();
            List<string> topMarketsETH = new List<string>();
            foreach (SummaryResult market in markets.result)
            {
                string mkbase = market.MarketName.Split('-')[0];
                if (mkbase == "BTC")
                {
                    topMarketsBTC.Add(market.MarketName, market.BaseVolume);
                }
                else if (mkbase == "ETH")
                {
                    topMarketsETH.Add(market.MarketName.Split('-')[1]);
                }
            }

            List<string> mks = new List<string>();
            foreach (KeyValuePair<string, decimal> mk in topMarketsBTC.OrderByDescending(x => x.Value).Take(n))
            {
                string coin = mk.Key.Split('-')[1];
                if (topMarketsETH.Contains(coin))
                    mks.Add(coin);
            }

            Trace.WriteLine(string.Format("Markets: {0}", mks.Count));
            return mks;
        }

        public static async Task<List<string>> GetTopMarketsByBVbtcOnly(int n)
        {
            MarketSummary markets = await BtrexREST.GetMarketSummary();
            Dictionary<string, decimal> topMarketsBTC = new Dictionary<string, decimal>();
            foreach (SummaryResult market in markets.result)
            {
                string mkbase = market.MarketName.Split('-')[0];
                if (mkbase == "BTC")
                {
                    topMarketsBTC.Add(market.MarketName, market.BaseVolume);
                }
            }

            List<string> mks = new List<string>();
            foreach (KeyValuePair<string, decimal> mk in topMarketsBTC.OrderByDescending(x => x.Value).Take(n))
            {
                mks.Add(mk.Key.Split('-')[1]);
            }

            return mks;
        }

    }
}
