// <copyright file="DiscordAnnouncementPlugInConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// Configuration for the <see cref="DiscordAnnouncementPlugIn"/>.
/// </summary>
public class DiscordAnnouncementPlugInConfiguration
{
    /// <summary>
    /// Gets or sets the interval between announcements.
    /// </summary>
    public TimeSpan Interval { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// Gets or sets the list of messages to rotate.
    /// </summary>
    public List<string> Messages { get; set; } = new()
    {
        "Unete a nuestra comunidad de Discord: https://discord.gg/gUeHUANPc",
        "Tenes dudas o sugerencias? Unete al Discord de LiberMU: https://discord.gg/gUeHUANPc",
        "Busca party, tradea items y enterate de todo en Discord: https://discord.gg/gUeHUANPc",
    };
}
