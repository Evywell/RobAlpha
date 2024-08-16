using System.Collections.Concurrent;
using System.Collections.Generic;
using RobClient;
using RobClient.Game.Entity;
using UnityEngine;
using System;
using Zenject;
using UnityClientSources.Events;

namespace UnityClientSources {
    public class GameManager : MonoBehaviour
    {
        public GameClient GameClient
        { get; private set; }

        [SerializeField]
        private GameObject _aoeEffect;

        private ObjectViewFactory _objectViewFactory;
        private Dictionary<ulong, WorldObjectMapping> _localObjects = new Dictionary<ulong, WorldObjectMapping>();
        private ConcurrentQueue<WorldObject> _updateObjectQueue = new ConcurrentQueue<WorldObject>();

        private bool _isCurrentPlayerSpawned = false;

        private GameObject _controlledGameObject = null;

        [Inject]
        public void Construct(GameClient gameClient, ObjectViewFactory objectViewFactory) {
            GameClient = gameClient;
            _objectViewFactory = objectViewFactory;
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

            if (_isCurrentPlayerSpawned && Input.GetKeyUp(KeyCode.E)) {
                StartAttack();
            }

            if (_controlledGameObject != null) {
                var controlledTransform = _controlledGameObject.transform;
                var rayStartPosition = controlledTransform.position + new Vector3(0, 2, 0);
                var directionVector = new Vector3(controlledTransform.forward.x, 0, controlledTransform.forward.z).normalized;

                Debug.DrawRay(
                    rayStartPosition, 
                    new Vector3(controlledTransform.forward.x, 0, controlledTransform.forward.z).normalized, 
                    Color.red, 
                    0, 
                    false
                );

                Debug.DrawRay(
                    rayStartPosition, 
                    Quaternion.AngleAxis(-60, Vector3.up) * directionVector,
                    Color.blue, 
                    0, 
                    false
                );

                Debug.DrawRay(
                    rayStartPosition, 
                    Quaternion.AngleAxis(60, Vector3.up) * directionVector,
                    Color.blue, 
                    0, 
                    false
                );
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
                    var objectView = _objectViewFactory.CreateBasicGameObjectView(worldObject);
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

        private void StartAttack()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit)) {
                GameObject gameObjectHit = hit.transform.gameObject;

                foreach (var localObject in _localObjects) {
                    if (localObject.Value.GameObject == gameObjectHit) {
                        GameClient.Interaction.EngageCombat(localObject.Value.WorldObject.Guid);
                    }
                }
            }
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
                var objectView = _objectViewFactory.CreatePlayerGameObjectView(playerObject);

                Debug.Log($"Creating player at {objectView.transform.position.x};{objectView.transform.position.z}");
                _localObjects[playerObject.Guid.GetRawValue()] = new WorldObjectMapping(playerObject, objectView);

                Destroy(playerObjectMapping.Value.GameObject);

                _controlledGameObject = objectView.transform.Find("PlayerArmature").gameObject;
                _isCurrentPlayerSpawned = true;
            }
        }
    }
}
