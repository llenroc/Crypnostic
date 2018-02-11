﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoExchanges
{
  public class ExchangeMonitor
  {
    public static readonly Random random = new Random();

    readonly Exchange[] exchangeList;

    /// <summary>
    /// CoinFullName (lowercase) to Coin
    /// </summary>
    readonly Dictionary<string, Coin> coinList = new Dictionary<string, Coin>();

    public ExchangeMonitor(
      params ExchangeName[] exchangeNameList)
    {
      exchangeList = new Exchange[exchangeNameList.Length];
      for (int i = 0; i < exchangeNameList.Length; i++)
      {
        ExchangeName name = exchangeNameList[i];
        Exchange exchange = Exchange.LoadExchange(this, name);
        exchangeList[i] = exchange;
      }
    }

    public Coin FindCoin(
      string coinFullName)
    {
      if(coinList.TryGetValue(coinFullName.ToLowerInvariant(), out Coin coin) == false)
      {
        return null;
      }

      return coin;
    }

    public async Task CompleteFirstLoad()
    {
      Task[] exchangeTaskList = new Task[exchangeList.Length];
      for (int i = 0; i < exchangeList.Length; i++)
      {
        exchangeTaskList[i] = exchangeList[i].GetAllPairs();
      }

      await Task.WhenAll(exchangeTaskList);
    }

    public void AddPair(
      TradingPair pair)
    {
      if(coinList.TryGetValue(pair.quoteCoinFullName.ToLowerInvariant(), out Coin coin) == false)
      {
        coin = new Coin(pair.quoteCoinFullName);
        coinList.Add(pair.quoteCoinFullName.ToLowerInvariant(), coin);
      }
      coin.AddPair(pair);
    }
  }
}
