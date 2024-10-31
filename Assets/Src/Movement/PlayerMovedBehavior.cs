using UnityClientSources;
using UnityEngine;

public class PlayerMovedBehavior : MonoBehaviour
{
    [SerializeField]
    public GameManager GameManager;

    private Vector3 _currentPosition;

    public void PlayerMoved(Vector3 nextPosition) {
        if (nextPosition == _currentPosition) {
            return;
        }

        _currentPosition.x = nextPosition.x;
        _currentPosition.y = nextPosition.y;
        _currentPosition.z = nextPosition.z;

        GameManager.GameClient.Interaction.MoveClient(
            _currentPosition.x,
            _currentPosition.y,
            _currentPosition.z,
            1.0f,
            new RobClient.Game.Entity.Vector3f(0, 0, 0)
        );

        Debug.Log("Sending movement to server...");
    }
}
