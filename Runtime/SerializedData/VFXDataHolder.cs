//@vadym udod

using hootybird.Mechanics._Vfx;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace hootybird.Serialized
{
    [CreateAssetMenu(fileName = "DefaultVFXHolder", menuName = "Create VFX Data Holder")]
    public class VFXDataHolder : ScriptableObject
    {
        public List<VfxData> list;
    }

    [Serializable]
    public class VfxData
    {
        public string type;
        public Vfx[] options;
    }
}