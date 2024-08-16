using UnityEngine;
using Zenject;

public class GameObjScript : MonoBehaviour
{
    public class Factory : PlaceholderFactory<GameObjScript>
    {
    }
}
