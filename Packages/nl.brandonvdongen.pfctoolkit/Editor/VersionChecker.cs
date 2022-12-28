using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public static class VersionChecker {
    internal struct PackageMeta {
        public string name;
        public string displayName;
        public string version;
        public string unity;
        public string description;
        public string latestUrl;
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
        string filePath = Path.GetFullPath("Packages/PackageName");
        File.Delete(filePath);
    }

    internal static void ImportPackageFromUrl(string FileUrl, bool ShowDialog = true) {
        string filePath = Path.GetFullPath("Assets/Temp");
        WebClient webclient = new WebClient();
        webclient.DownloadFile($"{FileUrl}?t={DateTime.Now}", filePath);
        AssetDatabase.ImportPackage(filePath, ShowDialog);
        File.Delete(filePath);
        AssetDatabase.Refresh();
    }
}
