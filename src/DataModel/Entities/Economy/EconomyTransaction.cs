// <copyright file="EconomyTransaction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// The type of economy transaction.
/// </summary>
public enum EconomyTransactionType
{
    /// <summary>
    /// Transaction through personal store.
    /// </summary>
    PersonalStore,

    /// <summary>
    /// Direct player to player trade.
    /// </summary>
    DirectTrade,

    /// <summary>
    /// Jewel exchange at Cambista NPC.
    /// </summary>
    Exchange,

    /// <summary>
    /// Admin shop delivery.
    /// </summary>
    AdminDelivery,
}

/// <summary>
/// A transaction log entry for the player-to-player economy.
/// </summary>
public class EconomyTransaction
{
    /// <summary>
    /// Gets or sets the transaction timestamp.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the transaction type.
    /// </summary>
    public EconomyTransactionType TransactionType { get; set; }

    /// <summary>
    /// Gets or sets the seller character (null for NPC/admin).
    /// </summary>
    public virtual Character? Seller { get; set; }

    /// <summary>
    /// Gets or sets the buyer character (null for admin sinks).
    /// </summary>
    public virtual Character? Buyer { get; set; }

    /// <summary>
    /// Gets or sets the name of the item that was traded.
    /// </summary>
    [Required]
    public string ItemName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity of items traded.
    /// </summary>
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// Gets or sets the price in Zen (if applicable).
    /// </summary>
    public int? PriceZen { get; set; }

    /// <summary>
    /// Gets or sets the name of the payment item (if barter).
    /// </summary>
    public string? PaymentItemName { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the payment item (if barter).
    /// </summary>
    public int? PaymentItemQuantity { get; set; }

    /// <summary>
    /// Gets or sets the Zen commission retained by the system (if applicable).
    /// </summary>
    public int? CommissionZen { get; set; }
}
