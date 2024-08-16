using System.Reactive.Linq;
using RobClient;
using UnityEngine;
using System;
using UnityClientSources;
using Zenject;

public class DrawOrientation : MonoBehaviour
{
    private GameClient _gameClient;
    private Transform _localTransform;

    private bool _isInFrontOf = false;

    [Inject]
    public void Construct(GameClient gameClient)
    {
        _gameClient = gameClient;
    }

    private void Start()
    {
        var gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _localTransform = GetComponent<Transform>();

        _gameClient.Game.DebugSignalSub
            .Where(signal => signal.Name == "IS_IN_FRONT_OF")
            .Subscribe(signal => {
                _isInFrontOf = signal.Value == 1;
            });
    }

    private void Update()
    {
        var rayStartPosition = _localTransform.position + new Vector3(0, 2, 0);
        var directionVector = new Vector3(_localTransform.forward.x, 0, _localTransform.forward.z).normalized;

        Debug.DrawRay(
            rayStartPosition, 
            directionVector, 
            _isInFrontOf ? Color.green : Color.red, 
            0, 
            false
        );
    }
}
