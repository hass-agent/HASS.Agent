using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using Serilog;

namespace HASS.Agent.Shared.Functions
{
    /// <summary>
    /// Various unclassified helper functions
    /// </summary>
    public static class SharedHelperFunctions
    {
        /// <summary>
        /// Returns the safe variant of the configured device name, or a safe version of the machinename if nothing's stored
        /// </summary>
        /// <returns></returns>
        public static string GetSafeConfiguredDeviceName() =>
            string.IsNullOrEmpty(Variables.DeviceName)
                ? GetSafeDeviceName()
                : GetSafeValue(Variables.DeviceName);

        /// <summary>
        /// Returns a safe version of this machine's name
        /// </summary>
        /// <returns></returns>
        public static string GetSafeDeviceName() => GetSafeValue(Environment.MachineName);

        /// <summary>
        /// Returns a safe version of the provided value
        /// </summary>
        /// <returns></returns>
        public static string GetSafeValue(string value)
        {
            var val = Regex.Replace(value, @"[^a-zA-Z0-9_\-_\s]", "_");
            return val.Replace(" ", "");
        }

        /// <summary>
        /// Provides a dictionary containing the pointers and titles of all open windows
        /// </summary>
        /// <returns></returns>
        public static IDictionary<IntPtr, string> GetOpenWindows()
        {
            var shellWindow = NativeMethods.GetShellWindow();
            var windows = new Dictionary<IntPtr, string>();

            NativeMethods.EnumWindows(delegate (IntPtr hWnd, int lParam)
            {
                if (hWnd == shellWindow) return true;
                if (!NativeMethods.IsWindowVisible(hWnd)) return true;

                var length = NativeMethods.GetWindowTextLength(hWnd);
                if (length == 0) return true;

                var builder = new StringBuilder(length);
                NativeMethods.GetWindowText(hWnd, builder, length + 1);

                windows[hWnd] = builder.ToString();
                return true;

            }, 0);

            return windows;
        }

        /// <summary>
        /// Returns the owner of the supplied process
        /// </summary>
        /// <param name="process"></param>
        /// <param name="includeDomain"></param>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "StringIndexOfIsCultureSpecific.1")]
        public static string GetProcessOwner(Process process, bool includeDomain = true)
        {
            var processHandle = IntPtr.Zero;
            try
            {
                NativeMethods.OpenProcessToken(process.Handle, 8, out processHandle);
                using var wi = new WindowsIdentity(processHandle);
                var user = wi.Name;
                if (!includeDomain) return user.Contains(@"\") ? user[(user.IndexOf(@"\") + 1)..] : user;
                else return user;
            }
            catch
            {
                return "NO OWNER";
            }
            finally
            {
                if (processHandle != IntPtr.Zero) NativeMethods.CloseHandle(processHandle);
            }
        }

        /// <summary>
        /// Gets the category of the provided enum
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetCategory(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo == null) return null;
            var attribute = (CategoryAttribute)fieldInfo.GetCustomAttribute(typeof(CategoryAttribute));
            return attribute?.Category ?? "?";
        }

        private static string _everyoneAccountName = string.Empty;

        /// <summary>
        /// Gets the localized name of the 'Everyone' account
        /// </summary>
        /// <returns></returns>
        public static string EveryoneLocalizedAccountName()
        {
            try
            {
                if (!string.IsNullOrEmpty(_everyoneAccountName)) return _everyoneAccountName;

                // one time retrieval
                _everyoneAccountName = Principal.FindByIdentity(new PrincipalContext(ContextType.Machine), IdentityType.Sid, "S-1-1-0")?.Name ?? "Everyone";
                return _everyoneAccountName;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, ex.Message);
                return "Everyone";
            }
        }

        private static string _systemAccountName = string.Empty;

        /// <summary>
        /// Gets the localized name of the 'SYSTEM' account
        /// </summary>
        /// <returns></returns>
        public static string SystemLocalizedAccountName()
        {
            try
            {
                if (!string.IsNullOrEmpty(_systemAccountName)) return _systemAccountName;

                // eenmalig ophalen
                _systemAccountName = Principal.FindByIdentity(new PrincipalContext(ContextType.Machine), IdentityType.Sid, "S-1-5-18")?.Name ?? "SYSTEM";
                return _systemAccountName;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, ex.Message);
                return "SYSTEM";
            }
        }

        /// <summary>
        /// Returns true if the provided or current user is either SYSTEM or a service account
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool UserIsSystemOrServiceAccount(string user = "")
        {
            if (string.IsNullOrEmpty(user)) user = Environment.UserName;
            return user == SystemLocalizedAccountName() || user.ToUpper() == $"{Environment.MachineName.ToUpper()}$";
        }

        /// <summary>
        /// Parses the application name from the application's reg key
        /// </summary>
        /// <param name="subKeyName"></param>
        /// <returns></returns>
        public static string ParseRegWebcamMicApplicationName(string subKeyName)
        {
            try
            {
                // get the reg's keyname
                subKeyName = subKeyName.Split('\\').Last();
                
                // create a lowercase variant
                var subKeyLowerName = subKeyName.ToLower();

                // win app?
                if ((subKeyLowerName.StartsWith("windows") || subKeyLowerName.StartsWith("microsoft")) && subKeyLowerName.Contains("_"))
                {
                    // yep, remove the first part
                    var name = subKeyName.Replace($"{subKeyName.Split('.').First()}.", "");

                    // remove the last part
                    name = name.Replace($"_{name.Split('_').Last()}", "");

                    // done
                    return name;
                }

                // nope, regular, replace the hashes
                var appName = subKeyName.Replace("#", "\\");

                // get the application and return it
                return Path.GetFileNameWithoutExtension(appName);
            }
            catch (Exception ex)
            {
                Log.Error("Unable to parse subkey '{key}': {err}", subKeyName, ex.Message);
                return subKeyName;
            }
        }

        /// <summary>
        /// Checks a long-lived access token for the Home Assistant API
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool CheckHomeAssistantApiToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return false;

            // check for two dots
            if (token.Count(f => f == '.') != 2) return false;

            // check for length
            if (token.Length != 183) return false;

            // check for whitespace
            if (token.Contains(" ")) return false;

            // looks good
            return true;
        }

        /// <summary>
        /// Checks the Home Assistant uri for plausability
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static bool CheckHomeAssistantUri(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri)) return false;

            // check for protocol
            if (!uri.Contains("//")) return false;

            // check for whitespace
            if (uri.Contains(" ")) return false;

            // looks good, port checking is tricky, 80/443 doesn't require it
            return true;
        }

        /// <summary>
        /// Checks the MQTT broker uri for plausability
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static bool CheckMqttBrokerUri(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri)) return false;

            // check for no protocol
            if (uri.Contains("//")) return false;

            // check for no port
            if (uri.Contains(":")) return false;

            // check for whitespace
            if (uri.Contains(" ")) return false;

            // looks good
            return true;
        }

        public static bool RunningElevated() => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
    }
}
