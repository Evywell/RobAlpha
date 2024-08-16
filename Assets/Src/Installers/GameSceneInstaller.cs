using UnityClientSources;
using UnityClientSources.Entities;
using UnityEngine;
using Zenject;

public class GameSceneInstaller : MonoInstaller
{
    public GameObject ObjPrefab;
    public GameObject PlayerPrefab;

    public override void InstallBindings()
    {
        Container.Bind<ObjectViewFactory>().AsSingle();
        Container.BindFactory<GameObjScript, GameObjScript.Factory>().FromComponentInNewPrefab(ObjPrefab);
        Container.BindFactory<Player, Player.Factory>().FromComponentInNewPrefab(PlayerPrefab);
    }
}
