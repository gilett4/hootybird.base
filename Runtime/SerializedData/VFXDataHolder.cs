//@vadym udod

using hootybird.Mechanics._Vfx;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace hootybird.Serialized
{
    [CreateAssetMenu(fileName = "DefaultVFXHolder", menuName = "Create VFX Data Holder")]
    public class VFXDataHolder : ScriptableObject
    {
        [ReorderableList]
        public List<VfxData> list;
    }

    [System.Serializable]
    public class VfxData
    {
        public string id;
        [ReorderableList]
        public List<Vfx> vfxs;
    }
}