using UnityEngine;
using RobClient.Game.Entity;

namespace UnityClientSources
{
    public class ObjectViewFactory
    {
        public GameObject CreateObjectView(WorldObject worldObject, GameObject prefab)
        {
            var position = worldObject.Position;

            return GameObject.Instantiate(prefab, new Vector3(position.X, position.Y, position.Z), Quaternion.Euler(position.Z, 0, 0));
        }
    }
}
