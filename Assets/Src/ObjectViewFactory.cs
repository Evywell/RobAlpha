using UnityEngine;
using RobClient.Game.Entity;
using UnityClientSources.Movement;
using UnityPlayer = UnityClientSources.Entities.Player;

namespace UnityClientSources
{
    public class ObjectViewFactory
    {
        private readonly GameObjScript.Factory _basicGameObjectfactory;
        private readonly UnityPlayer.Factory _playerFactory;

        public ObjectViewFactory(GameObjScript.Factory basicGameObjectfactory, UnityPlayer.Factory playerFactory)
        {
            _basicGameObjectfactory = basicGameObjectfactory;
            _playerFactory = playerFactory;
        }

        public GameObject CreatePlayerGameObjectView(WorldObject worldObject)
        {
            return CreateObjectView(worldObject, _playerFactory.Create().gameObject);
        }

        public GameObject CreateBasicGameObjectView(WorldObject worldObject)
        {
            return CreateObjectView(worldObject, _basicGameObjectfactory.Create(worldObject).gameObject);
        }

        private GameObject CreateObjectView(WorldObject worldObject, GameObject gameObject)
        {
            var position = worldObject.Position;

            gameObject.transform.position = new Vector3(position.X, position.Z, position.Y);
            gameObject.transform.rotation = Quaternion.Euler(
                0, 
                PositionNormalizer.TransformServerOrientationToUnityOrientation(position.O) * Mathf.Rad2Deg, 
                0
            );

            return gameObject;
        }
    }
}
