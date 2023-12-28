using System.Diagnostics;
using System.IO;
using HASS.Agent.Enums;
using HASS.Agent.Functions;
using HASS.Agent.Models.Internal;
using HASS.Agent.Resources.Localization;
using HASS.Agent.Settings;
using Octokit;
using Serilog;
using Syncfusion.Windows.Forms;

namespace HASS.Agent.Managers;

internal static class UpdateManager
{
    private static readonly DateTimeOffset s_releaseCutoff = new(2023, 11, 30, 23, 59, 59, TimeSpan.Zero);

    //private static readonly Version CurrentVersion = Version.Parse(Variables.Version.Split('-').First());

    /// <summary>
    /// Initialize initial and periodic update checking
    /// </summary>
    internal static async void Initialize()
    {
        // wait a minute in case Windows is busy launching
        await Task.Delay(TimeSpan.FromMinutes(1));

        // initial check
        var (isAvailable, version) = await CheckIsUpdateAvailableAsync();
        if (isAvailable)
            ProcessAvailableUpdate(version);

        // start periodic check
        _ = Task.Run(PeriodicUpdateCheck);
    }

    /// <summary>
    /// Checks for new updates every 30 minutes
    /// </summary>
    private static async void PeriodicUpdateCheck()
    {
        while (!Variables.ShuttingDown)
        {
            await Task.Delay(TimeSpan.FromMinutes(30));

            if (Variables.ShuttingDown)
                return;

            if (!Variables.AppSettings.CheckForUpdates)
                continue;

            var (isAvailable, version) = await CheckIsUpdateAvailableAsync();
            if (!isAvailable)
                continue;

            ProcessAvailableUpdate(version);
        }
    }

    /// <summary>
    /// Called by the automatic updater (on launch or periodically), informs the user of a new update
    /// </summary>
    /// <param name="pendingUpdate"></param>
    private static void ProcessAvailableUpdate(PendingUpdate pendingUpdate)
    {
        // show notification only once
        if (pendingUpdate.Version == Variables.AppSettings.LastUpdateNotificationShown)
            return;

        Variables.AppSettings.LastUpdateNotificationShown = pendingUpdate.Version;
        SettingsManager.StoreAppSettings();

        Variables.MainForm.ShowUpdateInfo(pendingUpdate);
    }

    /// <summary>
    /// Queries the GitHub API to determine whether the latest release tag is greater than this version
    /// </summary>
    /// <returns></returns>
    internal static async Task<(bool isAvailable, PendingUpdate pendingUpdate)> CheckIsUpdateAvailableAsync()
    {
        // beta version handling
        if (Variables.AppSettings.ShowBetaUpdates)
            return await CheckIsBetaUpdateAvailableAsync();

        var pendingUpdate = new PendingUpdate();

        try
        {
            var client = new GitHubClient(new ProductHeaderValue("HASS.Agent"));
            var latestRelease = await client.Repository.Release.GetLatest("hass-agent", "HASS.Agent");

            if (latestRelease == null || latestRelease.Draft || latestRelease.Prerelease)
                return (false, pendingUpdate);

            // we are interested only in releases created after 30.11.2023
            if(latestRelease.CreatedAt.CompareTo(s_releaseCutoff) <= 0)
                return (false, pendingUpdate);

            var isNewer = UpdateIsNewer(Variables.Version, latestRelease.TagName);
            if (!isNewer)
                return (false, pendingUpdate);

            pendingUpdate.Version = latestRelease.TagName;
            pendingUpdate.GitHubRelease = latestRelease;

            return (true, pendingUpdate);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "[UPDATER] Error checking for updates: {err}", ex.Message);
            return (false, pendingUpdate);
        }
    }

    /// <summary>
    /// Queries the GitHub API to determine whether there's a beta update available (or a regular one)
    /// </summary>
    /// <returns></returns>
    private static async Task<(bool isAvailable, PendingUpdate pendingUpdate)> CheckIsBetaUpdateAvailableAsync()
    {
        var pendingUpdate = new PendingUpdate();

        try
        {
            var client = new GitHubClient(new ProductHeaderValue("HASS.Agent"));
            var latestReleases = await client.Repository.Release.GetAll("hass-agent", "HASS.Agent");
            Release latestRelease = null;

            foreach (var release in latestReleases)
            {
                // we are interested only in releases created after 30.11.2023
                if (release.CreatedAt.CompareTo(s_releaseCutoff) <= 0)
                    continue;

                if (release.Draft)
                    return (false, pendingUpdate);

                if (release.Prerelease || release.TagName.Contains('b'))
                    pendingUpdate.IsBeta = true;

                var isNewer = UpdateIsNewer(Variables.Version, release.TagName, true);
                if (!isNewer)
                    return (false, pendingUpdate);

                pendingUpdate.Version = release.TagName;
                latestRelease = release;

                break;
            }

            if (latestRelease == null)
                return (false, pendingUpdate);

            pendingUpdate.GitHubRelease = latestRelease;
            return (true, pendingUpdate);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "[UPDATER] Error checking for beta updates: {err}", ex.Message);
            return (false, pendingUpdate);
        }
    }

    /// <summary>
    /// Returns the release's URL, release notes and installer url for the latest release tag
    /// </summary>
    /// <returns></returns>
    internal static PendingUpdate GetLatestVersionInfo(PendingUpdate pendingUpdate)
    {
        try
        {
            // get the installer
            var installerAssetUrl = string.Empty;
            var installerAsset = pendingUpdate.GitHubRelease.Assets.Select(x => x).FirstOrDefault(y => y.BrowserDownloadUrl.ToLower().EndsWith("installer.exe"));
            if (installerAsset == null)
                Log.Error("[UPDATER] No .installer.exe asset found for release: {v}", pendingUpdate.GitHubRelease.TagName);
            else
                installerAssetUrl = installerAsset.BrowserDownloadUrl;


            pendingUpdate.InstallerUrl = installerAssetUrl;
            pendingUpdate.ReleaseUrl = pendingUpdate.GitHubRelease.HtmlUrl;
            pendingUpdate.ReleaseNotes = pendingUpdate.GitHubRelease.Body;

            return pendingUpdate;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "[UPDATER] Error getting the latest version's info: {err}", ex.Message);

            pendingUpdate.ReleaseNotes = Languages.UpdateManager_GetLatestVersionInfo_Error;
            return pendingUpdate;
        }
    }

    /// <summary>
    /// Downloads the latest installer to a local temp file, and executes it
    /// </summary>
    /// <param name="pendingUpdate"></param>
    /// <param name="owner"></param>
    /// <returns></returns>
    internal static async Task DownloadAndExecuteUpdate(PendingUpdate pendingUpdate, IWin32Window owner)
    {
        var tempFile = await StorageManager.PrepareTempInstallerFilename();
        if (string.IsNullOrEmpty(tempFile))
        {
            MessageBoxAdv.Show(owner, Languages.UpdateManager_DownloadAndExecuteUpdate_MessageBox1, Variables.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            LaunchReleaseUrl(pendingUpdate);
            return;
        }

        var downloaded = await StorageManager.DownloadFileAsync(pendingUpdate.InstallerUrl, tempFile);
        if (!downloaded || !File.Exists(tempFile))
        {
            MessageBoxAdv.Show(owner, Languages.UpdateManager_DownloadAndExecuteUpdate_MessageBox2, Variables.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            LaunchReleaseUrl(pendingUpdate);
            return;
        }

        var certCheck = HelperFunctions.ConfirmCertificate(tempFile);
        if (!certCheck)
        {
            MessageBoxAdv.Show(owner, Languages.UpdateManager_DownloadAndExecuteUpdate_MessageBox3, Variables.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            LaunchReleaseUrl(pendingUpdate);
            return;
        }

        try
        {
            using (_ = Process.Start(new ProcessStartInfo(tempFile) { UseShellExecute = true })) { }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "[UPDATER] Error while launching the installer: {err}", ex.Message);
            MessageBoxAdv.Show(owner, Languages.UpdateManager_DownloadAndExecuteUpdate_MessageBox4, Variables.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            LaunchReleaseUrl(pendingUpdate);
        }
    }

    /// <summary>
    /// Attempts to launch the release's page, if that's not found, the latest release page
    /// </summary>
    /// <param name="pendingUpdate"></param>
    internal static void LaunchReleaseUrl(PendingUpdate pendingUpdate)
    {
        // open main releases page when release url is missing
        if (string.IsNullOrEmpty(pendingUpdate.ReleaseUrl))
        {
            HelperFunctions.LaunchUrl("https://github.com/hass-agent/HASS.Agent/releases/latest");
            return;
        }

        HelperFunctions.LaunchUrl(pendingUpdate.ReleaseUrl);
    }

    /// <summary>
    /// Returns whether the available version is newer than the current
    /// </summary>
    /// <param name="currentVersion"></param>
    /// <param name="availableVersion"></param>
    /// <param name="includeBeta"></param>
    /// <returns></returns>
    private static bool UpdateIsNewer(string currentVersion, string availableVersion, bool includeBeta = false)
    {
        try
        {
            if (!includeBeta && availableVersion.Contains('b'))
                return false;

            var currentVersionIsBeta = currentVersion.Contains('b');
            var availableVersionIsBeta = availableVersion.Contains('b');

            // backwards compatibility
            if (currentVersion.StartsWith('v') || currentVersion.StartsWith('b'))
                currentVersion = currentVersion.Remove(0, 1);
            if (availableVersion.StartsWith('v') || availableVersion.StartsWith('b'))
                availableVersion = availableVersion.Remove(0, 1);

            var versionParsed = Version.TryParse(currentVersion.Split('-').First(), out var currentVersionClean);
            if (!versionParsed)
            {
                Log.Error("[UPDATER] Unable to parse current version tag: {v}", currentVersion);
                return false;
            }

            versionParsed = Version.TryParse(availableVersion.Split('-').First(), out var availableVersionClean);
            if (!versionParsed)
            {
                Log.Error("[UPDATER] Unable to parse available version tag: {v}", currentVersion);
                return false;
            }

            var versionComparison = (VersionComparisonResult)currentVersionClean.CompareTo(availableVersionClean);
            switch (versionComparison)
            {
                // the available version is older, ignore
                case VersionComparisonResult.Older:
                    return false;

                // the available version appears identical
                case VersionComparisonResult.Identical:
                    switch (currentVersionIsBeta)
                    {
                        // are we going from beta to release?
                        case true when !availableVersionIsBeta:
                            // yep! update
                            return true;

                        // are we both beta?
                        case true:
                            // battle of the beta's
                            return AvailableBetaIsNewerThanCurrentBeta(currentVersion, availableVersion);

                        // nothing to do
                        default:
                            return false;
                    }

                case VersionComparisonResult.Newer:
                    // newer, beta or not
                    return true;

                default:
                    // nothing to do
                    return false;
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "[UPDATER] Unable to compare version {a} with {b}: {err}", currentVersion, availableVersion, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Returns whether the available beta version is newer than the current beta version
    /// </summary>
    /// <param name="currentBetaVersion"></param>
    /// <param name="availableBetaVersion"></param>
    /// <returns></returns>
    private static bool AvailableBetaIsNewerThanCurrentBeta(string currentBetaVersion, string availableBetaVersion)
    {
        try
        {
            if (!currentBetaVersion.Contains('b'))
                return false;
            if (!availableBetaVersion.Contains('b'))
                return false;

            var currentBeta = int.Parse(currentBetaVersion.Split('a').Last());
            var availableBeta = int.Parse(availableBetaVersion.Split('a').Last());

            return availableBeta > currentBeta;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "[UPDATER] Unable to compare beta version {a} with {b}: {err}", currentBetaVersion, availableBetaVersion, ex.Message);
            return false;
        }
    }
}
