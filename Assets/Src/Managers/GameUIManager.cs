using System.Collections.Generic;
using RobClient;
using UnityClientSources.Core.UI;
using UnityClientSources.Core.UI.Screen;
using UnityClientSources.Events;
using UnityClientSources.UI.screens;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

public class GameUIManager : BaseUIManager
{
    private UIScreen gameScreen;
    private UIScreen loadingScreen;

    private GameClient gameClient;

    [Inject]
    public void Construct(GameClient gameClient) {
        this.gameClient = gameClient;
    }

    protected override void RegisterScreens()
    {
        VisualElement root = Document.rootVisualElement;

        gameScreen = new GameScreen(root.Q<VisualElement>("game__container"));
        loadingScreen = new LoadingScreen(root.Q<VisualElement>("loading__container"));

        m_Screens = new List<UIScreen>
        {
            gameScreen, 
            loadingScreen
        };
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();
        
        ShowFirstScreen(loadingScreen);
    }

    protected override void SubscribeToEvents()
    {
        UIEvents.AssetsReady += UIEvents_OnAssetsReady;
        UIEvents.AssetsLoading += UIEvents_OnAssetsLoading;
    }

    protected override void UnsubscribeFromEvents()
    {
        UIEvents.AssetsReady -= UIEvents_OnAssetsReady;
        UIEvents.AssetsLoading -= UIEvents_OnAssetsLoading;
    }

    public void UIEvents_OnAssetsReady()
    {
        Show(gameScreen);
    }

    public async void UIEvents_OnAssetsLoading()
    {
        await gameClient.AuthenticateWithUserId(1);
        await gameClient.Realm.JoinWorldWithCharacter(1);
        UIEvents.AssetsReady?.Invoke();
    }
}
