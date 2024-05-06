using UnityClientSources.Core.UI.Screen;
using UnityClientSources.Events;
using UnityEngine.UIElements;

namespace UnityClientSources.UI.screens
{
    public class MainMenuScreen : UIScreen
    {
        private Button loginBtn;

        public MainMenuScreen(VisualElement parentElement) : base(parentElement)
        {
            SetVisualElements();
            RegisterCallbacks();
        }

        private void SetVisualElements()
        {
            loginBtn = m_RootElement.Q<Button>("ButtonLogIn");
        }

        private void RegisterCallbacks()
        {
            m_EventRegistry.RegisterCallback<ClickEvent>(loginBtn, evt => UIEvents.JoinWorldClicked?.Invoke());
        }
    }
}