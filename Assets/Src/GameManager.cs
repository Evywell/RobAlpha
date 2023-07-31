using System.Collections.Concurrent;
using System.Collections.Generic;
using RobClient;
using RobClient.Game.Entity;
using RobClient.Network;
using UnityEngine;
using System;

namespace UnityClientSources {
    public class GameManager : MonoBehaviour
    {
        public GameClient GameClient
        { get; private set; }

        [SerializeField]
        private GameObject _prefab;
        private ObjectViewFactory _objectViewFactory = new ObjectViewFactory();
        private Dictionary<ulong, WorldObjectMapping> _localObjects = new Dictionary<ulong, WorldObjectMapping>();
        private ConcurrentQueue<WorldObject> _updateObjectQueue = new ConcurrentQueue<WorldObject>();
        private GatewayCommunication gatewayCommunication;

        void Start()
        {
            gatewayCommunication = new GatewayCommunication("127.0.0.1", 11111);
            var gameClientFactory = new GameClientFactory();
            GameClient = gameClientFactory.Create(gatewayCommunication, gatewayCommunication);


            GameClient.Game.WorldObjectUpdatedSub.Subscribe(obj => {
                UpdateObject(obj);
            });
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

                    view.transform.position = new Vector3(position.X, position.Z, position.Y);
                } else {
                    // Create the GameObject
                    var objectView = _objectViewFactory.CreateObjectView(worldObject, _prefab);
                    _localObjects.Add(worldObject.Guid.GetRawValue(), new WorldObjectMapping(worldObject, objectView));
                }
            }
        }

        void FixedUpdate()
        {
            if (GameClient.Game.ControlledObjectId == null) {
                return;
            }

            GameClient.Game.Update((int)(Time.fixedDeltaTime * 1000));
        }

        async void OnDestroy()
        {
            await gatewayCommunication.Disconnect();
        }

        public void UpdateObject(WorldObject worldObject)
        {
            _updateObjectQueue.Enqueue(worldObject);
        }
    }
}
