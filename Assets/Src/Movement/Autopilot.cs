using System.Collections.Generic;
using Fr.Raven.Proto.Message.Game;
using RobClient.Game.Entity;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace UnityClientSources.Movement {
    public class Autopilot : MonoBehaviour {
        private WorldObject _worldObject;
        private readonly float _speed = 3.5f; 
        private CharacterController _characterController;
        private NavMeshAgent _navMeshAgent;
        private float _nextPositionMismatchCheckTime = 0;
        private Vector3 _lastHandledPosition;
        private TravelPlanExecutor _travelPlanExecutor;

        [Inject]
        public void Construct(WorldObject worldObject)
        {
            _worldObject = worldObject;
            _lastHandledPosition = new Vector3(worldObject.Position.X, worldObject.Position.Y, worldObject.Position.Z);
        }

        private void Start()
        {
            _characterController = gameObject.AddComponent<CharacterController>();
            _navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
            _travelPlanExecutor = new TravelPlanExecutor(_worldObject, _navMeshAgent);
        }

        private void FixedUpdate()
        {
            if (!_worldObject.IsMoving) {
                return;
            }

            _travelPlanExecutor.Execute();

            /*
            float unityOrientationDeg = PositionNormalizer.TransformServerOrientationToUnityOrientation(_worldObject.Position.O) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, unityOrientationDeg, 0);

            transform.rotation = rotation;

            float targetRotation = Mathf.Atan2(_worldObject.Direction.X, _worldObject.Direction.Y) * Mathf.Rad2Deg;
            float unityTargetOrientation = PositionNormalizer.TransformServerOrientationToUnityOrientation(targetRotation);
            //Vector3 targetDirection = Quaternion.Euler(0.0f, unityTargetOrientation, 0.0f) * Vector3.forward;
            Vector3 targetDirection = new Vector3(_worldObject.Direction.X, _worldObject.Direction.Z, _worldObject.Direction.Y);

            _characterController.Move(Time.deltaTime * _speed * targetDirection.normalized);

            Debug.Log($"target orientation from direction {unityTargetOrientation}");
            Debug.Log($"target direction {targetDirection}");

            Debug.Log($"orientation server {_worldObject.Position.O}");
            Debug.Log($"orientation client {PositionNormalizer.TransformServerOrientationToUnityOrientation(_worldObject.Position.O)}");
            Debug.Log($"Normalized orientation {targetDirection.normalized}");
            */

            ResolvePositionMismatch();
        }

        private void ResolvePositionMismatch()
        {
            if (Time.fixedTime < _nextPositionMismatchCheckTime) {
                return;
            }

            _nextPositionMismatchCheckTime = Time.fixedTime + 0.5f; // Every 500ms

            if (!IsPositionDirty()) {
                return;
            }

            Vector3 serverPosition = PositionNormalizer.PositionToUnityVector3(_worldObject.Position);
            float distanceBetweenPositions = Vector3.Distance(gameObject.transform.position, serverPosition);

            if (distanceBetweenPositions > 1) {
                transform.position = serverPosition;
            }

            _lastHandledPosition.x = _worldObject.Position.X;
            _lastHandledPosition.y = _worldObject.Position.Y;
            _lastHandledPosition.z = _worldObject.Position.Z;

            // Debug.Log($"Teleport to {_lastHandledPosition.x};{_lastHandledPosition.y};{_lastHandledPosition.z}");
        }

        private bool IsPositionDirty()
        {
            return _lastHandledPosition.x != _worldObject.Position.X
                || _lastHandledPosition.y != _worldObject.Position.Y
                || _lastHandledPosition.z != _worldObject.Position.Z;
        }
    }
}