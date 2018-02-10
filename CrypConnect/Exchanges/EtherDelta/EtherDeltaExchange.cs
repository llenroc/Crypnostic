﻿using System;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;
using HD;

namespace CryptoExchanges
{
  internal class EtherDeltaExchange : Exchange
  {
    protected readonly IRestClient restClient;

    public EtherDeltaExchange()
      : base(ExchangeName.EtherDelta)
    {
      restClient = new RestClient("https://api.etherdelta.com");
    }

    protected override void LoadTickerNames()
    {
      // TODO how do we do this for EtherDelta?
    }

    protected override async Task<List<TradingPair>> GetAllTradingPairs()
    {
      Dictionary<string, Dictionary<string, object>> tickerList;
      while (true)
      {
        try
        {
          tickerList =
            restClient.Get<Dictionary<string, Dictionary<string, object>>>(
              "returnTicker");

          if (tickerList != null)
          {
            break;
          }
        }
        catch
        {
          await Task.Delay(3500 + random.Next(2000));
        }
      }

      return AddTradingPairs(tickerList,
        (KeyValuePair<string, Dictionary<string, object>> ticker) =>
          (baseCoin: "ETH",
          quoteCoin: ticker.Key.GetAfter("_"),
          askPrice: Convert.ToDecimal(ticker.Value["ask"]),
          bidPrice: Convert.ToDecimal(ticker.Value["bid"])));
    }
  }
}
