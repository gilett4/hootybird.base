//@vadym udod

using UnityEngine;

namespace hootybird.Tools.Pools
{
    public class PoolManager
    {
        internal IPoolItem item;
        internal Transform instancesParent;

        private StackModified<PoolItem> stack = new StackModified<PoolItem>();

        public PoolManager(IPoolItem item, Transform instancesParent)
        {
            this.item = item;
            this.instancesParent = instancesParent;
        }

        public T Pull<T>() where T : PoolItem
        {
            if (stack.Count > 0) return (T)stack.Pop().OnPulled();

            return Add<T>();
        }

        public T Add<T>() where T : PoolItem => (T)Object.Instantiate(item.Instance, instancesParent).OnPulled();

        public void Push(IPoolItem item)
        {
            if (this.item == item) return;

            item.OnPushed();
            stack.Push(item.Instance);
        }

        public void Remove(PoolItem item) => stack.Remove(item);

        public bool OfItem(string id) => item.id == id;

        public bool OfRoot(IPoolItem item) => this.item == item;
    }
}