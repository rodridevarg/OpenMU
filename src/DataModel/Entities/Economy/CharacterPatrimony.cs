// <copyright file="CharacterPatrimony.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// A snapshot of a character's estimated wealth.
/// </summary>
public class CharacterPatrimony
{
    /// <summary>
    /// Gets or sets the snapshot timestamp.
    /// </summary>
    public DateTime SnapshotTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the character.
    /// </summary>
    [Required]
    public virtual Character? Character { get; set; }

    /// <summary>
    /// Gets or sets the total patrimony value in reference points (Zen-equivalent).
    /// </summary>
    public decimal TotalPatrimony { get; set; }

    /// <summary>
    /// Gets or sets the Zen value.
    /// </summary>
    public decimal ZenValue { get; set; }

    /// <summary>
    /// Gets or sets the Bless count.
    /// </summary>
    public int BlessCount { get; set; }

    /// <summary>
    /// Gets or sets the Bless market value.
    /// </summary>
    public decimal BlessValue { get; set; }

    /// <summary>
    /// Gets or sets the Soul count.
    /// </summary>
    public int SoulCount { get; set; }

    /// <summary>
    /// Gets or sets the Soul market value.
    /// </summary>
    public decimal SoulValue { get; set; }

    /// <summary>
    /// Gets or sets the Life count.
    /// </summary>
    public int LifeCount { get; set; }

    /// <summary>
    /// Gets or sets the Life market value.
    /// </summary>
    public decimal LifeValue { get; set; }

    /// <summary>
    /// Gets or sets the Chaos count.
    /// </summary>
    public int ChaosCount { get; set; }

    /// <summary>
    /// Gets or sets the Chaos market value.
    /// </summary>
    public decimal ChaosValue { get; set; }
}
