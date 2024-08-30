using RobClient.Game.Entity;
using UnityClientSources;
using UnityPlayer = UnityClientSources.Entities.Player;
using UnityEngine;
using Zenject;

public class GameSceneInstaller : MonoInstaller
{
    public GameObject ObjPrefab;
    public GameObject PlayerPrefab;

    public override void InstallBindings()
    {
        Container.Bind<ObjectViewFactory>().AsSingle();
        Container.BindFactory<WorldObject, GameObjScript, GameObjScript.Factory>().FromComponentInNewPrefab(ObjPrefab);
        Container.BindFactory<UnityPlayer, UnityPlayer.Factory>().FromComponentInNewPrefab(PlayerPrefab);
    }
}
