//@vadym udod

using System.Collections.Generic;
using UnityEngine;

namespace hootybird.Tools.Pools
{
    public static class Pools
    {
        internal static Dictionary<string, PoolManager> pools = new Dictionary<string, PoolManager>();

        public static PoolManager AddPool(IPoolItem item, Transform instancesParent)
        {
            PoolManager poolManager = new PoolManager(item, instancesParent);
            pools.Add(item.id, poolManager);

            return poolManager;
        }

        public static T Pull<T>(IPoolItem item) where T : PoolItem => Pull<T>(item.id);

        public static T Pull<T>(string id) where T : PoolItem
        {
            if (!pools.ContainsKey(id)) return default;

            return pools[id].Pull<T>();
        }

        public static void Push(IPoolItem item)
        {
            if (!pools.ContainsKey(item.id)) return;

            pools[item.id].Push(item);
        }

        public static void Remove(IPoolItem item)
        {
            if (!pools.ContainsKey(item.id)) return;

            pools[item.id].Remove(item.Instance);
        }

        public static PoolManager GetPool(IPoolItem item) => GetPool(item.id);

        public static PoolManager GetPool(string id) => (pools.ContainsKey(id) ? pools[id] : null);
    }
}