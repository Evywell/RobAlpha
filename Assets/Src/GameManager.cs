using System.Collections.Concurrent;
using System.Collections.Generic;
using RobClient.Game.Entity;
using UnityEngine;

namespace UnityClientSources {
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _prefab;
        private ObjectViewFactory _objectViewFactory = new ObjectViewFactory();
        private Dictionary<ulong, WorldObjectMapping> _localObjects = new Dictionary<ulong, WorldObjectMapping>();
        private ConcurrentQueue<WorldObject> _updateObjectQueue = new ConcurrentQueue<WorldObject>();

        void Start()
        {
        }

        void Update()
        {
            if (_updateObjectQueue.IsEmpty) {
                return;
            }

            var processedUpdates = 0;

            while (_updateObjectQueue.TryDequeue(out WorldObject worldObject) && processedUpdates < 50) {
                ++processedUpdates;

                if (_localObjects.ContainsKey(worldObject.Guid.GetRawValue())) {
                    // Update the GameObject
                    var view = _localObjects[worldObject.Guid.GetRawValue()].GameObject;
                    var position = worldObject.Position;

                    view.transform.position = new Vector3(position.X, position.Y, position.Z);
                } else {
                    // Create the GameObject
                    var objectView = _objectViewFactory.CreateObjectView(worldObject, _prefab);
                    _localObjects.Add(worldObject.Guid.GetRawValue(), new WorldObjectMapping(worldObject, objectView));
                }
            }
        }

        public void UpdateObject(WorldObject worldObject)
        {
            _updateObjectQueue.Enqueue(worldObject);
        }
    }
}
