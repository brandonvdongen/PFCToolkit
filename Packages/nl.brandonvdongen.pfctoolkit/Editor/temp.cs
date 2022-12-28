using UnityEditor;
using UnityEngine;

public static class temp {


    private static string PackageUrl = "https://raw.githubusercontent.com/brandonvdongen/PFCToolkit/master/Packages/nl.brandonvdongen.pfctoolkit/package.json";
    [MenuItem("PFCTools/Update")]
    private static async void UpdatePFCTools() {

        try {
            AssetDatabase.StartAssetEditing();
            VersionChecker.PackageMeta package = await VersionChecker.GetPackageInfo(PackageUrl);
            Debug.Log($"Downloading: {package.name}, version {package.version}, from: {package.latestUrl}");
            VersionChecker.ImportPackageFromUrl(package.latestUrl);
        }
        finally {
            Debug.Log("Updated PFCToolkit.");
            AssetDatabase.StopAssetEditing();
        }
    }
}
