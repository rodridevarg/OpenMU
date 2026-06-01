// <copyright file="LostTower.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The initialization for the Lost Tower map.
/// </summary>
internal class LostTower : Version075.Maps.LostTower
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LostTower"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public LostTower(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    protected override string TerrainVersionPrefix => string.Empty;

    /// <inheritdoc />
    protected override IEnumerable<MonsterSpawnArea> CreateMonsterSpawns()
    {
        foreach (var spawn in base.CreateMonsterSpawns())
        {
            yield return spawn;
        }

        // Beta spots - hard to find (normal quantity)
        yield return this.CreateMonsterSpawn(900, this.NpcDictionary[37], 5, 15, 5, 15, 4); // Devil x4
        yield return this.CreateMonsterSpawn(901, this.NpcDictionary[41], 225, 235, 235, 245, 6); // Death Cow x6
    }
}