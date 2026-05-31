// <copyright file="AddRandomPetForAllCharacters.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.PlugIns.CharacterCreated;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Plugin which adds a random pet (Guardian Angel or Demon) to every new character.
/// </summary>
[Guid("E1F2A3B4-C5D6-7890-ABCD-E12345678901")]
[PlugIn]
[Display(Name = "Add Random Pet for All Characters", Description = "Gives every new character a random Guardian Angel or Demon pet.")]
public class AddRandomPetForAllCharacters : ICharacterCreatedPlugIn
{
    /// <inheritdoc/>
    public void CharacterCreated(Player player, Character createdCharacter)
    {
        using var logScope = player.Logger.BeginScope(this.GetType());
        if (createdCharacter.Inventory is null)
        {
            return;
        }

        // Randomly choose between Guardian Angel (group 13, number 0) and Demon (group 13, number 64)
        var random = new Random();
        var isAngel = random.Next(2) == 0;
        var itemGroup = (byte)13;
        var itemNumber = isAngel ? (byte)0 : (byte)64;
        var petName = isAngel ? "Guardian Angel" : "Demon";

        if (createdCharacter.Inventory.Items.FirstOrDefault(i => i.ItemSlot == 8) is { } existingItem)
        {
            player.Logger.LogError("Pet slot {0} already contains an item ({1}).", 8, existingItem);
            return;
        }

        if (player.GameContext.Configuration.Items
                .FirstOrDefault(def => def.Group == itemGroup && def.Number == itemNumber)
            is { } itemDefinition)
        {
            var item = player.PersistenceContext.CreateNew<Item>();
            item.Definition = itemDefinition;
            item.Durability = item.Definition.Durability;
            item.ItemSlot = 8; // Pet slot
            createdCharacter.Inventory.Items.Add(item);
            player.Logger.LogInformation("Added {0} to new character {1}.", petName, createdCharacter.Name);
        }
        else
        {
            player.Logger.LogWarning("Unknown pet item, group {0}, number {1}.", itemGroup, itemNumber);
        }
    }
}
