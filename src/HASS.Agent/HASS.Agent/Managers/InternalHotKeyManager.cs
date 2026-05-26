using HASS.Agent.Commands;
using HASS.Agent.HomeAssistant;
using HASS.Agent.Shared.Enums;
using NHotkey;
using Serilog;

namespace HASS.Agent.Managers
{
    internal class InternalHotKeyManager
    {
        internal event EventHandler<HotkeyEventArgs> HotkeyActivated;

        /// <summary>
        /// Initializes the quickaction hotkeys
        /// </summary>
        internal void InitializeQuickActionsHotKeys()
        {
            Variables.MainForm?.BeginInvoke(new MethodInvoker(delegate
            {
                // first the global hotkey
                InitializeGlobalQuickActionsHotKey();

                // then the individual hotkeys
                InitializeIndividualQuickActionsHotKeys();
            }));
        }

        /// <summary>
        /// Reloads the global- and individual quickaction hotkey bindings
        /// </summary>
        internal void ReloadQuickActionsHotKeys()
        {
            Variables.MainForm?.BeginInvoke(new MethodInvoker(delegate
            {
                // remove all bindings
                RemoveAllRegisteredHotkeys();

                // reload
                InitializeQuickActionsHotKeys();
            }));
        }

        /// <summary>
        /// Looks up the specific quick action bound to the specified hotkey, and executes it
        /// </summary>
        /// <param name="hotkey"></param>
        internal static void ProcessQuickActionHotKey(string hotkey)
        {
            if (string.IsNullOrEmpty(hotkey)) return;

            // check if we stil have the hotkey bound to a quickaction
            if (Variables.QuickActions.All(x => x.HotKey != hotkey))
            {
                Log.Warning("[HOTKEY] Registered hotkey no longer bound to a QuickAction: {hotkey}", hotkey);
                return;
            }

            // fetch the associated quickaction
            var quickAction = Variables.QuickActions.Find(x => x.HotKey == hotkey);
            if (quickAction == null)
            {
                Log.Error("[HOTKEY] Registered hotkey not found: {hotkey}", hotkey);
                return;
            }

            if (!quickAction.HotKeyEnabled)
            {
                Log.Warning("[HOTKEY] QuickAction bound to hotkey has 'hotkey enabled' set to false: {hotkey}", hotkey);
                return;
            }

            // is it an internal command?
            if (quickAction.Domain == HassDomain.HASSAgentCommands)
            {
                // execute local command
                Task.Run(() => CommandsManager.ExecuteCommandByName(quickAction.Entity));
            }
            else
            {
                // execute the command through HA
                Task.Run(() => HassApiManager.ProcessQuickActionAsync(quickAction));
            }
        }

        internal void RemoveAllRegisteredHotkeys()
        {
            foreach (var quickAction in Variables.QuickActions)
            {
                Variables.HotKeyListener?.Remove(quickAction.HotKey);
            }
        }
        
        private void InitializeGlobalQuickActionsHotKey()
        {
            Variables.MainForm?.BeginInvoke(new MethodInvoker(delegate
            {
                // check if it's enabled and configured
                if (!Variables.AppSettings.QuickActionsHotKeyEnabled || string.IsNullOrWhiteSpace(Variables.QuickActionsHotKey) || Variables.QuickActionsHotKey == "None")
                {
                    return;
                }

                // all good, bind
                var globalHotkey = HotkeyFromString(Variables.QuickActionsHotKey);
                if (globalHotkey.Item1 != Keys.None)
                {
                    Variables.HotKeyListener?.AddOrReplace(Variables.QuickActionsHotKey, globalHotkey.Item1 | globalHotkey.Item2, OnHotkeyActivated);
                    Log.Information("[HOTKEY] Completed bind for global quickaction hotkey");
                }
                else
                {
                    Log.Warning("[HOTKEY] Could not bind for global quickaction hotkey");
                }
            }));
        }

        private void InitializeIndividualQuickActionsHotKeys()
        {
            Variables.MainForm?.BeginInvoke(new MethodInvoker(delegate
            {
                var count = 0;
                foreach (var quickAcion in Variables.QuickActions.Where(x =>
                             x.HotKeyEnabled && !string.IsNullOrWhiteSpace(x.HotKey)))
                {
                    try
                    {
                        var hotkey = HotkeyFromString(quickAcion.HotKey);
                        Variables.HotKeyListener?.AddOrReplace(quickAcion.HotKey, hotkey.Item1 | hotkey.Item2, OnHotkeyActivated);
                        count++;
                    }
                    catch (Exception ex)
                    {
                        Log.Fatal(ex, "[HOTKEYS] Unable to bind individual quickaction hotkey '{hotkey}': {msg}",
                            quickAcion.HotKey, ex.Message);
                    }
                }

                if (count == 0) return;
                Log.Information("[HOTKEY] Completed bind for {count} individual quickaction hotkeys", count);
            }));
        }

        /// <summary>
        /// Process a changed quickactions hotkey
        /// </summary>
        /// <param name="previousHotkey"></param>
        /// <param name="register"></param>
        internal void QuickActionsHotKeyChanged(string previousHotkey, bool register = true)
        {
            Variables.MainForm?.BeginInvoke(new MethodInvoker(delegate
            {
                Variables.HotKeyListener?.Remove(previousHotkey);
                if (!register || Variables.QuickActionsHotKey == null 
                              || !string.IsNullOrWhiteSpace(Variables.QuickActionsHotKey))
                {
                    return;
                }
                
                var parsedHotkey = HotkeyFromString(previousHotkey);
                if (parsedHotkey.Item1 != Keys.None)
                {
                    Variables.HotKeyListener?.AddOrReplace(Variables.QuickActionsHotKey,
                        parsedHotkey.Item1 | parsedHotkey.Item2, OnHotkeyActivated);
                }
            }));
        }

        private void OnHotkeyActivated(object sender, HotkeyEventArgs e)
        {
            HotkeyActivated?.Invoke(sender, e);
        }

        private static (Keys, Keys) HotkeyFromString(string stringHotkey)
        {
            if (string.IsNullOrWhiteSpace(stringHotkey))
            {
                return (Keys.None, Keys.None);
            }

            var parts = stringHotkey.Split("+", 2, StringSplitOptions.TrimEntries);
            var modifiersString = parts.Length == 2 ? parts[0] : string.Empty;
            var keyString = parts.Length == 2 ? parts[1] : parts[0];

            var modifiers = Keys.None;
            foreach (var modKey in modifiersString.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                if (!Enum.TryParse<Keys>(modKey, out var parsedModifiers))
                {
                    continue;
                }

                modifiers |= parsedModifiers;
            }

            return Enum.TryParse<Keys>(keyString, out var parsedKey) ? (parsedKey, modifiers) : (Keys.None, Keys.None);
        }
    }
}