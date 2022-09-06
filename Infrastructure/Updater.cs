using Autodesk.AutoCAD.Runtime;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace Addon.Infrastructure
{
    internal class Updater
    {
        private const string _url = "https://api.github.com/repos/bankoViktor/test-updater/releases/latest";
        private readonly HttpClient _client;

        public Updater()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(Addon.AssemblyTitle, Addon.AssemblyFileVersion));
        }

        public void Update()
        {
            try
            {
                // Request to host
                var json = _client.GetStringAsync(_url).Result;
                if (!string.IsNullOrWhiteSpace(json))
                {
                    var resp = JsonConvert.DeserializeObject<GithubReleaseResponce>(json);
                    var respVersion = Version.Parse(resp.TagName);
                    if (resp.Assets.Length == 1)
                    {
                        if (respVersion > Version.Parse(Addon.AssemblyFileVersion))
                        {
                            // Compose release directory
                            var releasePath = Path.Combine(Addon.AcadRootPath, "Plugins", Addon.AssemblyTitle, respVersion.ToString());

                            // Create release directory
                            EnsureDirectory(releasePath);

                            // Download asset
                            var asset = resp.Assets.FirstOrDefault();
                            DownloadAsset(asset, releasePath);

                            // Update path to new release
                            UpdateRegisterSettings(releasePath);

                            // Notification user
                            Addon.InfoMessage("Restart Autodesk AutoCAD.");
                        }
                        else
                        {
                            Addon.InfoMessage("You using latest version.");
                        }

                        return;
                    }
                }
            }
            catch (System.Exception exc)
            {
                var ed = AcadApp.DocumentManager.MdiActiveDocument.Editor;
                ed.WriteMessage(exc.ToString());
            }

            Addon.ErrorMessage("Update fault.");
        }

        private void DownloadAsset(GithubReleaseResponceAsset asset, string releasePath)
        {
            var assetPath = Path.Combine(releasePath, asset.Name);

            // Download asset
            var bytes = _client.GetByteArrayAsync(asset.DownloadUrl).Result;

            // Write to file
            File.WriteAllBytes(assetPath, bytes);

            // if ZIP then unpack
            if (string.Equals(Path.GetExtension(assetPath), ".zip", StringComparison.InvariantCultureIgnoreCase))
            {
                ZipFile.ExtractToDirectory(assetPath, releasePath);
            }
            File.Delete(assetPath);
        }

        private void EnsureDirectory(string releasePath)
        {
            if (Directory.Exists(releasePath))
            {
                Directory.Delete(releasePath, true);
            }
            Directory.CreateDirectory(releasePath);
        }

        private void UpdateRegisterSettings(string releasePath)
        {
            try
            {
                var path = $@"SOFTWARE\Autodesk\AutoCAD\R24.1\ACAD-5101:409\Applications";
                using (var appsKey = Registry.LocalMachine.OpenSubKey(path, true))
                {
                    var addonKey = appsKey.OpenSubKey(Addon.AssemblyTitle, true);
                    if (addonKey == null)
                    {
                        addonKey = appsKey.CreateSubKey(Addon.AssemblyTitle);
                        addonKey.SetValue("LOADCTRLS", 2, Microsoft.Win32.RegistryValueKind.DWord);
                        addonKey.SetValue("MANAGED", 1, Microsoft.Win32.RegistryValueKind.DWord);
                        addonKey.SetValue("DESCRIPTION", "EDEC Inc. plugin", Microsoft.Win32.RegistryValueKind.String);
                    }

                    var dllPath = Path.Combine(releasePath, "net48", $"{Addon.AssemblyTitle}.dll");
                    addonKey.SetValue("LOADER", dllPath, Microsoft.Win32.RegistryValueKind.String);

                    addonKey.Close();
                    appsKey.Close();
                }
            }
            catch (System.Exception exc)
            {
                throw new System.Exception("Update register fail", exc);
            }
        }
    }
}
