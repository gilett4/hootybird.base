//@vadym udod

namespace hootybird.Tools.Pools
{
    public interface IPoolItem
    {
        string id { get; }
        PoolItem Instance { get; }
        IPoolItem OnPushed();
        IPoolItem OnPulled();
    }
}