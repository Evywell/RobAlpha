using System.Collections.Generic;
using UnityClientSources.Core.Scenes;
using UnityClientSources.Core.UI;
using UnityClientSources.Core.UI.Screen;
using UnityClientSources.Events;
using UnityClientSources.UI.screens;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityClientSources.Managers
{
    public class MainMenuUIManager : BaseUIManager
    {
        UIScreen m_HomeScreen;

        UIScreen m_GameScreen;

        protected override void RegisterScreens()
        {
            VisualElement root = Document.rootVisualElement;

            m_HomeScreen = new MainMenuScreen(root.Q<VisualElement>("mainMenu__container"));
            m_GameScreen = new GameScreen(root.Q<VisualElement>("game__container"));

            m_Screens = new List<UIScreen>
            {
                m_HomeScreen,
                m_GameScreen,
            };
        }

        protected override void SubscribeToEvents()
        {
            UIEvents.MainMenuShown += UIEvents_OnMainMenuShown;
            UIEvents.JoinWorldClicked += UIEvents_OnJoinWorldClicked;
        }

        protected override void UnsubscribeFromEvents()
        {
            UIEvents.MainMenuShown -= UIEvents_OnMainMenuShown;
            UIEvents.JoinWorldClicked -= UIEvents_OnJoinWorldClicked;
        }
 

        public void UIEvents_OnMainMenuShown()
        {
            ShowFirstScreen(m_HomeScreen);
        }

        public void UIEvents_OnJoinWorldClicked()
        {
            GameEvents.WorldJoining?.Invoke();
            //GameObject.Find("Main Camera").SetActive(false);
            SceneEvents.SceneIndexLoaded?.Invoke(1);
            CurrentScreen.Hide();
            //Show(m_GameScreen);
        }
    }
}