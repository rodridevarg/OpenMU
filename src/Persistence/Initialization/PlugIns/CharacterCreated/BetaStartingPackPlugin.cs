// <copyright file="BetaStartingPackPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.PlugIns.CharacterCreated;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Plugin which gives beta testers a starting set +7+L+Option and 10 Bless + 10 Soul jewels.
/// </summary>
[Guid("B4A2C1D0-E5F6-7890-ABCD-E12345678902")]
[PlugIn]
[Display(Name = "Beta Starting Pack", Description = "Gives new characters a +7+L+Opt set and 10 Bless + 10 Soul jewels for beta testing.")]
public class BetaStartingPackPlugin : ICharacterCreatedPlugIn
{
    /// <inheritdoc/>
    public void CharacterCreated(Player player, Character createdCharacter)
    {
        using var logScope = player.Logger.BeginScope(this.GetType());
        if (createdCharacter.Inventory is null)
        {
            return;
        }

        // Determine set number based on character class
        byte setNumber = createdCharacter.CharacterClass?.Number switch
        {
            // Dark Knight / Blade Knight / BladeMaster -> Dragon Set
            0 or 2 or 3 => 1,
            // Dark Wizard / SoulMaster / GrandMaster -> Legendary Set
            4 or 6 or 7 => 3,
            // FairyElf / MuseElf / HighElf -> Guardian Set
            8 or 10 or 11 => 14,
            // MagicGladiator / DuelMaster -> Brave Set
            12 or 13 => 46,
            // DarkLord / LordEmperor -> Valiant Set
            16 or 17 => 37,
            // Summoner / BloodySummoner / DimensionMaster -> Red Wing Set
            20 or 22 or 23 => 40,
            // RageFighter / FistMaster -> Hades Set
            24 or 25 => 52,
            // Default: Leather Set
            _ => 5
        };

        var setName = createdCharacter.CharacterClass?.Number switch
        {
            0 or 2 or 3 => "Dragon",
            4 or 6 or 7 => "Legendary",
            8 or 10 or 11 => "Guardian",
            12 or 13 => "Brave",
            16 or 17 => "Valiant",
            20 or 22 or 23 => "Red Wing",
            24 or 25 => "Hades",
            _ => "Leather"
        };

        player.Logger.LogInformation(
            "Giving beta starting pack to {0}: {1} Set +7+L+Opt, 10 Bless, 10 Soul.",
            createdCharacter.Name,
            setName);

        // Define armor pieces and their inventory slots
        var pieces = new[]
        {
            (Slot: (byte)2, Group: (byte)7, Name: "Helm"),      // Helm
            (Slot: (byte)3, Group: (byte)8, Name: "Armor"),    // Armor
            (Slot: (byte)4, Group: (byte)9, Name: "Pants"),    // Pants
            (Slot: (byte)5, Group: (byte)10, Name: "Gloves"),  // Gloves
            (Slot: (byte)6, Group: (byte)11, Name: "Boots"),   // Boots
        };

        // Create each armor piece +7 + Luck + Option(+4)
        foreach (var (slot, group, name) in pieces)
        {
            if (createdCharacter.Inventory.Items.Any(i => i.ItemSlot == slot))
            {
                player.Logger.LogWarning("Slot {0} already occupied, skipping {1}.", slot, name);
                continue;
            }

            var itemDef = player.GameContext.Configuration.Items
                .FirstOrDefault(d => d.Group == group && d.Number == setNumber);

            if (itemDef is null)
            {
                player.Logger.LogWarning("Could not find definition for {0} Set {1} (G:{2}, N:{3}).", setName, name, group, setNumber);
                continue;
            }

            var item = player.PersistenceContext.CreateNew<Item>();
            item.Definition = itemDef;
            item.Level = 7;
            item.Durability = itemDef.Durability;
            item.ItemSlot = slot;

            // Add Luck
            var luckOption = itemDef.PossibleItemOptions
                .SelectMany(o => o.PossibleOptions)
                .FirstOrDefault(o => o.OptionType == ItemOptionTypes.Luck);
            if (luckOption is not null)
            {
                var luckLink = player.PersistenceContext.CreateNew<ItemOptionLink>();
                luckLink.ItemOption = luckOption;
                item.ItemOptions.Add(luckLink);
            }

            // Add Option +4 (level 1)
            var normalOption = itemDef.PossibleItemOptions
                .SelectMany(o => o.PossibleOptions)
                .FirstOrDefault(o => o.OptionType == ItemOptionTypes.Option);
            if (normalOption is not null)
            {
                var optionLink = player.PersistenceContext.CreateNew<ItemOptionLink>();
                optionLink.ItemOption = normalOption;
                optionLink.Level = 1; // +4
                item.ItemOptions.Add(optionLink);
            }

            createdCharacter.Inventory.Items.Add(item);
            player.Logger.LogDebug("Added {0} Set {1} +7+L+Opt to slot {2}.", setName, name, slot);
        }

        // Create 10 Jewel of Bless and 10 Jewel of Soul in free inventory slots
        byte currentSlot = 12; // First free inventory slot after equipment
        foreach (var jewelNumber in new[] { (byte)13, (byte)14 }) // 13=Bless, 14=Soul
        {
            var jewelName = jewelNumber == 13 ? "Jewel of Bless" : "Jewel of Soul";
            for (int i = 0; i < 10; i++)
            {
                // Find next free slot
                while (currentSlot < 76 && createdCharacter.Inventory.Items.Any(x => x.ItemSlot == currentSlot))
                {
                    currentSlot++;
                }

                if (currentSlot >= 76)
                {
                    player.Logger.LogWarning("Inventory full, could not add all jewels.");
                    break;
                }

                var jewelDef = player.GameContext.Configuration.Items
                    .FirstOrDefault(d => d.Group == 14 && d.Number == jewelNumber);

                if (jewelDef is null)
                {
                    player.Logger.LogWarning("Could not find definition for {0}.", jewelName);
                    continue;
                }

                var jewel = player.PersistenceContext.CreateNew<Item>();
                jewel.Definition = jewelDef;
                jewel.Durability = 1;
                jewel.ItemSlot = currentSlot;
                createdCharacter.Inventory.Items.Add(jewel);
                currentSlot++;
            }
        }

        player.Logger.LogInformation(
            "Beta starting pack delivered to {0} successfully.",
            createdCharacter.Name);
    }
}
