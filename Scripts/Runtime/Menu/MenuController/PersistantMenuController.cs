﻿//@vadym udod


using hootybird.Tools;

namespace hootybird.UI
{
    public class PersistantMenuController : MenuController
    {
        public const string GAMEPLAY_SCENE_NAME = "gamePlayNew";
        public const string MAIN_MENU_SCENE_NAME = "tabbedUINew";
        public const string LOGO_SCENE_NAME = "logo";

        public static PersistantMenuController instance;

        protected override void Awake()
        {
            if (instance)
            {
                DestroyImmediate(gameObject);

                return;
            }

            DontDestroyOnLoad(gameObject);
            instance = this;
            base.Awake();
        }

        protected override void OnBack()
        {
            if (screensStack.Count == 0) return;

            if (screensStack.Count > 0)
            {
                MenuScreen menuScreen = screensStack.Peek();

                if (!menuScreen.interactable) return;

                menuScreen.OnBack();
            }
        }

        public override void CloseCurrentScreen(bool animate = true)
        {
            if (screensStack.Count > 0)
            {
                screensStack.Pop().Close(animate);

                if (screensStack.Count > 0)
                {
                    MenuScreen menuScreen = screensStack.Peek();

                    SetCurrentScreen(menuScreen);
                    menuScreen.Open();
                }
                else
                {
                    MenuController underlayingMenuController = 
                        GetMenu(GAMEPLAY_MENU_CANVAS_NAME) ?? 
                        GetMenu(MAIN_MENU_CANVAS_NAME);

                    if (underlayingMenuController.screensStack.Count > 0) 
                        underlayingMenuController.screensStack.Peek().Open();
                }
            }
        }
    }
}
