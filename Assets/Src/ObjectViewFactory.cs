using UnityEngine;
using RobClient.Game.Entity;
using UnityClientSources.Movement;

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
                Quaternion.Euler(
                    0, 
                    PositionNormalizer.TransformServerOrientationToUnityOrientation(position.O) * Mathf.Rad2Deg, 
                    0
                )
            );
        }
    }
}
