//@vadym udod

using hootybird.Serialized;
using hootybird.Tools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace hootybird.Mechanics._Vfx
{
    public class VfxHolder : MonoBehaviour
    {
        public static VfxHolder instance;

        public VFXDataHolder[] data;

        //[HideInInspector]
        //protected VfxPool[] pools;

        //private Dictionary<string, int> vfxDictionary;
        private Dictionary<string, List<List<Vfx>>> vfxDictionary;
        private Dictionary<string, List<Vfx>> vfxLookup;
        //private List<VfxData> allVfxs;

        protected void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        protected void Start()
        {
            //allVfxs = data.SelectMany(_data => _data.list).ToList();

            //vfxDictionary = new Dictionary<string, int>();
            vfxDictionary = new Dictionary<string, List<List<Vfx>>>();
            vfxLookup = new Dictionary<string, List<Vfx>>();

            foreach (VFXDataHolder dataFile in data)
            {
                foreach (VfxData vfxData in dataFile.list)
                {
                    if (!vfxDictionary.ContainsKey(vfxData.type))
                    {
                        vfxDictionary.Add(vfxData.type, new List<List<Vfx>>());
                        vfxLookup.Add(vfxData.type, new List<Vfx>());
                    }

                    foreach (Vfx option in vfxData.options)
                    {
                        vfxDictionary[vfxData.type].Add(new List<Vfx>());
                        //vfxLookup[vfxData.type].Add()
                    }
                }
            }

            //pools = new VfxPool[totalVfxsSize];

            //for (int )
            //for (int poolIndex = 0; poolIndex < data.data.Count; poolIndex++)
            //{
            //    vfxDictionary.Add(data.data[poolIndex].type, poolIndex);

            //    pools[poolIndex] = new VfxPool(data.data[poolIndex].type, new List<Vfx>[data.data[poolIndex].options.Length]);

            //    for (int optionIndex = 0; optionIndex < data.data[poolIndex].options.Length; optionIndex++)
            //        pools[poolIndex].pool[optionIndex] = new List<Vfx>();
            //}
        }

        public T GetVfx<T>(string type, int optionIndex = - 1) where T : Vfx
        {
            //if (pools.Length == 0) return default(T);

            //VfxPool upperPool = pools[vfxDictionary[type]];

            Vfx result = null;
            //List<Vfx> pool;

            //if (optionIndex == -1)
            //    pool = upperPool.pool[Random.Range(0, upperPool.pool.Length)];
            //else
            //    pool = upperPool.pool[optionIndex];

            //if (!vfxDictionary.ContainsKey(type)) return null;

            //if (optionIndex == -1)
            //{
            //    int optionIndex = 
            //    foreach (Vfx _vfx in vfxDictionary[type].Random())
            //}

            //foreach (Vfx vfx in pool)
            //{
            //    if (!vfx.isActive)
            //    {
            //        result = vfx;
            //        break;
            //    }
            //}

            //if (!result) result = AddVfx(vfxDictionary[type], upperPool.pool.ElementIndex(pool));

            return result as T;
        }

        public int GetRandomPoolIndex(string type)
        {
            //VfxPool upperPool = pools[vfxDictionary[type]];

            //return Random.Range(0, upperPool.pool.Length);
            return 0;
        }

        public Vfx AddVfx(int poolIndex, int decayIndex)
        {
            //Vfx vfx = Instantiate(data.data[poolIndex].options[decayIndex]);
            //vfx.Initialize(this);

            //vfx.transform.SetParent(transform);

            //pools[poolIndex].pool[decayIndex].Add(vfx);

            //return vfx;
            return null;
        }
    }

    public class VfxPool
    {
        public string type;
        public List<Vfx>[] pool;

        public VfxPool(string type, List<Vfx>[] pool)
        {
            this.type = type;
            this.pool = pool;
        }
    }
}