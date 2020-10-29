//@vadym udod

using hootybird.Tools.Localization;
using hootybird.UI.Helpers;
using System;
using TMPro;

namespace hootybird.UI
{
    /// <summary>
    /// Promp screen can be used in various places
    /// </summary>
    public class PromptScreen : MenuScreen
    {
        public TMP_Text promptTitle;
        public TMP_Text promptText;

        public ButtonExtended acceptButton;
        public ButtonExtended declineButton;

        protected Action onAccept;
        protected Action onDecline;

        public override void OnBack()
        {
            base.OnBack();

            Decline();
        }

        public virtual void Prompt(string title, string text, Action accept = null, Action decline = null) =>
            Prompt(title, text, LocalizationManager.Value("yes"), LocalizationManager.Value("no"), accept, decline);

        public virtual void Prompt(string title, string text, string yes, string no, Action accept = null, Action decline = null)
        {
            onDecline = decline;
            onAccept = accept;

            UpdateAcceptButton(yes);
            UpdateDeclineButton(no);

            if (promptTitle) promptTitle.text = title;

            if (promptText) promptText.text = text;

            Prompt();
        }

        public virtual void Prompt()
        {
            transform.SetAsLastSibling();
            menuController.OpenScreen(this);
        }

        public virtual void Accept()
        {
            onAccept?.Invoke();
        }

        public virtual void Decline()
        {
            if (onDecline != null)
                onDecline.Invoke();
            else
                menuController.CloseCurrentScreen();
        }

        public void UpdateAcceptButton(string yes)
        {
            if (acceptButton)
            {
                if (string.IsNullOrEmpty(yes))
                    acceptButton.SetActive(false);
                else
                {
                    acceptButton.SetActive(true);
                    acceptButton.SetLabel(yes);
                }
            }
        }

        public void UpdateDeclineButton(string no)
        {
            if (declineButton)
            {
                if (string.IsNullOrEmpty(no))
                    declineButton.SetActive(false);
                else
                {
                    declineButton.SetActive(true);
                    declineButton.SetLabel(no);
                }
            }
        }
    }
}
