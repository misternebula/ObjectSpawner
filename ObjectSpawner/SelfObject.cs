using System.Linq;
using UnityEngine;
using OWML.ModHelper.Events;

namespace ObjectSpawner
{
    class SelfObject : ObjectBase
    {
        public SelfObject(string a, string b) : base(a, b)
        {
        }

        public override GameObject Load()
        {
            var obj = GameObject.Instantiate(Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(g => g.name == GameObjectString));

            foreach (var item in obj.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                item.enabled = true;
            }

            foreach (var item in obj.GetComponentsInChildren<Animator>())
            {
                item.enabled = true;
            }


            return obj;
        }
    }
}
