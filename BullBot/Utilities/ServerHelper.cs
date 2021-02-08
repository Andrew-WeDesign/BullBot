using BullBot.Common;
using Discord;
using Discord.WebSocket;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BullBot.Utilities
{
    public class ServerHelper
    {

        private readonly Servers _servers;
        private readonly Ranks _ranks;
        private readonly AutoRoles _autoRoles;

        public ServerHelper(AutoRoles autoRoles, Servers servers, Ranks ranks)
        {
            _autoRoles = autoRoles;
            _servers = servers;
            _ranks = ranks;
        }

        public async Task SendLogAsync(IGuild guild, string title, string description)
        {
            var channelId = await _servers.GetLogsASync(guild.Id);
            if (channelId == 0)
                return;

            var fetchedChannel = await guild.GetTextChannelAsync(channelId);
            if (fetchedChannel == null)
            {
                await _servers.ClearLogsAsync(guild.Id);
                return;
            }

            await fetchedChannel.SendLogAsync(title, description);
        }

        public async Task<List<IRole>> GetRanksAsync(IGuild guild)
        {
            var roles = new List<IRole>();
            var invalidRanks = new List<Rank>();

            var ranks = await _ranks.GetRanksAsync(guild.Id);

            foreach (var rank in ranks)
            {
                var role = guild.Roles.FirstOrDefault(x => x.Id == rank.RoleId);
                if (role == null)
                {
                    invalidRanks.Add(rank);
                }
                else
                {
                    var currentUser = await guild.GetCurrentUserAsync();
                    var heirarchy = (currentUser as SocketGuildUser).Hierarchy;

                    if (role.Position > heirarchy)
                        invalidRanks.Add(rank);
                    else
                        roles.Add(role);
                }
            }

            if (invalidRanks.Count > 0)
                await _ranks.ClearRanksAsync(invalidRanks);

            return roles;
        }

        public async Task<List<IRole>> GetAutoRolesAsync(IGuild guild)
        {
            var roles = new List<IRole>();
            var invalidAutoRoles = new List<AutoRole>();

            var autoRoles = await _autoRoles.GetAutoRolesAsync(guild.Id);

            foreach (var autoRole in autoRoles)
            {
                var role = guild.Roles.FirstOrDefault(x => x.Id == autoRole.RoleId);
                if (role == null)
                {
                    invalidAutoRoles.Add(autoRole);
                }
                else
                {
                    var currentUser = await guild.GetCurrentUserAsync();
                    var heirarchy = (currentUser as SocketGuildUser).Hierarchy;

                    if (role.Position > heirarchy)
                        invalidAutoRoles.Add(autoRole);
                    else
                        roles.Add(role);
                }
            }

            if (invalidAutoRoles.Count > 0)
                await _autoRoles.ClearAutoRolesAsync(invalidAutoRoles);

            return roles;

        }

    }
}
