﻿using System.Threading.Tasks;
using CryptoGramBot.Helpers;
using CryptoGramBot.Models;
using CryptoGramBot.Services.Data;
using CryptoGramBot.Services.Pricing;

namespace CryptoGramBot.Services
{
    public class ProfitAndLossService
    {
        private readonly DatabaseService _databaseService;
        private readonly PriceService _priceService;

        public ProfitAndLossService(
            PriceService priceService,
            DatabaseService databaseService)
        {
            _priceService = priceService;
            _databaseService = databaseService;
        }

        public async Task<ProfitAndLoss> GetPnLInfo(string ccy1, string ccy2, string exchange)
        {
            var tradesForPair = await _databaseService.GetTradesForPair(ccy1, ccy2);
            var profitAndLoss = ProfitCalculator.GetProfitAndLossForPair(tradesForPair, new Currency { Base = ccy1, Terms = ccy2 });

            var dollarAmount = await _priceService.GetDollarAmount(ccy1, profitAndLoss.Profit, exchange);

            profitAndLoss.DollarProfit = dollarAmount;

            await _databaseService.SaveProfitAndLoss(profitAndLoss);

            return profitAndLoss;
        }
    }
}