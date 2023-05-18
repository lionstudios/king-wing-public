using System.IO;
using UnityEditor;

public static class BuildAssetBundles
{
    
    static void BuildAllAssetBundles(BuildTarget buildTarget)
    {
        string assetBundleDirectory = "AssetBundles";
        if(!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, 
            BuildAssetBundleOptions.None, 
            buildTarget);
        EditorUtility.RevealInFinder(assetBundleDirectory);
    }
    
    [MenuItem("LionStudios/Build AssetBundles for Android")]
    static void BuildAllAssetBundlesForAndroid()
    {
        BuildAllAssetBundles(BuildTarget.Android);
    }
    
    [MenuItem("LionStudios/Build AssetBundles for iOS")]
    static void BuildAllAssetBundlesForIOS()
    {
        BuildAllAssetBundles(BuildTarget.iOS);
    }
    
}
