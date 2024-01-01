using UnityEngine;
using RobClient.Game.Entity;

namespace UnityClientSources
{
    public class ObjectViewFactory
    {
        public GameObject CreateObjectView(WorldObject worldObject, GameObject prefab)
        {
            var position = worldObject.Position;

            return GameObject.Instantiate(
                prefab,
                new Vector3(position.X, position.Z, position.Y),
                Quaternion.Euler(position.O, 0, 0)
            );
        }
    }
}
