// <copyright file="MarketPriceSnapshot.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// A snapshot of market prices for a specific item pair.
/// </summary>
public class MarketPriceSnapshot
{
    /// <summary>
    /// Gets or sets the snapshot timestamp.
    /// </summary>
    public DateTime SnapshotTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the name of the primary item (e.g. "Jewel of Chaos").
    /// </summary>
    [Required]
    public string ItemName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the reference item (e.g. "Jewel of Bless").
    /// </summary>
    [Required]
    public string ReferenceItemName { get; set; } = "Jewel of Bless";

    /// <summary>
    /// Gets or sets the average price of the item in reference units.
    /// </summary>
    public decimal AveragePrice { get; set; }

    /// <summary>
    /// Gets or sets the number of transactions used to calculate the average.
    /// </summary>
    public int TransactionCount { get; set; }

    /// <summary>
    /// Gets or sets the lowest price in the period.
    /// </summary>
    public decimal MinPrice { get; set; }

    /// <summary>
    /// Gets or sets the highest price in the period.
    /// </summary>
    public decimal MaxPrice { get; set; }
}
