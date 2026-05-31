// <copyright file="CambistaNpcPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Economy;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Plugin which handles the Cambista NPC interaction.
/// </summary>
[Guid("A1B2C3D4-E5F6-7890-ABCD-EF1234567890")]
[PlugIn]
public class CambistaNpcPlugin : IPlayerTalkToNpcPlugIn
{
    /// <summary>
    /// Gets the NPC number of the Cambista.
    /// </summary>
    public static short CambistaNpcNumber => 546;

    /// <inheritdoc />
    public async ValueTask PlayerTalksToNpcAsync(Player player, NonPlayerCharacter npc, NpcTalkEventArgs eventArgs)
    {
        if (npc.Definition.Number != CambistaNpcNumber)
        {
            return;
        }

        eventArgs.HasBeenHandled = true;
        var message = "Bienvenido al Cambista. Intercambia joyas al 1:1. Comision: 500.000 Zen. Usa /exchange [joya_origen] [joya_destino]. Ej: /exchange bless soul";
        await player.InvokeViewPlugInAsync<MUnique.OpenMU.GameLogic.Views.NPC.IShowMessageOfObjectPlugIn>(p => p.ShowMessageOfObjectAsync(message, npc)).ConfigureAwait(false);
    }
}
