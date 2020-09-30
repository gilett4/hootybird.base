//@vadym udod

using Coffee.UIExtensions;
using hootybird.audio;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace hootybird.Mechanics._Vfx
{
    public class Vfx : MonoBehaviour
    {
        public string type;
        public float duration = 1f;
        public float uiCopyScale = 100f;
        public UnityEvent onStart;

        [HideInInspector]
        public VfxHolder holder;
        [HideInInspector]
        public bool isActive = false;

        public float durationLeft { get; protected set; }
        public Vfx uiLayerCopy { get; private set; }
        public Vfx copyParent { get; private set; }
        public RectTransform parentRectTransform { get; private set; }

        protected virtual void Update()
        {
            if (isActive && durationLeft != 0f)
            {
                if (durationLeft - Time.deltaTime > 0f)
                    durationLeft -= Time.deltaTime;
                else
                    Disable();
            }
        }

        public void Initialize(VfxHolder holder)
        {
            this.holder = holder;

            gameObject.SetActive(false);
        }

        public virtual Vfx StartVfx()
        {
            if (copyParent)
                transform.SetAsLastSibling();

            isActive = true;
            onStart.Invoke();

            //check if this is part of ui canvas
            parentRectTransform = GetComponentInParent<RectTransform>();

            if (parentRectTransform && !copyParent)
            {
                if (!uiLayerCopy)
                {
                    gameObject.SetActive(false);

                    //get a ui layer copy of this vfx
                    uiLayerCopy = Instantiate(this);
                    uiLayerCopy.copyParent = this;

                    List<ParticleSystem> particleSystems = new List<ParticleSystem>();
                    //add UIParticle
                    uiLayerCopy.GetComponentsInChildren(true, particleSystems);

                    if (particleSystems.Count > 0)
                    {
                        foreach (var p in particleSystems.Where(x => !x.GetComponent<UIParticle>()))
                            p.gameObject.AddComponent<UIParticle>();

                        //set scale to root
                        particleSystems[0].GetComponent<UIParticle>().scale = uiCopyScale;
                    }
                }

                uiLayerCopy.transform.SetParent(transform.parent);
                uiLayerCopy.transform.localScale = Vector3.one;
                uiLayerCopy.transform.localPosition = transform.localPosition;

                uiLayerCopy.StartVfx();
            }
            else
            {
                gameObject.SetActive(true);
                durationLeft = duration;
            }

            return this;
        }

        public virtual Vfx StartVfx(Transform target, Vector3 offset, float rotation)
        {
            transform.SetParent(target);

            transform.localScale = Vector3.one;

            if (target)
            {
                Canvas canvas = target.GetComponentInParent<Canvas>();

                if (canvas)
                    transform.localPosition = offset * canvas.transform.lossyScale.x;
            }
            transform.localPosition = offset;

            transform.localEulerAngles = new Vector3(0f, 0f, rotation);

            return StartVfx();
        }

        public virtual Vfx StartVfx(Transform target, Vector3 offset, float rotation, float customDuration)
        {
            duration = customDuration;

            return StartVfx(target, offset, rotation);
        }

        public virtual void AddDuration(float time)
        {
            durationLeft += time;
        }

        public virtual void RefreshDuration(float time)
        {
            durationLeft = time;
        }

        public virtual void Disable()
        {
            isActive = false;
            gameObject.SetActive(false);

            durationLeft = 0f;

            //if this vfx is controlled by its parent vfx, unactivate parent too
            if (copyParent)
                copyParent.isActive = false;
        }

        public void PlaySfx(string audio, float volume) => AudioHolder.Instance.PlaySelfSfxOneShotTracked(audio, volume);
    }
}