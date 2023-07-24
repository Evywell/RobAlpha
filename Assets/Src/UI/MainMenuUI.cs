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

        private void OnEnable()
        {
            Debug.Log("OnEnable");

            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            Button buttonLogin = root.Q<Button>("ButtonLogIn");
            Button buttonMove = root.Q<Button>("ButtonMove");

            buttonLogin.clicked += async () => await LogToServer();
            buttonMove.clicked += () => Move();
        }

        private async Task LogToServer()
        {
            Debug.Log("Clicked");
            var gc = _gameManger.GameClient;

            await gc.AuthenticateWithUserId(1);
            await gc.Realm.JoinWorldWithCharacter(1);
        }

        private void Move()
        {
            _gameManger.GameClient.Interaction.Move(30);
        }
    }
}
