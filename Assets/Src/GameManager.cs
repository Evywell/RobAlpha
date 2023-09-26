using System.Collections.Concurrent;
using System.Collections.Generic;
using RobClient;
using RobClient.Game.Entity;
using RobClient.Network;
using UnityEngine;
using System;
using UnityClientSources.Movement;

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

        private bool _wasMoving = false;
        private MovementInfo _previousMovementInfo;

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

            HandlePlayerMovement();
        }

        async void OnDestroy()
        {
            await gatewayCommunication.Disconnect();
        }

        public void UpdateObject(WorldObject worldObject)
        {
            _updateObjectQueue.Enqueue(worldObject);
        }

        private void HandlePlayerMovement()
        {
            // If pressing movement keys (Z, Q, S, D, Space, ...)
                // Create current movement info object (forward, left, right, jumping, sprinting, ...)
                // If controlled object is already moving AND previous movement info is equal to current movement info
                    // return
                // Move the object
            // Else If controlled object is moving
                // Stop the movement 

            var currentMovementInfo = CreateMovementInfoFromGlobals();
            var isMovementInProgress = currentMovementInfo.IsMovementInProgress();
            var controlledGameObject = GameClient.Game.GetControlledObject();

            if (!isMovementInProgress && _wasMoving) {
                // Stop the movement

                _wasMoving = false;
                _previousMovementInfo = null;
                return;
            }

            if (currentMovementInfo == _previousMovementInfo) {
                return;
            }

            _previousMovementInfo = currentMovementInfo;
            _wasMoving = true;

            GameClient.Interaction.Move(controlledGameObject.Position.O);
        }

        private MovementInfo CreateMovementInfoFromGlobals()
        {
            var movementType = GetMovementTypeFromGlobals();

            return new MovementInfo(movementType, Input.GetButtonDown("Space"));
        }

        private MovementType GetMovementTypeFromGlobals()
        {
            if (Input.GetButtonDown("Z"))
            {
                return MovementType.Forward;
            }

            if (Input.GetButtonDown("S"))
            {
                return MovementType.Backward;
            }
            
            if (Input.GetButtonDown("Q"))
            {
                return MovementType.TurnLeft;
            }

            if (Input.GetButtonDown("D"))
            {
                return MovementType.TurnRight;
            }
            
            return MovementType.None;
        }
    }
}
