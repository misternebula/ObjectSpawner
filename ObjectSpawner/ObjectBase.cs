using System.Linq;
using UnityEngine;

namespace ObjectSpawner
{
    public class ObjectBase
    {
        protected string GameObjectString { get; set; }
        protected string AssetBundleName { get; set; }

        public ObjectBase(string gameObjectString, string assetBundleName)
        {
            GameObjectString = gameObjectString;
            AssetBundleName = assetBundleName;
        }

        public virtual GameObject Load()
        {
            var obj = GameObject.Instantiate(Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(g => g.name == GameObjectString));
            if (AssetBundleName != "")
            {
                MainClass.helper.Console.WriteLine("Loading assetbundle " + AssetBundleName);
                StreamingManager.LoadStreamingAssets(AssetBundleName);
            }

            foreach (var item in obj.GetComponentsInChildren<StreamingMeshHandle>())
            {
                item.enabled = true;
            }

            foreach (var item in obj.GetComponentsInChildren<MeshRenderer>())
            {
                item.enabled = true;
            }

            return obj;
        }
    }
}
