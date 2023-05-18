using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LionStudios.Suite.Creatives
{

    public class LionCreativesSettings : MonoBehaviour
    {
        public static LionCreativesSettings Instance;

        public bool useRemote = true;
        public string localCommandsPath = "Assets/commands.yml";
        public string localAssetBundlesPath = "AssetBundles/";

        private void Awake()
        {
            Instance = this;
        }

        public static bool GetUseRemote()
        {
            if (Instance == null)
            {
                return true;
            }
            return Instance.useRemote;
        }

        public static string GetLocalCommandsPath()
        {
            if (Instance == null)
            {
                return "";
            }
            return Instance.localCommandsPath;
        }

        public static string GetLocalAssetBundlesPath()
        {
            if (Instance == null)
            {
                return "";
            }
            return Instance.localAssetBundlesPath;
        }
    }
}