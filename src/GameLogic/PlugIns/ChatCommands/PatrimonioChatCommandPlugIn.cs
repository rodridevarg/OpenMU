// <copyright file="PatrimonioChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which shows the player's estimated patrimony.
/// </summary>
[Guid("E5F6A7B8-C9D0-1234-EF01-123456789014")]
[PlugIn]
[ChatCommandHelp(Command, CharacterStatus.Normal)]
public class PatrimonioChatCommandPlugIn : IChatCommandPlugIn
{
    private const string Command = "/patrimonio";

    /// <inheritdoc />
    public string Key => Command;

    /// <inheritdoc />
    public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.Normal;

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        try
        {
            var character = player.SelectedCharacter;
            if (character?.Inventory is null)
            {
                await player.ShowBlueMessageAsync("Inventario no disponible.").ConfigureAwait(false);
                return;
            }

            // Get reference prices
            using var context = player.GameContext.PersistenceContextProvider.CreateNewContext();
            var refPrices = (await context.GetAsync<ReferencePrice>().ConfigureAwait(false))
                .Where(r => r.IsActive)
                .ToDictionary(r => r.ItemName, r => r.PriceInBless);

            // Count jewels in inventory
            var blessCount = this.CountItems(character.Inventory, "Jewel of Bless");
            var soulCount = this.CountItems(character.Inventory, "Jewel of Soul");
            var lifeCount = this.CountItems(character.Inventory, "Jewel of Life");
            var chaosCount = this.CountItems(character.Inventory, "Jewel of Chaos");
            var zen = character.Inventory.Money;

            // Calculate values in Bless-equivalent points
            var blessPrice = 1m;
            var soulPrice = refPrices.GetValueOrDefault("Jewel of Soul", 0.5m);
            var lifePrice = refPrices.GetValueOrDefault("Jewel of Life", 3m);
            var chaosPrice = refPrices.GetValueOrDefault("Jewel of Chaos", 5m);

            // Estimate Zen value (arbitrary: 1M Zen = 1 Bless point for simplicity)
            var zenValue = zen / 1_000_000m;

            var total = (blessCount * blessPrice) +
                        (soulCount * soulPrice) +
                        (lifeCount * lifePrice) +
                        (chaosCount * chaosPrice) +
                        zenValue;

            await player.ShowBlueMessageAsync("--- Patrimonio LibreMU ---").ConfigureAwait(false);
            await player.ShowBlueMessageAsync($"Zen: {zen:N0} ({zenValue:N1} pts)").ConfigureAwait(false);
            await player.ShowBlueMessageAsync($"Bless: {blessCount} ({blessCount * blessPrice:N1} pts)").ConfigureAwait(false);
            await player.ShowBlueMessageAsync($"Soul: {soulCount} ({soulCount * soulPrice:N1} pts)").ConfigureAwait(false);
            await player.ShowBlueMessageAsync($"Life: {lifeCount} ({lifeCount * lifePrice:N1} pts)").ConfigureAwait(false);
            await player.ShowBlueMessageAsync($"Chaos: {chaosCount} ({chaosCount * chaosPrice:N1} pts)").ConfigureAwait(false);
            await player.ShowBlueMessageAsync($"TOTAL: {total:N1} puntos").ConfigureAwait(false);

            // Persist snapshot
            var patrimony = context.CreateNew<CharacterPatrimony>();
            patrimony.SnapshotTime = DateTime.UtcNow;
            patrimony.Character = character;
            patrimony.ZenValue = zenValue;
            patrimony.BlessCount = blessCount;
            patrimony.BlessValue = blessCount * blessPrice;
            patrimony.SoulCount = soulCount;
            patrimony.SoulValue = soulCount * soulPrice;
            patrimony.LifeCount = lifeCount;
            patrimony.LifeValue = lifeCount * lifePrice;
            patrimony.ChaosCount = chaosCount;
            patrimony.ChaosValue = chaosCount * chaosPrice;
            patrimony.TotalPatrimony = total;
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, "Error processing /patrimonio command.");
            await player.ShowBlueMessageAsync("Error al calcular patrimonio.").ConfigureAwait(false);
        }
    }

    private int CountItems(ItemStorage storage, string itemName)
    {
        return storage.Items.Count(i => i.Definition?.Name == itemName);
    }
}
