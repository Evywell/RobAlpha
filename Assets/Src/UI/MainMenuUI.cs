using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Threading.Tasks;
using RobClient;
using RobClient.Network;

namespace UnityClientSources.UI {
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField]
        private GameManager _gameManger;

        private GameClient gc;

        private void OnEnable()
        {
            Debug.Log("OnEnable");
            var communication = new GatewayCommunication("127.0.0.1", 11111);
            var gameClientFactory = new GameClientFactory();
            gc = gameClientFactory.Create(communication, communication);

            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            Button buttonLogin = root.Q<Button>("ButtonLogIn");

            buttonLogin.clicked += async () => await LogToServer();

            gc.Game.WorldObjectUpdatedSub.Subscribe((obj) => {
                _gameManger.UpdateObject(obj);
            });
        }

        private async Task LogToServer()
        {
            Debug.Log("Clicked");

            await gc.AuthenticateWithUserId(1);
            await gc.Realm.JoinWorldWithCharacter(1);

            gc.Interaction.Move(0);

            GetComponent<UIDocument>().rootVisualElement.SetEnabled(false);
        }
    }
}
