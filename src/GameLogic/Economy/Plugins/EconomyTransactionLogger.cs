// <copyright file="EconomyTransactionLogger.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Economy;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Plugin which logs player-to-player transactions for the economy system.
/// </summary>
[Guid("C3D4E5F6-A7B8-9012-CDEF-123456789012")]
[PlugIn]
public class EconomyTransactionLogger : IItemSoldToOtherPlayerPlugIn, IItemTradedToOtherPlayerPlugIn
{
    /// <inheritdoc />
    public void ItemSold(Player seller, Item item, Player buyer)
    {
        _ = this.LogTransactionAsync(
            EconomyTransactionType.PersonalStore,
            seller,
            buyer,
            item,
            item.StorePrice);
    }

    /// <inheritdoc />
    public void ItemTraded(ITrader source, ITrader target, Item item)
    {
        if (source is not Player seller || target is not Player buyer)
        {
            return;
        }

        _ = this.LogTransactionAsync(
            EconomyTransactionType.DirectTrade,
            seller,
            buyer,
            item,
            null);
    }

    private async Task LogTransactionAsync(
        EconomyTransactionType type,
        Player seller,
        Player buyer,
        Item item,
        int? priceZen)
    {
        try
        {
            using var context = seller.GameContext.PersistenceContextProvider.CreateNewContext();

            var transaction = context.CreateNew<EconomyTransaction>();
            transaction.Timestamp = DateTime.UtcNow;
            transaction.TransactionType = type;
            transaction.Seller = seller.SelectedCharacter;
            transaction.Buyer = buyer.SelectedCharacter;
            transaction.ItemName = item.Definition?.Name ?? "Unknown";
            transaction.Quantity = 1;
            transaction.PriceZen = priceZen;

            await context.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            seller.Logger.LogError(ex, "Failed to log economy transaction.");
        }
    }
}
