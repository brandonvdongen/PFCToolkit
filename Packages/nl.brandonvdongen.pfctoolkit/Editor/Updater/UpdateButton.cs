using UnityEditor;

namespace PFCToolkit.VersionManager {
    public static class UpdateButton {

        private static string PackageUrl = "https://raw.githubusercontent.com/brandonvdongen/PFCToolkit/master/Packages/nl.brandonvdongen.pfctoolkit/package.json";
        [MenuItem("PFCToolkit/Update PFCToolkit")]
        private static void UpdatePFCTools() {
            VersionManager.UpdatePackage(PackageUrl, false);
        }
    }
}