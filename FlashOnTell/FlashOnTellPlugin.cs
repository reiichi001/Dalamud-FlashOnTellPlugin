using System;
using System.Diagnostics;
using Dalamud.Data;
using Dalamud.Game.ClientState;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using System.Runtime.InteropServices;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.IoC;
using Dalamud.Plugin;
using FlashOnTell.Attributes;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Logging;

namespace FlashOnTell
{
    public class FlashOnTellPlugin : IDalamudPlugin
    {
        private PluginCommandManager<FlashOnTellPlugin> commandManager;
        public Configuration Config;

        [PluginService]
        public DalamudPluginInterface Interface { get; private set; }

        [PluginService]
        public ClientState State { get; private set; }

        [PluginService]
        public ChatGui Chat { get; set; }

        [PluginService]
        public DataManager Data { get; set; }

        public FlashOnTellPlugin(CommandManager command)
        {
            this.Config = (Configuration)this.Interface.GetPluginConfig() ?? new Configuration();
            this.Config.Initialize(this.Interface);

            this.Chat.ChatMessage += ChatOnOnChatMessage;

            this.commandManager = new PluginCommandManager<FlashOnTellPlugin>(this, command);
        }

        private void ChatOnOnChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            // don't flash if it's a handled request.
            if (isHandled)
                return;

            if (type == XivChatType.TellIncoming)
            {

                if (FlashWindow.ApplicationIsActivated())
                {
                    // don't flash if FFXIV is already active. There's no point and it hurts performance.
                    // this.Chat.Print($"Flash on tell didn't fuck your client today. :)");
                    // isHandled = true;
                    PluginLog.Information("Ignored flashontell because window was active.");
                    return;
                }
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
                PluginLog.Information("Processed flashontell because window was't active.");
                // isHandled = true;
            }
            
        }

        [Command("/pflash")]
        [HelpMessage("Prints sample text to the chatbox")]
        public void FlashOnTellCommand(string command, string args)
        {
            // You may want to assign these references to private variables for convenience.
            // Keep in mind that the local player does not exist until after logging in.
            this.Chat.Print($"This doesn't do anything yet.");
        }

        public string Name => "flashontell plugin";

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
            this.commandManager.Dispose();

            this.Interface.SavePluginConfig(this.Config);
            this.Chat.ChatMessage -= ChatOnOnChatMessage;
            this.Interface.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
