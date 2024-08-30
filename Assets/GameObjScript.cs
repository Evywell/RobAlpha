using RobClient.Game.Entity;
using UnityClientSources.Movement;
using UnityEngine;
using Zenject;

public class GameObjScript : MonoBehaviour
{
    private WorldObject _worldObject;

    [Inject]
    public void Construct(WorldObject worldObject)
    {
        _worldObject = worldObject;
    }

    public class Factory : PlaceholderFactory<WorldObject, GameObjScript>
    {
        private readonly DiContainer _container;

        public Factory(DiContainer container)
        {
            _container = container;
        }

        public override GameObjScript Create(WorldObject worldObject)
        {
            GameObjScript basicGameObject = base.Create(worldObject);
            _container.InstantiateComponent<Autopilot>(basicGameObject.gameObject, new object[]{ worldObject });

            return basicGameObject;
        }
    }
}
