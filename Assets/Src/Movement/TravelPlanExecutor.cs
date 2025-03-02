using RobClient.Game.Entity;
using UnityEngine;
using UnityEngine.AI;

namespace UnityClientSources.Movement {
    public class TravelPlanExecutor {
        private Vector3f _currentDestination;
        private WorldObject _worldObject;
        private NavMeshAgent _agent;

        public TravelPlanExecutor(WorldObject worldObject, NavMeshAgent agent)
        {
            _worldObject = worldObject;
            _agent = agent;
        }

        public void Execute()
        {
            if (!HasDestinationChanged()) {
                return;
            }

            UpdateDestination();
        }

        private bool HasDestinationChanged()
        {
            if (_worldObject.LastRequestedMovementDestination != null && _currentDestination == null) {
                return true;
            }

            if (_worldObject.LastRequestedMovementDestination == null && _currentDestination != null) {
                return true;
            }

            if (_worldObject.LastRequestedMovementDestination == null && _currentDestination == null) {
                return false;
            }

            return _currentDestination.X != _worldObject.LastRequestedMovementDestination.X
                || _currentDestination.Y != _worldObject.LastRequestedMovementDestination.Y
                || _currentDestination.Z != _worldObject.LastRequestedMovementDestination.Z;
        }

        private void UpdateDestination()
        {
            _currentDestination = _worldObject.LastRequestedMovementDestination;

            if (_currentDestination == null) {
                _agent.ResetPath();

                return;
            }

            SetDestination();
        }

        private void SetDestination()
        {
            _agent.SetDestination(new Vector3(
                _worldObject.LastRequestedMovementDestination.X, 
                _worldObject.LastRequestedMovementDestination.Y, 
                _worldObject.LastRequestedMovementDestination.Z
            ));
        }
    }
}