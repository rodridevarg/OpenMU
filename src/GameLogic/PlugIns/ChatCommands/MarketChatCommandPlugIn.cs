// <copyright file="MarketChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which shows current market trends.
/// </summary>
[Guid("D4E5F6A7-B8C9-0123-DEF0-123456789013")]
[PlugIn]
[ChatCommandHelp(Command, CharacterStatus.Normal)]
public class MarketChatCommandPlugIn : IChatCommandPlugIn
{
    private const string Command = "/mercado";

    /// <inheritdoc />
    public string Key => Command;

    /// <inheritdoc />
    public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.Normal;

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        try
        {
            using var context = player.GameContext.PersistenceContextProvider.CreateNewContext();
            var now = DateTime.UtcNow;
            var last24h = now.AddHours(-24);
            var prev24h = now.AddHours(-48);

            // Get transactions for last 24h and previous 24h (limit to last 50 for performance)
            var allTransactions = (await context.GetAsync<EconomyTransaction>().ConfigureAwait(false))
                .Where(t => t.Timestamp >= prev24h && t.PriceZen.HasValue && t.PriceZen.Value > 0)
                .Take(50)
                .ToList();

            var recent = allTransactions.Where(t => t.Timestamp >= last24h).ToList();
            var previous = allTransactions.Where(t => t.Timestamp >= prev24h && t.Timestamp < last24h).ToList();

            // Get reference prices
            var refPrices = (await context.GetAsync<ReferencePrice>().ConfigureAwait(false))
                .Where(r => r.IsActive)
                .ToDictionary(r => r.ItemName, r => r.PriceInBless);

            // Calculate trends per item
            var items = recent.Select(t => t.ItemName).Union(previous.Select(t => t.ItemName)).Distinct().ToList();

            await player.ShowBlueMessageAsync("--- Mercado LibreMU (24h) ---").ConfigureAwait(false);

            if (!items.Any())
            {
                await player.ShowBlueMessageAsync("Sin movimientos recientes. Usa /patrimonio para calcular tu riqueza.").ConfigureAwait(false);
                return;
            }

            foreach (var itemName in items.OrderBy(i => i))
            {
                var recentAvg = recent.Where(t => t.ItemName == itemName).Select(t => t.PriceZen ?? 0).DefaultIfEmpty(0).Average();
                var prevAvg = previous.Where(t => t.ItemName == itemName).Select(t => t.PriceZen ?? 0).DefaultIfEmpty(0).Average();
                var count = recent.Count(t => t.ItemName == itemName);

                string trend;
                if (prevAvg > 0 && recentAvg > 0)
                {
                    var pctChange = ((recentAvg - prevAvg) / prevAvg) * 100;
                    if (pctChange > 5)
                        trend = $" ^ ({pctChange:F0}%)";
                    else if (pctChange < -5)
                        trend = $" v ({pctChange:F0}%)";
                    else
                        trend = " ->";
                }
                else
                {
                    trend = count > 0 ? " (nuevo)" : "";
                }

                var refPrice = refPrices.ContainsKey(itemName) ? $" [Ref: {refPrices[itemName]:F2} Bless]" : "";
                var message = $"{itemName}: {recentAvg:N0} Zen ({count} ops){trend}{refPrice}";
                await player.ShowBlueMessageAsync(message).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, "Error processing /mercado command.");
            await player.ShowBlueMessageAsync("Error al consultar el mercado. Intenta mas tarde.").ConfigureAwait(false);
        }
    }
}
