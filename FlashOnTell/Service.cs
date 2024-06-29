using Dalamud.IoC;
using Dalamud.Plugin.Services;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashOnTell
{
    public class Service
    {
        [PluginService] internal static IDalamudPluginInterface Interface { get; private set; } = null!;
        [PluginService] internal static IClientState State { get; private set; } = null!;
        [PluginService] internal static IChatGui Chat { get; private set; } = null!;
        [PluginService] internal static IDataManager Data { get; private set; } = null!;
        [PluginService] internal static IPluginLog PluginLog { get; private set; } = null!;
    }
}
