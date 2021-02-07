//@vadym udod

using hootybird.Serialized;
using hootybird.Tools.Localization;
using hootybird.Tween;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace hootybird.audio
{
    public class AudioManager : MonoBehaviour
    {
        private const string PATH = "Audio";

        public static AudioManager Instance
        {
            get
            {
                if (!instance)
                {
                    GameObject go = new GameObject("AudioManager");
                    instance = go.AddComponent<AudioManager>();

                    instance.OrganizeSounds(Resources.LoadAll<AudioDataHolder>(PATH));
                    instance.audioSource = go.AddComponent<AudioSource>();

                    instance.mixer = Resources.Load<AudioMixer>("Master");
                    instance.audioSource.outputAudioMixerGroup = instance.mixer.FindMatchingGroups("Sfx")[0];
                    instance.bgOutputGroup = instance.mixer.FindMatchingGroups("Audio")[0];
                }

                return instance;
            }
        }

        private static AudioManager instance;

        protected AudioMixer mixer = default;
        protected AudioMixerGroup bgOutputGroup = default;

        private AudioSource audioSource;
        private Dictionary<string, SerializedAudioData> byPath = new Dictionary<string, SerializedAudioData>();
        private Dictionary<string, CachedAudioData> currentPlayedPool = new Dictionary<string, CachedAudioData>();
        private Queue<string> toRemove = new Queue<string>();


        public List<BGAudio> currentlyPlayingBG { get; private set; }

        protected void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);

                return;
            }
        }

        protected void Start()
        {
            DontDestroyOnLoad(gameObject);

            audioSource = GetComponent<AudioSource>();
            currentlyPlayingBG = new List<BGAudio>();
        }

        protected void Update()
        {
            if (currentPlayedPool.Count > 0)
            {
                foreach (string clip in currentPlayedPool.Keys.ToList())
                {
                    if (currentPlayedPool[clip].durationLeft - Time.deltaTime > 0f)
                    {
                        currentPlayedPool[clip].durationLeft -= Time.deltaTime;
                    }
                    else
                    {
                        toRemove.Enqueue(clip);
                    }
                }
            }

            while (toRemove.Count > 0)
            {
                currentPlayedPool.Remove(toRemove.Dequeue());
            }
        }

        public static void SetMasterVolume(float value) =>
            Instance?.mixer.SetFloat("MasterVolume", Mathf.Clamp01(1f - value) * -80f);

        public static void SfxVolume(float value) =>
            Instance?.mixer.SetFloat("SfxVolume", Mathf.Clamp01(1f - value) * -80f);

        public static void AudioVolume(float value) =>
            Instance?.mixer.SetFloat("AudioVolume", Mathf.Clamp01(1f - value) * -80f);

        public float PlaySfx(
            string path,
            float volume = 1f,
            SystemLanguage language = SystemLanguage.English,
            int clipIndex = -1) =>
            PlaySfx(path, audioSource, volume, language, clipIndex);

        public float PlaySfx(
            string path,
            AudioSource source,
            float volume = 1f,
            SystemLanguage language = SystemLanguage.English,
            int clipIndex = -1)
        {
            if (!source)
            {
                return 0f;
            }

            path = FixPath(path);

            if (language == SystemLanguage.English)
            {
                language = LocalizationManager.PlayerLanguage;
            }

            SerializedAudioData data = null;
            if (byPath.ContainsKey(path))
            {
                data = byPath[path];
            }

            if (data == null)
            {
                return 0f;
            }

            bool playClip = false;
            bool createNew = false;

            if (currentPlayedPool.ContainsKey(path))
            {
                if (currentPlayedPool[path].progress > data.repeatThreshold)
                {
                    playClip = true;
                    currentPlayedPool[path].durationLeft = currentPlayedPool[path].duration;
                }
            }
            else
            {
                //add to pool
                createNew = true;
                playClip = true;
            }

            if (playClip)
            {
                AudioClip clip = data.GetClip(language, clipIndex);
                if (createNew)
                {
                    currentPlayedPool.Add(
                        path,
                        new CachedAudioData() { duration = clip.length, durationLeft = clip.length });
                }

                source.PlayOneShot(clip, volume);

                return clip.length;
            }

            return 0f;
        }

        public AudioClip GetAudioClip(
            string path,
            SystemLanguage language = SystemLanguage.English,
            int clipIndex = -1)
        {
            path = FixPath(path);
            if (byPath.ContainsKey(path))
            {
                return byPath[path].GetClip(language, clipIndex);
            }

            return null;
        }

        public bool IsBGAudioPlaying(string path) => 
            currentlyPlayingBG.Any(_bgAudio => _bgAudio.path == FixPath(path));

        public BGAudio PlayBGAudio(
            string path,
            bool repeat = false,
            float volume = 1f,
            float fadeInTime = 0f,
            SystemLanguage language = SystemLanguage.English,
            int clipIndex = -1)
        {
            AudioClip audioClip = GetAudioClip(path, language, clipIndex);

            if (audioClip == null)
            {
                return null;
            }

            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.loop = repeat;
            audioSource.outputAudioMixerGroup = bgOutputGroup;

            VolumeTween volumeTween = gameObject.AddComponent<VolumeTween>();
            volumeTween.audioSource = audioSource;
            volumeTween.playbackTime = fadeInTime;
            volumeTween.to = volume;
            volumeTween.curve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

            volumeTween.PlayForward(true);

            BGAudio bgAudio = new BGAudio() { 
                path = FixPath(path), 
                audioSource = audioSource, 
                volumeTween = volumeTween, 
                audioClip = audioClip, };

            audioSource.Play();

            if (!repeat)
            {
                bgAudio.playbackRoutine = StartCoroutine(BGAudioRoutine(bgAudio));
            }

            currentlyPlayingBG.Add(bgAudio);

            return bgAudio;
        }

        public void StopBGAudio(string path, float fadeOutTime)
        {
            BGAudio bgAudio = currentlyPlayingBG.Find(_bgAudio => _bgAudio.path == FixPath(path));

            if (bgAudio != null)
            {
                StopBGAudio(bgAudio, fadeOutTime);
            }
        }

        public void StopBGAudio(BGAudio bgAudio, float fadeOutTime = 0f)
        {
            if (bgAudio == null || !currentlyPlayingBG.Contains(bgAudio) || !this) return;

            bgAudio.volumeTween.playbackTime = fadeOutTime;

            if (bgAudio.playbackRoutine != null)
            {
                StopCoroutine(bgAudio.playbackRoutine);
            }

            StartCoroutine(BGAudioFadeOutRoutine(bgAudio));
        }

        private string FixPath(string oldPath)
        {
            return oldPath.Replace("/", "\"").Replace(",", "\"");
        }

        private void OrganizeSounds(AudioDataHolder[] data)
        {
            foreach (AudioDataHolder dataHolder in data)
            {
                foreach (SerializedAudioData serializedAudioData in dataHolder.data)
                {
                    string path = dataHolder.name + "\"" + serializedAudioData.id;

                    if (!byPath.ContainsKey(path))
                    {
                        byPath.Add(path, serializedAudioData);
                    }
                }
            }
        }

        private IEnumerator BGAudioRoutine(BGAudio bgAudio)
        {
            yield return new WaitForSeconds(bgAudio.audioClip.length - bgAudio.volumeTween.playbackTime);

            StartCoroutine(BGAudioFadeOutRoutine(bgAudio));
        }

        private IEnumerator BGAudioFadeOutRoutine(BGAudio bgAudio)
        {
            bgAudio.volumeTween.PlayBackward(true);

            yield return new WaitForSeconds(bgAudio.volumeTween.playbackTime);

            RemoveBGAudio(bgAudio);
        }

        private void RemoveBGAudio(BGAudio bgAudio)
        {
            if (!currentlyPlayingBG.Contains(bgAudio))
            {
                return;
            }

            Destroy(bgAudio.volumeTween);
            Destroy(bgAudio.audioSource);

            currentlyPlayingBG.Remove(bgAudio);
        }

        public class BGAudio
        {
            public string path;

            public AudioSource audioSource;
            public AudioClip audioClip;

            public Coroutine playbackRoutine;
            public VolumeTween volumeTween;
        }

        private class CachedAudioData
        {
            public float duration;
            public float durationLeft;

            public float progress => (duration - durationLeft) / duration;
        }
    }
}