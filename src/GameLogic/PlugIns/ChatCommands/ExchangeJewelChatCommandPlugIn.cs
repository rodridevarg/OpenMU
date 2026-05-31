// <copyright file="ExchangeJewelChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which handles jewel exchange at the Cambista.
/// </summary>
[Guid("B2C3D4E5-F6A7-8901-BCDE-F12345678901")]
[PlugIn]
[ChatCommandHelp(Command, CharacterStatus.Normal)]
public class ExchangeJewelChatCommandPlugIn : IChatCommandPlugIn
{
    private const string Command = "/exchange";
    private const int CommissionZen = 500000;

    private static readonly Dictionary<string, string> JewelNames = new(StringComparer.OrdinalIgnoreCase)
    {
        { "bless", "Jewel of Bless" },
        { "soul", "Jewel of Soul" },
        { "life", "Jewel of Life" },
        { "chaos", "Jewel of Chaos" },
    };

    /// <inheritdoc />
    public string Key => Command;

    /// <inheritdoc />
    public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.Normal;

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 3)
        {
            await player.ShowBlueMessageAsync("Uso: /exchange [bless|soul|life|chaos] [bless|soul|life|chaos]").ConfigureAwait(false);
            return;
        }

        var fromKey = parts[1];
        var toKey = parts[2];

        if (!JewelNames.TryGetValue(fromKey, out var fromName) || !JewelNames.TryGetValue(toKey, out var toName))
        {
            await player.ShowBlueMessageAsync("Joyas validas: bless, soul, life, chaos.").ConfigureAwait(false);
            return;
        }

        if (fromKey.Equals(toKey, StringComparison.OrdinalIgnoreCase))
        {
            await player.ShowBlueMessageAsync("No puedes intercambiar una joya por si misma.").ConfigureAwait(false);
            return;
        }

        var fromDef = player.GameContext.Configuration.Items.FirstOrDefault(i => i.Name == fromName);
        var toDef = player.GameContext.Configuration.Items.FirstOrDefault(i => i.Name == toName);

        if (fromDef is null || toDef is null)
        {
            await player.ShowBlueMessageAsync("Definicion de joya no encontrada en el servidor.").ConfigureAwait(false);
            return;
        }

        if (player.Inventory is null)
        {
            await player.ShowBlueMessageAsync("Inventario no disponible.").ConfigureAwait(false);
            return;
        }

        var sourceItem = player.Inventory.Items.FirstOrDefault(i => i.Definition == fromDef);
        if (sourceItem is null)
        {
            await player.ShowBlueMessageAsync($"No tienes {fromName} en tu inventario.").ConfigureAwait(false);
            return;
        }

        if (player.Money < CommissionZen)
        {
            await player.ShowBlueMessageAsync($"Necesitas {CommissionZen:N0} Zen para la comision.").ConfigureAwait(false);
            return;
        }

        var tempItem = player.PersistenceContext.CreateNew<Item>();
        tempItem.Definition = toDef;
        tempItem.Durability = 1;
        var freeSlot = player.Inventory.CheckInvSpace(tempItem);
        if (freeSlot is null)
        {
            await player.ShowBlueMessageAsync("Tu inventario esta lleno.").ConfigureAwait(false);
            return;
        }

        if (!player.TryRemoveMoney(CommissionZen))
        {
            await player.ShowBlueMessageAsync("No se pudo deducir la comision en Zen.").ConfigureAwait(false);
            return;
        }

        await player.DestroyInventoryItemAsync(sourceItem).ConfigureAwait(false);

        var newItem = player.PersistenceContext.CreateNew<Item>();
        newItem.Definition = toDef;
        newItem.Durability = 1;
        newItem.ItemSlot = (byte)freeSlot;
        await player.Inventory.AddItemAsync(newItem).ConfigureAwait(false);

        await player.ShowBlueMessageAsync($"Intercambio exitoso: 1 {fromName} -> 1 {toName}. Comision: {CommissionZen:N0} Zen.").ConfigureAwait(false);
    }
}
