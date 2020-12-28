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
    [RequireComponent(typeof(AudioSource))]
    public class AudioHolder : MonoBehaviour
    {
        public static AudioHolder Instance
        {
            get
            {
                if (!instance) instance = Instantiate(Resources.Load("AudioHolder") as AudioHolder);
                return instance;
            }

            //private set;
        }

        private static AudioHolder instance;

        [SerializeField]
        protected AudioMixer mixer = default;
        [SerializeField]
        protected AudioMixerGroup bgOutputGroup = default;
        [SerializeField]
        protected AudioDataHolder[] audioData = default;

        private AudioSource audioSource;
        private Dictionary<string, SerializedAudioData> fastAccess = new Dictionary<string, SerializedAudioData>();
        private Dictionary<string, CachedAudioData> currentPlayedPool = new Dictionary<string, CachedAudioData>();
        private Queue<string> toRemove = new Queue<string>();


        public List<BGAudio> currentlyPlayingBG { get; private set; }

        protected void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            //Instance = this;

            DontDestroyOnLoad(gameObject);

            audioSource = GetComponent<AudioSource>();

            currentlyPlayingBG = new List<BGAudio>();

            foreach (AudioDataHolder dataHolder in audioData)
                foreach (SerializedAudioData serializedAudioData in dataHolder.data)
                    if (!fastAccess.ContainsKey(serializedAudioData.id))
                        fastAccess.Add(serializedAudioData.id, serializedAudioData);
        }

        protected void FixedUpdate()
        {
            if (currentPlayedPool.Count > 0)
            {
                foreach (string clip in currentPlayedPool.Keys.ToList())
                    if (currentPlayedPool[clip].durationLeft - Time.deltaTime > 0f) 
                        currentPlayedPool[clip].durationLeft -= Time.fixedDeltaTime;
                    else 
                        toRemove.Enqueue(clip);
            }

            while (toRemove.Count > 0) currentPlayedPool.Remove(toRemove.Dequeue());
        }

        public void SetMasterVolume(float value) => mixer.SetFloat("MasterVolume", Mathf.Clamp01(1f - value) * -80f);

        public void SfxVolume(float value) => mixer.SetFloat("SfxVolume", Mathf.Clamp01(1f - value) * -80f);

        public void AudioVolume(float value) => mixer.SetFloat("AudioVolume", Mathf.Clamp01(1f - value) * -80f);

        public float PlaySfxOneShotTracked(string type, AudioSource source, float volume = 1f, SystemLanguage language = SystemLanguage.English, int clipIndex = -1)
        {
            if (!source) return 0f;

            if (language == SystemLanguage.English) language = LocalizationManager.PlayerLanguage;

            SerializedAudioData data = null;
            if (fastAccess.ContainsKey(type)) data = fastAccess[type];
            if (data == null) return 0f;

            bool playClip = false;
            bool createNew = false;

            if (currentPlayedPool.ContainsKey(type))
            {
                if (currentPlayedPool[type].progress > data.repeatThreshold)
                {
                    playClip = true;
                    currentPlayedPool[type].durationLeft = currentPlayedPool[type].duration;
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
                if (createNew) currentPlayedPool.Add(type, new CachedAudioData() { duration = clip.length, durationLeft = clip.length });
                source.PlayOneShot(clip, volume);

                return clip.length;
            }

            return 0f;
        }

        public float PlaySelfSfxOneShotTracked(string type, float volume = 1f, SystemLanguage language = SystemLanguage.English, int clipIndex = -1) =>
            PlaySfxOneShotTracked(type, audioSource, volume, language, clipIndex);

        /// <summary>
        /// Gets random audio clip from given audio type
        /// </summary>
        /// <param name="type">Audio type</param>
        /// <returns></returns>
        public AudioClip GetAudioClip(string type, SystemLanguage language = SystemLanguage.English, int clipIndex = -1)
        {
            if (fastAccess.ContainsKey(type)) return fastAccess[type].GetClip(language, clipIndex);

            return null;
        }

        public bool IsBGAudioPlaying(string type)
        {
            foreach (BGAudio audio in currentlyPlayingBG)
                if (audio.type == type)
                    return true;

            return false;
        }

        public BGAudio PlayBGAudio(string type, bool repeat = false, float volume = 1f, float fadeInTime = 0f, SystemLanguage language = SystemLanguage.English, int clipIndex = -1)
        {
            AudioClip audioClip = GetAudioClip(type, language, clipIndex);

            if (audioClip == null) return null;

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

            BGAudio bgAudio = new BGAudio() { type = type, audioSource = audioSource, volumeTween = volumeTween, audioClip = audioClip, };

            audioSource.Play();

            if (!repeat)
                bgAudio.playbackRoutine = StartCoroutine(BGAudioRoutine(bgAudio));

            currentlyPlayingBG.Add(bgAudio);

            return bgAudio;
        }

        public void StopBGAudio(BGAudio bgAudio, float fadeOutTime = 0f)
        {
            if (bgAudio == null || !currentlyPlayingBG.Contains(bgAudio) || !this) return;

            bgAudio.volumeTween.playbackTime = fadeOutTime;

            if (bgAudio.playbackRoutine != null) StopCoroutine(bgAudio.playbackRoutine);

            StartCoroutine(BGAudioFadeOutRoutine(bgAudio));
        }

        public void StopBGAudio(string type, float fadeOutTime)
        {
            BGAudio bgAudio = currentlyPlayingBG.Find(_bgAudio => _bgAudio.type == type);

            if (bgAudio != null)
                StopBGAudio(bgAudio, fadeOutTime);
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
                return;

            Destroy(bgAudio.volumeTween);
            Destroy(bgAudio.audioSource);

            currentlyPlayingBG.Remove(bgAudio);
        }

        public class BGAudio
        {
            public string type;

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