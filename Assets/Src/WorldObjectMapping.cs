using RobClient.Game.Entity;
using UnityEngine;

namespace UnityClientSources {
    public struct WorldObjectMapping
    {
        public WorldObject WorldObject
        { get; private set; }

        public GameObject GameObject
        { get; private set; }

        public WorldObjectMapping(WorldObject worldObject, GameObject gameObject)
        {
            WorldObject = worldObject;
            GameObject = gameObject;
        }
    }
}