using UnityClientSources;
using UnityClientSources.Movement;
using UnityEngine;

public class SendMovementToServer : MonoBehaviour
{
    private GameManager _gameManager;

    private Transform _playerTransform;

    private Vector3 _currentPosition;

    private Vector3 _previousPosition;

    private bool _shouldSendPosition = true;

    private float _timeElapsedSinceLastServerUpdate = 0f; // In seconds

    private const float TimeBetweenServerUpdates = 0.2f; // In seconds

    private void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _playerTransform = GetComponent<Transform>();
        _currentPosition = _playerTransform.position;
    }

    private void FixedUpdate()
    {
        _currentPosition = _playerTransform.position;

        if (!_shouldSendPosition) {
            UpdateTimeElapsedTimer();
        }

        if (_currentPosition == _previousPosition) {
            return;
        }

        SendMovementToServerIfNecessary();

        SetShouldSendPositionFlag();

    }

    private void UpdateTimeElapsedTimer()
    {
        if (_timeElapsedSinceLastServerUpdate >= TimeBetweenServerUpdates) {
            return;
        }

        _timeElapsedSinceLastServerUpdate += Time.deltaTime;
    }

    private void SetShouldSendPositionFlag()
    {
        if (_timeElapsedSinceLastServerUpdate < TimeBetweenServerUpdates) {
            return;
        }

        _shouldSendPosition = true;
        _timeElapsedSinceLastServerUpdate = 0;
    }

    private void SendMovementToServerIfNecessary()
    {
        if (!_shouldSendPosition) {
            return;
        }

        _previousPosition.x = _currentPosition.x;
        _previousPosition.y = _currentPosition.y;
        _previousPosition.z = _currentPosition.z;

        _playerTransform.rotation.ToAngleAxis(out float orientationDeg, out _);

        float orientationRad = orientationDeg * Mathf.Deg2Rad;

        float orientation = NormalizeOrientation(PositionNormalizer.TransformUnityOrientationToServerOrientation(orientationRad));

        _gameManager.GameClient.Interaction.MoveClient(
            _currentPosition.x,
            _currentPosition.z,
            _currentPosition.y,
            orientation
        );

        _shouldSendPosition = false;

        Debug.Log($"Sending movement to server... {_currentPosition}");
        Debug.Log($"Orientation rad {orientationRad}");
        Debug.Log($"Orientation sent {orientation}");
    }

    private float NormalizeOrientation(float orientation) {
        return orientation % (2 * Mathf.PI);
    }
}
