// <copyright file="SystemBank.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// A singleton accumulator for server-side commissions and fees.
/// There should always be exactly one row with Id = 1.
/// </summary>
public class SystemBank
{
    /// <summary>
    /// Gets or sets the singleton id.
    /// </summary>
    public Guid Id { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

    /// <summary>
    /// Gets or sets the total Zen collected from commissions.
    /// </summary>
    public long TotalZenCollected { get; set; }

    /// <summary>
    /// Gets or sets the total number of transactions that incurred a commission.
    /// </summary>
    public long TotalTransactions { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the last update.
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
