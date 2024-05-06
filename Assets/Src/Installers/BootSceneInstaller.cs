using RobClient;
using RobClient.Network;
using Zenject;

public class BootSceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        var gatewayCommunication = new GatewayCommunication("127.0.0.1", 11111);
        var gameClientFactory = new GameClientFactory();
        var gameClient = gameClientFactory.Create(gatewayCommunication, gatewayCommunication);

        Container.Bind<GatewayCommunication>()
            .FromInstance(gatewayCommunication)
            .AsSingle();

        Container.Bind<GameClient>()
            .FromInstance(gameClient)
            .AsSingle()
            .CopyIntoDirectSubContainers();
    }
}
