using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin;
using FlashOnTell.Attributes;

namespace FlashOnTell
{
    public class FlashOnTellPlugin : IDalamudPlugin
    {
        private DalamudPluginInterface _pi;
        private PluginCommandManager<FlashOnTellPlugin> commandManager;
        private Configuration config;
        private readonly Random _rng = new Random();

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            _pi = pluginInterface;

            this.config = (Configuration)_pi.GetPluginConfig() ?? new Configuration();
            this.config.Initialize(_pi);

            pluginInterface.Framework.Gui.Chat.OnChatMessage += Chat_OnChatMessage;

            this.commandManager = new PluginCommandManager<FlashOnTellPlugin>(this, _pi);
        }

        private void Chat_OnChatMessage(Dalamud.Game.Text.XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            if (type == Dalamud.Game.Text.XivChatType.TellIncoming)
            {
                // maybe we can reflect this out of Dalamud?
                // for now, I've ripped code from http://eddiejackson.net/wp/?p=21197 and adapted it to work
                var flashInfo = new FlashWindow.FLASHWINFO
                {
                    cbSize = (uint)Marshal.SizeOf<FlashWindow.FLASHWINFO>(),
                    uCount = uint.MaxValue,
                    dwTimeout = 0,
                    dwFlags = FlashWindow.FLASHW_ALL | FlashWindow.FLASHW_TIMERNOFG,
                    hwnd = Process.GetCurrentProcess().MainWindowHandle,
                };
                FlashWindow.Flash(flashInfo);
            }
            
        }

        [Command("/FlashOnTell")]
        [HelpMessage("Prints sample text to the chatbox")]
        public void FlashOnTellCommand(string command, string args)
        {
            // You may want to assign these references to private variables for convenience.
            // Keep in mind that the local player does not exist until after logging in.
            var chat = this._pi.Framework.Gui.Chat;
            chat.Print($"This is sample text to the default Dalamud chat message type.");
        }

        public string Name => "flashontell plugin";

        public void Dispose()
        {
            _pi.Framework.Gui.Chat.OnChatMessage -= Chat_OnChatMessage;
            _pi.Dispose();
        }
    }
}
