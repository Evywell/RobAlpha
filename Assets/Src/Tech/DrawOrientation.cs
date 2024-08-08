using UnityEngine;

public class DrawOrientation : MonoBehaviour
{
    private Transform _localTransform;

    private void Start()
    {
        _localTransform = GetComponent<Transform>();
    }

    private void Update()
    {
        var rayStartPosition = _localTransform.position + new Vector3(0, 2, 0);
        var directionVector = new Vector3(_localTransform.forward.x, 0, _localTransform.forward.z).normalized;

        Debug.DrawRay(
            rayStartPosition, 
            directionVector, 
            Color.red, 
            0, 
            false
        );
    }
}
