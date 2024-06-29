using Dalamud.Configuration;
using Dalamud.Plugin;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FlashOnTell
{
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; }

        // Add any other properties or methods here.
        [JsonIgnore] private IDalamudPluginInterface pluginInterface;

        public void Initialize(IDalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;

        }

        public void Save()
        {
            this.pluginInterface.SavePluginConfig(this);
        }
    }
}
