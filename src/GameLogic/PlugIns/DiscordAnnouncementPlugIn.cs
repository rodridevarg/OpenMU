// <copyright file="DiscordAnnouncementPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Sends a global announcement inviting players to join the Discord server every 30 minutes.
/// </summary>
[PlugIn]
[Guid("F4A1B2C3-D5E6-7890-1234-567890ABCDEF")]
public class DiscordAnnouncementPlugIn : IPeriodicTaskPlugIn, ISupportCustomConfiguration<DiscordAnnouncementPlugInConfiguration>, ISupportDefaultCustomConfiguration
{
    private DateTime _nextRunUtc = DateTime.UtcNow.AddMinutes(1);
    private int _messageIndex;

    /// <inheritdoc />
    public DiscordAnnouncementPlugInConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        if (DateTime.UtcNow < this._nextRunUtc)
        {
            return;
        }

        var config = this.Configuration ??= (DiscordAnnouncementPlugInConfiguration)this.CreateDefaultConfig();
        this._nextRunUtc = DateTime.UtcNow.Add(config.Interval);

        var messages = config.Messages;
        if (messages.Count == 0)
        {
            return;
        }

        var message = messages[this._messageIndex % messages.Count];
        this._messageIndex++;

        var logger = gameContext.LoggerFactory.CreateLogger(this.GetType().Name);
        logger.LogInformation("Enviando anuncio de Discord: {Message}", message);

        await gameContext.SendGlobalMessageAsync(message, Interfaces.MessageType.GoldenCenter).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void ForceStart()
    {
        this._nextRunUtc = DateTime.UtcNow;
    }

    /// <inheritdoc />
    public object CreateDefaultConfig()
    {
        return new DiscordAnnouncementPlugInConfiguration();
    }
}
