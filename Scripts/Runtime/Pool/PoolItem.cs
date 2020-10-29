//@vadym udod

using System;
using UnityEngine;

namespace hootybird.Tools.Pools
{
    public class PoolItem : MonoBehaviour, IPoolItem
    {
        [SerializeField]
        protected string ID;

        public string id => ID;

        public PoolItem Instance => this;

        public PoolManager Pool => Pools.GetPool(id);

        protected virtual void OnDestroy() => Pools.Remove(this);

        public T GetInstance<T>() where T : PoolItem => Pools.Pull<T>(this);

        public virtual IPoolItem OnPulled() => this;

        public virtual IPoolItem OnPushed() => this;

        public void PutBack() => Pools.Push(this);
    }
}