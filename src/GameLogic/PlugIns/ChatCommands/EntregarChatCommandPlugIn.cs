// <copyright file="EntregarChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Admin chat command to deliver shop purchases to a player's vault.
/// Usage: /entregar [account_name] [item_name] [quantity]
/// Example: /entregar rodridevarg "Jewel of Bless" 5
/// </summary>
[Guid("F6A7B8C9-D0E1-2345-F012-123456789015")]
[PlugIn]
[ChatCommandHelp(Command, CharacterStatus.GameMaster)]
public class EntregarChatCommandPlugIn : IChatCommandPlugIn
{
    private const string Command = "/entregar";

    /// <inheritdoc />
    public string Key => Command;

    /// <inheritdoc />
    public CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 4)
        {
            await player.ShowBlueMessageAsync("Uso: /entregar [cuenta] [item] [cantidad]").ConfigureAwait(false);
            return;
        }

        var accountName = parts[1];
        var itemName = string.Join(" ", parts[2..^1]).Trim('"');
        if (!int.TryParse(parts[^1], out var quantity) || quantity < 1 || quantity > 100)
        {
            await player.ShowBlueMessageAsync("Cantidad invalida. Maximo 100.").ConfigureAwait(false);
            return;
        }

        try
        {
            using var context = player.GameContext.PersistenceContextProvider.CreateNewPlayerContext(player.GameContext.Configuration);

            // Find account
            var accounts = await context.GetAsync<Account>().ConfigureAwait(false);
            var targetAccount = accounts.FirstOrDefault(a => string.Equals(a.LoginName, accountName, StringComparison.OrdinalIgnoreCase));

            if (targetAccount?.Vault is null)
            {
                await player.ShowBlueMessageAsync($"Cuenta '{accountName}' no encontrada o sin vault.").ConfigureAwait(false);
                return;
            }

            // Find item definition
            var itemDef = player.GameContext.Configuration.Items.FirstOrDefault(i => string.Equals(i.Name.ToString(), itemName, StringComparison.OrdinalIgnoreCase));
            if (itemDef is null)
            {
                await player.ShowBlueMessageAsync($"Item '{itemName}' no encontrado.").ConfigureAwait(false);
                return;
            }

            // Add items to vault
            var added = 0;
            for (var i = 0; i < quantity; i++)
            {
                var newItem = context.CreateNew<Item>();
                newItem.Definition = itemDef;
                newItem.Durability = 1;
                newItem.ItemSlot = (byte)(targetAccount.Vault.Items.Count + i);
                targetAccount.Vault.Items.Add(newItem);
                added++;
            }

            await context.SaveChangesAsync().ConfigureAwait(false);

            await player.ShowBlueMessageAsync($"Entregado: {added}x {itemName} al vault de {accountName}.").ConfigureAwait(false);
            player.Logger.LogInformation("Admin delivered {Quantity}x {ItemName} to account {Account}", added, itemName, accountName);
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, "Error processing /entregar command.");
            await player.ShowBlueMessageAsync("Error al entregar items. Revisa logs.").ConfigureAwait(false);
        }
    }
}
