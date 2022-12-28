using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public static class VersionManager {
    internal struct PackageMeta {
        public string name;
        public string displayName;
        public string version;
        public string unity;
        public string description;
        public string latestUrl;
    }

    internal static async void UpdatePackage(string packageUrl, bool ShowDialog) {
        VersionManager.PackageMeta package = await VersionManager.GetPackageInfo(packageUrl);
        try {
            Debug.Log($"Downloading: {package.name}, version {package.version}, from: {package.latestUrl}");
            string filePath = Path.GetFullPath($"Assets/Package.unitypackage");
            string packagePath = Path.GetFullPath($"Packages/{package.name}");
            WebClient webclient = new WebClient();
            webclient.DownloadFile($"{package.latestUrl}?t={DateTime.Now}", filePath);
            Debug.Log("Attempting to remove old package...");
            Directory.Delete(packagePath, true);
            File.Delete(packagePath + ".meta");
            UnityEditor.PackageManager.Client.Remove(package.name);
            AssetDatabase.Refresh();
            Debug.Log("Importing updated package...");
            AssetDatabase.ImportPackage(filePath, ShowDialog);
            File.Delete(filePath);

        }
        finally {
            Debug.Log("Finished Updating PFCToolkit.");
            EditorUtility.DisplayDialog("Update Complete", $"PFCTools has succesfully been updated to v{package.version}", "close");
        }
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
