//@vadym udod

using hootybird.UI;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace hootybird.Tools
{
    public class GameContentManager : MonoBehaviour
    {
        public static GameContentManager instance;

        public List<PrefabTypePair> typedPrefabs;
        public List<Screen> screens;

        public Dictionary<PrefabType, PrefabTypePair> typedPrefabsFastAccess { get; private set; }

        protected virtual void Awake()
        {
            if (InstanceCheck()) OnInitialized();
        }

        protected virtual bool InstanceCheck()
        {
            if (instance)
            {
                Destroy(gameObject);

                return false;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            return true;
        }

        protected virtual void OnInitialized()
        {
            typedPrefabsFastAccess = new Dictionary<PrefabType, PrefabTypePair>();
            foreach (PrefabTypePair prefabTypePair in typedPrefabs)
                if (!typedPrefabsFastAccess.ContainsKey(prefabTypePair.prefabType))
                    typedPrefabsFastAccess.Add(prefabTypePair.prefabType, prefabTypePair);
        }

        public static GameObject GetPrefab(PrefabType type)
        {
            if (!instance.typedPrefabsFastAccess.ContainsKey(type)) return null;

            return instance.typedPrefabsFastAccess[type].prefab;
        }

        public static T GetPrefab<T>(PrefabType type) => GetPrefab(type).GetComponent<T>();

        public static T InstantiatePrefab<T>(PrefabType type, Transform parent) where T : Component
        {
            if (!instance.typedPrefabsFastAccess.ContainsKey(type)) return null;

            //need this object to have component(T) specified
            if (!instance.typedPrefabsFastAccess[type].prefab.GetComponent<T>()) return null;

            GameObject result = Instantiate(instance.typedPrefabsFastAccess[type].prefab, parent);
            result.transform.localScale = Vector3.one;

            return result.GetComponent<T>();
        }

        [Serializable]
        public class Screen
        {
            [HideInInspector]
            public string _name;

            [ShowIf("Check")]
            public MenuScreen prefab;

            private bool Check()
            {
                _name = prefab ? prefab.name : "No Prefab";

                return true;
            }
        }

        [Serializable]
        public class PrefabTypePair
        {
            [HideInInspector]
            public string _name;
            
            [ShowIf("Check")]
            public PrefabType prefabType;
            public GameObject prefab;

            private bool Check()
            {
                _name = prefabType.ToString();

                return true;
            }
        }

        public enum PrefabType
        {
            NONE = 0,

        }
    }
}