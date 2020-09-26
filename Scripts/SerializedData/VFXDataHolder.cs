//@vadym udod

using hootybird.Mechanics._Vfx;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace hootybird.Serialized
{
    [CreateAssetMenu(fileName = "DefaultVFXHolder", menuName = "Create VFX Data Holder")]
    public class VFXDataHolder : ScriptableObject
    {
        public List<VfxNamePair> data;
    }

    [Serializable]
    public class VfxNamePair
    {
        [HideInInspector]
        public string _name;

        [ShowIf("Check")]
        public VfxType type;
        public Vfx[] options;

        public bool Check()
        {
            _name = type.ToString();

            return true;
        }
    }

    public class VfxPool
    {
        public VfxType type;
        public List<Vfx>[] pool;
    }
}