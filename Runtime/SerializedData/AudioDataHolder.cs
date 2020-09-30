//@vadym udod

using hootybird.Tools;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace hootybird.Serialized
{
    [CreateAssetMenu(fileName = "DefaultSFXHolder", menuName = "Create SFX Data Holder")]
    public class AudioDataHolder : ScriptableObject
    {
        [ReorderableList]
        public List<SerializedAudioData> data;
    }

    [Serializable]
    public class SerializedAudioData
    {
        public string id;
        public bool playInSequence = true;
        [Range(0f, 1f)]
        public float repeatThreshold = .2f;
        [ReorderableList]
        public List<SeriazliedAudioLanguagePair> audios;

        private int lastIndex = 0;

        public AudioClip GetClip(SystemLanguage language, int clipIndex)
        {
            if (audios.Count > 0)
            {
                foreach (SeriazliedAudioLanguagePair seriazliedAudioLanguagePair in audios)
                    if (seriazliedAudioLanguagePair.clips.Length == 0) continue;
                    else
                    {
                        if (seriazliedAudioLanguagePair.language == language)
                        {
                            if (clipIndex == -1)
                            {
                                if (playInSequence)
                                {
                                    if (lastIndex + 1 >= seriazliedAudioLanguagePair.clips.Length) lastIndex = 0;
                                    else lastIndex++;

                                    return seriazliedAudioLanguagePair.clips[lastIndex];
                                }
                                else return seriazliedAudioLanguagePair.clips.Random();
                            }
                            else return seriazliedAudioLanguagePair.clips[clipIndex];
                        }
                    }

                SeriazliedAudioLanguagePair randomOne = audios.Where(data => data.clips.Length > 0).Random();

                if (randomOne != null)
                {
                    if (clipIndex == -1)
                    {
                        if (playInSequence)
                        {
                            if (lastIndex + 1 >= randomOne.clips.Length) lastIndex = 0;
                            else lastIndex++;
                            return randomOne.clips[lastIndex];
                        }
                        else return randomOne.clips.Random();
                    }
                    else return randomOne.clips[clipIndex];
                }
            }

            return null;
        }
    }

    [Serializable]
    public class SeriazliedAudioLanguagePair
    {
        [HideInInspector]
        public string _name;
        [ShowIf("ShowIf")]
        public SystemLanguage language;
        public AudioClip[] clips;

        private bool ShowIf()
        {
            _name = language.ToString();
            return true;
        }
    }
}