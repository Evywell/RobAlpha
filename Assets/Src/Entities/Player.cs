using UnityEngine;
using Zenject;

namespace UnityClientSources.Entities {
    public class Player : MonoBehaviour {

        public class Factory : PlaceholderFactory<Player>, IFactory<Player>
        {
            private readonly DiContainer _container;

            public Factory(DiContainer container)
            {
                _container = container;
            }

            public override Player Create()
            {
                Player player = base.Create();
                _container.InstantiateComponent<SendMovementToServer>(player.gameObject.transform.Find("PlayerArmature").gameObject);

                return player;
            }
        }
    }
}