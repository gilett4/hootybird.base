//@vadym udod

using hootybird.Serialized;
using hootybird.Tools.Pools;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace hootybird.Mechanics._Vfx
{
    public class VfxHolder : MonoBehaviour
    {
        public static VfxHolder Instance;

        [ReorderableList, SerializeField]
        private List<VFXDataHolder> data = default;

        private Dictionary<string, List<PoolManager>> pools;

        protected void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            //init pools
            pools = new Dictionary<string, List<PoolManager>>();

            foreach (VFXDataHolder dataHolder in data)
                foreach (VfxData vfxData in dataHolder.list)
                {
                    if (!pools.ContainsKey(vfxData.id)) pools.Add(vfxData.id, new List<PoolManager>());

                    foreach (Vfx vfx in vfxData.vfxs) pools[vfxData.id].Add(Pools.AddPool(vfx, transform));
                }
        }

        public T GetVfx<T>(string id, int index = -1) where T : Vfx
        {
            if (pools.Count == 0 || !pools.ContainsKey(id)) return default(T);

            //fix option index
            if (index == -1) index = Random.Range(0, pools[id].Count);
            else index = Mathf.Clamp(index, 0, pools[id].Count - 1);

            return pools[id][index].Pull<PoolItem>() as T;
        }

        public static T Get<T>(string id, int index = -1) where T : Vfx => Instance.GetVfx<T>(id, index);
    }
}