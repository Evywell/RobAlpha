using System.Collections.Generic;
using UnityClientSources.Core.UI.Screen;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityClientSources.Core.UI
{
    public abstract class BaseUIManager : MonoBehaviour
    {
        [Tooltip("Required UI Document")]
        [SerializeField] UIDocument m_Document;

        // The currently active UIScreen
        UIScreen m_CurrentScreen;

        // A stack of previously displayed UIScreens
        Stack<UIScreen> m_History = new Stack<UIScreen>();

        // A list of all Views to show/hide
        protected List<UIScreen> m_Screens = new List<UIScreen>();

        public UIScreen CurrentScreen => m_CurrentScreen;
        public UIDocument Document => m_Document;

        protected abstract void RegisterScreens();

        protected abstract void SubscribeToEvents();

        protected abstract void UnsubscribeFromEvents();

        // Register event listeners to game events
        protected virtual void OnEnable()
        {
            SubscribeToEvents();

            // Because non-MonoBehaviours can't run coroutines, the Coroutines helper utility allows us to
            // designate a MonoBehaviour to manage starting/stopping coroutines
            Coroutines.Initialize(this);

            Initialize();
        }

        private void Initialize()
        {
            NullRefChecker.Validate(this);

            RegisterScreens();
            HideScreens();
        }

        // Unregister the listeners to prevent errors
        protected void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        // Clear history and hide all Views
        protected void HideScreens()
        {
            m_History.Clear();

            foreach (UIScreen screen in m_Screens)
            {
                screen.Hide();
            }
        }

        // Finds the first registered UI View of the specified type T
        public T GetScreen<T>() where T : UIScreen
        {
            foreach (var screen in m_Screens)
            {
                if (screen is T typeOfScreen)
                {
                    return typeOfScreen;
                }
            }
            return null;
        }

        // Shows a View of a specific type T, with the option to add it
        // to the history stack
        public void Show<T>(bool keepInHistory = true) where T : UIScreen
        {
            foreach (var screen in m_Screens)
            {
                if (screen is T)
                {
                    Show(screen, keepInHistory);
                    break;
                }
            }
        }

        // 
        public void Show(UIScreen screen, bool keepInHistory = true)
        {
            if (screen == null)
                return;

            if (m_CurrentScreen != null)
            {
                if (!screen.IsTransparent)
                    m_CurrentScreen.Hide();

                if (keepInHistory)
                {
                    m_History.Push(m_CurrentScreen);
                }
            }

            screen.Show();
            m_CurrentScreen = screen;
        }

        // Shows a UIScreen with the keepInHistory always enabled
        public void Show(UIScreen screen)
        {
            Show(screen, true);
        }

        public void ShowFirstScreen(UIScreen screen)
        {
            m_CurrentScreen = screen;

            HideScreens();
            m_History.Push(screen);
            screen.Show();
        }
    }
}