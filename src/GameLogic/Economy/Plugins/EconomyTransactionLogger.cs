// <copyright file="EconomyTransactionLogger.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Economy;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Persistence;
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
        var sellerId = seller.SelectedCharacter?.Id;
        var buyerId = buyer.SelectedCharacter?.Id;
        var itemName = item.Definition?.Name ?? "Unknown";
        var price = item.StorePrice;
        var contextProvider = seller.GameContext.PersistenceContextProvider;

        _ = this.LogTransactionAsync(
            EconomyTransactionType.PersonalStore,
            contextProvider,
            sellerId,
            buyerId,
            itemName,
            price);
    }

    /// <inheritdoc />
    public void ItemTraded(ITrader source, ITrader target, Item item)
    {
        if (source is not Player seller || target is not Player buyer)
        {
            return;
        }

        var sellerId = seller.SelectedCharacter?.Id;
        var buyerId = buyer.SelectedCharacter?.Id;
        var itemName = item.Definition?.Name ?? "Unknown";
        var contextProvider = seller.GameContext.PersistenceContextProvider;

        _ = this.LogTransactionAsync(
            EconomyTransactionType.DirectTrade,
            contextProvider,
            sellerId,
            buyerId,
            itemName,
            null);
    }

    private async Task LogTransactionAsync(
        EconomyTransactionType type,
        IPersistenceContextProvider contextProvider,
        Guid? sellerId,
        Guid? buyerId,
        string itemName,
        int? priceZen)
    {
        try
        {
            using var context = contextProvider.CreateNewContext();

            var transaction = context.CreateNew<EconomyTransaction>();
            transaction.Timestamp = DateTime.UtcNow;
            transaction.TransactionType = type;
            transaction.SellerId = sellerId;
            transaction.BuyerId = buyerId;
            transaction.ItemName = itemName;
            transaction.Quantity = 1;
            transaction.PriceZen = priceZen;

            await context.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (Exception)
        {
            // Silently fail — economy logging must never break the game
        }
    }
}
