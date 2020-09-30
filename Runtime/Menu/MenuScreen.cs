//@vadym udod

using hootybird.audio;
using hootybird.Tools;
using hootybird.Tween;
using hootybird.UI.Widgets;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace hootybird.UI.Menu
{
    /// <summary>
    /// Represents each separate screen in a menu
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class MenuScreen : RoutinesBase
    {
        public bool closePreviousWhenOpened = true;

        [HideInInspector]
        public MenuController menuController;

        [SerializeField]
        protected UnityEvent onClose;
        [SerializeField]
        protected UnityEvent onOpen;
        [SerializeField]
        protected UnityEvent onBack;

        [SerializeField, Tooltip("What audio to play when onBack invoked")]
        protected string onBackSfx = "menu_back";

        [SerializeField]
        protected bool defaultScreen = false;

        protected CanvasGroup canvasGroup;
        protected TweenBase tween;
        protected List<WidgetBase> widgets;

        public bool initialized { get; protected set; }
        public bool isOpened { get; private set; }
        public RectTransform rectTransform { get; protected set; }
        public LayoutElement layoutElement { get; protected set; }
        public bool inputBlocked => canvasGroup.blocksRaycasts && !canvasGroup.interactable;
        public bool interactable => canvasGroup ? canvasGroup.interactable : gameObject.activeInHierarchy;
        public virtual bool isCurrent => menuController ? menuController.currentScreen == this : false;

        protected override void Awake()
        {
            base.Awake();

            menuController = GetComponentInParent<MenuController>();

            canvasGroup = GetComponent<CanvasGroup>();
            tween = GetComponent<TweenBase>();
            rectTransform = GetComponent<RectTransform>();
            layoutElement = GetComponent<LayoutElement>();

            widgets = new List<WidgetBase>(GetComponentsInChildren<WidgetBase>()).Where(widget => widget.GetComponentInParent<MenuScreen>() == this).ToList();
        }

        protected virtual void Start()
        {
            if (defaultScreen)
            {
                menuController.SetCurrentScreen(this);
                menuController.screensStack.Push(this);

                isOpened = true;
            }

            OnInitialized();

            initialized = true;
        }

        public virtual void Open()
        {
            UpdateWidgets();

            onOpen.Invoke();

            if (isOpened) return;

            isOpened = true;

            if (tween) tween.PlayForward(true);

            SetInteractable(true);
            SetBlockrays(true);
        }

        /// <summary>
        /// Close screen
        /// </summary>
        public virtual void Close(bool animate = true)
        {
            isOpened = false;

            if (tween)
            {
                if (animate)
                    tween.PlayBackward(true);
                else
                    tween.Progress(0f, PlaybackDirection.FORWARD);
            }

            SetInteractable(false);
            SetBlockrays(false);

            onClose.Invoke();
        }

        public void CloseSelf(bool animate = true)
        {
            if (menuController)
            {
                if (isCurrent) menuController.CloseCurrentScreen(animate);
                else
                {
                    Close(animate);
                    if (menuController.screensStack.Contains(this)) menuController.screensStack.Remove(this);
                }
            }
            else
                Close(animate);
        }

        public virtual void ExecuteMenuEvent(MenuEvents menuEvent) { }

        public virtual bool IsWidgetVisible(WidgetBase widget)
        {
            if (widget.gameObject.activeInHierarchy && widget.canvasGroup && widget.canvasGroup.alpha > 0f) return true;

            return false;
        }

        public virtual void BlockInput()
        {
            SetInteractable(false);
            canvasGroup.blocksRaycasts = true;
        }

        public virtual void BlockInput(float time)
        {
            BlockInput();

            StartRoutine("blockInput", time, () => SetInteractable(true), null);
        }

        public virtual void OnBack()
        {
            onBack.Invoke();

            if (AudioHolder.Instance) AudioHolder.Instance.PlaySelfSfxOneShotTracked(onBackSfx);
        }

        public virtual void BackToRoot() => menuController.BackToRoot();

        public void SetInteractable(bool state)
        {
            if (canvasGroup) canvasGroup.interactable = state;
        }

        public void SetBlockrays(bool state)
        {
            if (canvasGroup) canvasGroup.blocksRaycasts = state;
        }

        public void UpdateWidgets() => widgets.ForEach(widget => widget._Update());

        public virtual List<T> GetWidgets<T>() => widgets.Where(widget => widget.GetType() == typeof(T) || widget.GetType().IsSubclassOf(typeof(T))).Cast<T>().ToList();

        protected virtual void OnInitialized() { }
    }
}