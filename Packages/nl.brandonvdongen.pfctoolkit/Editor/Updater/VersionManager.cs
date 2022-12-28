using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace PFCToolkit.VersionManager {
    public static class VersionManager {
        internal struct PackageMeta {
            public string name;
            public string displayName;
            public string version;
            public string unity;
            public string description;
            public string latestUrl;
        }

        internal static PackageMeta CurrentPackage;

        internal static async void UpdatePackage(string packageUrl, bool ShowDialog = true) {
            CurrentPackage = await VersionManager.GetPackageInfo(packageUrl);
            Debug.Log($"Downloading: {CurrentPackage.name}, version {CurrentPackage.version}, from: {CurrentPackage.latestUrl}");
            string filePath = Path.GetFullPath($"Assets/Package.unitypackage");
            string packagePath = Path.GetFullPath($"Packages/{CurrentPackage.name}");
            WebClient webclient = new WebClient();
            webclient.DownloadFile($"{CurrentPackage.latestUrl}?t={DateTime.Now}", filePath);
            if (Directory.Exists(packagePath)) {
                Directory.Delete(packagePath, true);
                File.Delete(packagePath + ".meta");
            } else {
                ShowDialog = true;
                Debug.LogWarning($"{CurrentPackage.name} seems to be installed locally, so we won't Delete anything and Forced Updating is disabled.");
            }
            Debug.Log("Importing updated package...");
            AssetDatabase.importPackageCompleted += UpdateComplete;
            AssetDatabase.importPackageCancelled += UpdateCanceled;
            AssetDatabase.ImportPackage(filePath, ShowDialog);
            File.Delete(filePath);
            AssetDatabase.Refresh();


        }

        private static void UpdateCanceled(string packageName) {
            Debug.LogWarning("Import was Canceled, warning this means that PFCTools may now no longer be present in your project, you'll have to manually reinstall it!");
            AssetDatabase.importPackageCompleted -= UpdateComplete;
            AssetDatabase.importPackageCompleted -= UpdateCanceled;
        }

        private static void UpdateComplete(string packageName) {

            Debug.Log("Finished Updating PFCToolkit.");
            EditorUtility.DisplayDialog($"PFCTools", $"{CurrentPackage.name} has been updated to version {CurrentPackage.version} succesfully!", "Ok");
            AssetDatabase.importPackageCompleted -= UpdateComplete;
            AssetDatabase.importPackageCompleted -= UpdateCanceled;
        }

        internal static async Task<PackageMeta> GetPackageInfo(string packageFileUrl) {
            string response = await HTTPGet(packageFileUrl);
            Debug.Log(response);
            PackageMeta data = (PackageMeta)JsonUtility.FromJson(response, typeof(PackageMeta));
            return data;
        }

        private static async Task<string> HTTPGet(string uri) {
            try {
                return await new WebClient().DownloadStringTaskAsync(uri);
            }
            catch (WebException) {
                return null;
            }
        }

        internal static void DeletePackageByName(string PackageName) {
            string filePath = Path.GetFullPath($"Packages/{PackageName}");
            File.Delete(filePath);
        }
    }
}