using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LionStudios.Suite.Creatives
{

    public static partial class LionDebug
    {
        private const string BUNDLES_BASE_URL = "https://lion-unity-asset-bundles.s3.us-west-2.amazonaws.com/"; 

        private static void LoadAssetBundle(string bundleName, string outKey)
        {
            bool useRemoteBundle = LionCreativesSettings.GetUseRemote();
            if(useRemoteBundle)
            {
                string applicationPath = Application.identifier.Replace('.', '/');
                string url = BUNDLES_BASE_URL + applicationPath + "/" + bundleName;
                var request = UnityEngine.Networking.UnityWebRequestAssetBundle.GetAssetBundle(url, 0);
                request.SendWebRequest();
                while (!request.isDone) { }
                AssetBundle loadedAssetBundle = UnityEngine.Networking.DownloadHandlerAssetBundle.GetContent(request);
                Store(outKey, loadedAssetBundle);
            }
            else
            {
                string path = LionCreativesSettings.GetLocalAssetBundlesPath() + bundleName;
                AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
                Store(outKey, assetBundle);
            }
        }

        private static void LoadAsset(string bundleKey, string assetName, string outKey)
        {
            AssetBundle assetBundle = GetStored(bundleKey) as AssetBundle;
            object asset = assetBundle.LoadAsset(assetName);
            Store(outKey, asset);
        }

        private static void LoadAsset(string bundleKey, string assetName, string type, string outKey)
        {
            AssetBundle assetBundle = GetStored(bundleKey) as AssetBundle;
            object asset = assetBundle.LoadAsset(assetName, GetType(type));
            Store(outKey, asset);
        }

        private static void LoadAndApplyMesh(string bundleKey, string meshName, string gameObjectPath)
        {
            AssetBundle assetBundle = GetStored(bundleKey) as AssetBundle;
            Mesh mesh = assetBundle.LoadAsset(meshName, typeof(Mesh)) as Mesh;
            if(mesh == null)
            {
                Debug.LogError($"Mesh: {meshName} not found");
            }
            GameObject obj = GameObject.Find(gameObjectPath);
            if(obj == null)
            {
                Debug.LogError($"No gameobject found at path: {gameObjectPath}");
            }
            SkinnedMeshRenderer skinnedMeshRenderer = obj.GetComponent<SkinnedMeshRenderer>();
            if(skinnedMeshRenderer != null)
            {
                skinnedMeshRenderer.sharedMesh = mesh;
            }
            else
            {
                MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
                if (meshFilter == null)
                {
                    Debug.LogWarning($"No MeshRenderer component attached to gameobject at {gameObjectPath}. Adding one manually.");
                    meshFilter = obj.AddComponent<MeshFilter>();
                }
                meshFilter.mesh = mesh;
            }
        }

        [RuntimeInitializeOnLoadMethod]
        public static void AddAssetBundleCommands()
        {

            AddCommand<string, string>("loadassetbundle", "Loads and stores Asset Bundle", LoadAssetBundle);
            AddCommand<string, string, string>("loadasset", "Loads an Asset from an Asset Bundle", LoadAsset);
            AddCommand<string, string, string, string>("loadasset", "Loads an Asset from an Asset Bundle", LoadAsset);
            AddCommand<string, string, string>("loadandapplymesh", "Loads a mesh from an asset bundle, and applies it to a game object", LoadAndApplyMesh);

        }

    }
    
}
