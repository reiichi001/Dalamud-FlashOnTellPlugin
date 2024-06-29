using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.IoC;
using Dalamud.Plugin;
using FlashOnTell.Attributes;
using Dalamud.Game.Text;
using Dalamud.Plugin.Services;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;

namespace FlashOnTell
{
    public class FlashOnTellPlugin : IDalamudPlugin
    {
        private PluginCommandManager<FlashOnTellPlugin> commandManager;
        public Configuration Config;

        public DalamudPluginInterface Interface;
        public IChatGui Chat;
        public IPluginLog Logger;

        public FlashOnTellPlugin(DalamudPluginInterface pluginInterface, ICommandManager command)
        {
            Interface = pluginInterface;
            Config = (Configuration)Interface.GetPluginConfig() ?? new Configuration();
            Config.Initialize(Interface);
            Interface.Create<Service>();

            Chat = Service.Chat;
            Logger = Service.PluginLog;
            
            Chat.ChatMessage += ChatOnOnChatMessage;

            commandManager = new PluginCommandManager<FlashOnTellPlugin>(this, command);


        }

        private void ChatOnOnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
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
                    Logger.Debug("Ignored flashontell because window was active.");
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
                Logger.Debug("Processed flashontell because window was't active.");
                // isHandled = true;
            }
            
        }

        [Command("/pflash")]
        [HelpMessage("Prints sample text to the chatbox")]
        public async void FlashOnTellCommand(string command, string args)
        {
            // You may want to assign these references to private variables for convenience.
            // Keep in mind that the local player does not exist until after logging in.
            Chat.Print($"FlashOnTell will try to flash the icon in 3 seconds. This generally won't work because it's the active window unless you tab out immediately.");
            Logger.Debug("Attempted to flashontell but window is active.");
            await Task.Delay(3000);
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

        public string Name => "flashontell plugin";

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
            commandManager.Dispose();

            Interface.SavePluginConfig(Config);
            Chat.ChatMessage -= ChatOnOnChatMessage;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
