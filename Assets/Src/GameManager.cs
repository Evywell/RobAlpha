using System.Collections.Concurrent;
using System.Collections.Generic;
using RobClient;
using RobClient.Game.Entity;
using UnityEngine;
using System;
using UnityClientSources.Movement;
using Zenject;
using UnityClientSources.Events;

namespace UnityClientSources {
    public class GameManager : MonoBehaviour
    {
        public GameClient GameClient
        { get; private set; }

        [SerializeField]
        private GameObject _prefab;

        [SerializeField]
        private GameObject _playerPrefab;

        [SerializeField]
        private GameObject _aoeEffect;

        private ObjectViewFactory _objectViewFactory = new ObjectViewFactory();
        private Dictionary<ulong, WorldObjectMapping> _localObjects = new Dictionary<ulong, WorldObjectMapping>();
        private ConcurrentQueue<WorldObject> _updateObjectQueue = new ConcurrentQueue<WorldObject>();

        private bool _wasMoving = false;
        private MovementInfo _previousMovementInfo;

        private bool _isCurrentPlayerSpawned = false;

        private GameObject _controlledGameObject = null;

        [Inject]
        public void Construct(GameClient gameClient) {
            GameClient = gameClient;
        }

        void Start()
        {
            UIEvents.AssetsLoading?.Invoke();

            GameClient.Game.WorldObjectUpdatedSub.Subscribe(obj => {
                UpdateObject(obj);
            });
        }

        void Update()
        {
            if (_isCurrentPlayerSpawned && Input.GetKeyUp(KeyCode.Q)) {
                CastSpell();
            }

            if (_updateObjectQueue.IsEmpty) {
                return;
            }

            var processedUpdates = 0;

            while (_updateObjectQueue.TryDequeue(out WorldObject worldObject) && processedUpdates < 50) {
                ++processedUpdates;

                if (_localObjects.TryGetValue(worldObject.Guid.GetRawValue(), out WorldObjectMapping localObjectMapping)) {
                    // Update the GameObject
                    // var view = _localObjects[worldObject.Guid.GetRawValue()].GameObject;

                    Debug.Log($"{worldObject.Guid.GetRawValue()} Health = {worldObject.Health}");

                    localObjectMapping.GameObject.transform.position = new Vector3(
                        worldObject.Position.X,
                        worldObject.Position.Y,
                        worldObject.Position.Z
                    );

                    // var position = worldObject.Position;

                    // view.transform.position = new Vector3(position.X, position.Z, position.Y);
                } else {
                    // Create the GameObject
                    var objectView = _objectViewFactory.CreateObjectView(worldObject, _prefab);
                    _localObjects.Add(worldObject.Guid.GetRawValue(), new WorldObjectMapping(worldObject, objectView));
                }
            }
        }

        void FixedUpdate()
        {
            TrySpawnPlayer();

            if (GameClient.Game.ControlledObjectId == null) {
                return;
            }

            GameClient.Game.Update((int)(Time.fixedDeltaTime * 1000));

            // HandlePlayerMovement();
        }

        public void UpdateObject(WorldObject worldObject)
        {
            _updateObjectQueue.Enqueue(worldObject);
        }

        private void CastSpell() 
        {
            if (_controlledGameObject == null) {
                return;
            }

            GameClient.Interaction.CastSpell(1);
            Instantiate(_aoeEffect, _controlledGameObject.transform);
        }

        private void TrySpawnPlayer()
        {
            if (_isCurrentPlayerSpawned) {
                return;
            }

            var playerObjectId = GameClient.Game.ControlledObjectId;

            if (playerObjectId == null) {
                return;
            }

            WorldObjectMapping? playerObjectMapping = null;

            foreach (var localObject in _localObjects) {
                if (localObject.Value.WorldObject.Guid.GetRawValue() == playerObjectId.GetRawValue()) {
                    playerObjectMapping = localObject.Value;
                    
                    break;
                }
            }

            if (playerObjectMapping != null) {
                var playerObject = playerObjectMapping.Value.WorldObject;
                var objectView = _objectViewFactory.CreateObjectView(playerObject, _playerPrefab);
                _localObjects[playerObject.Guid.GetRawValue()] = new WorldObjectMapping(playerObject, objectView);

                Destroy(playerObjectMapping.Value.GameObject);

                _isCurrentPlayerSpawned = true;
                _controlledGameObject = objectView.transform.Find("PlayerArmature").gameObject;
                _controlledGameObject.AddComponent<SendMovementToServer>();
            }
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
