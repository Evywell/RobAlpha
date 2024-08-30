using RobClient.Game.Entity;
using UnityEngine;
using Zenject;

namespace UnityClientSources.Movement {
    public class Autopilot : MonoBehaviour {
        private WorldObject _worldObject;
        private readonly float _speed = 3.5f; 
        private CharacterController _characterController;
        private float _nextPositionMismatchCheckTime = 0;
        private Vector3 _lastHandledPosition;

        [Inject]
        public void Construct(WorldObject worldObject)
        {
            _worldObject = worldObject;
            _lastHandledPosition = new Vector3(worldObject.Position.X, worldObject.Position.Y, worldObject.Position.Z);
        }

        private void Start()
        {
            _characterController = gameObject.AddComponent<CharacterController>();
        }

        private void FixedUpdate()
        {
            if (!_worldObject.IsMoving) {
                return;
            }

            float serverOrientation = PositionNormalizer.TransformServerOrientationToUnityOrientation(_worldObject.Position.O) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, serverOrientation, 0);

            transform.rotation = rotation;

            Vector3 targetDirection = rotation * Vector3.forward;

            _characterController.Move(Time.deltaTime * _speed * targetDirection.normalized);

            Debug.Log($"orientation server {_worldObject.Position.O}");
            Debug.Log($"orientation client {PositionNormalizer.TransformServerOrientationToUnityOrientation(_worldObject.Position.O)}");
            Debug.Log($"Normalized orientation {targetDirection.normalized}");

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
        }

        private bool IsPositionDirty()
        {
            return _lastHandledPosition.x != _worldObject.Position.X
                || _lastHandledPosition.y != _worldObject.Position.Y
                || _lastHandledPosition.z != _worldObject.Position.Z;
        }
    }
}