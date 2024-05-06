using UnityClientSources.Core.UI.Screen;
using UnityEngine.UIElements;

namespace UnityClientSources.UI.screens
{
    public class GameScreen : UIScreen
    {
        public GameScreen(VisualElement parentElement) : base(parentElement)
        {
            SetVisualElements();
            RegisterCallbacks();
        }

        private void SetVisualElements()
        {
        }

        private void RegisterCallbacks()
        {
        }
    }
}