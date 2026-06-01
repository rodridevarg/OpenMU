// <copyright file="ReferencePrice.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// An official reference price set by the administrator.
/// </summary>
public class ReferencePrice
{
    /// <summary>
    /// Gets or sets the name of the item.
    /// </summary>
    [Required]
    public string ItemName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the price in reference units (default: Bless-equivalent).
    /// </summary>
    public decimal PriceInBless { get; set; }

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets a value indicating whether this price is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}
