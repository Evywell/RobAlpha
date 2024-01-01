using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;

namespace UnityClientSources.UI {
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField]
        private GameManager _gameManger;

        private VisualElement _root;

        private void OnEnable()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;

            Button buttonLogin = _root.Q<Button>("ButtonLogIn");
            Button buttonMove = _root.Q<Button>("ButtonMove");

            buttonLogin.clicked += async () => await LogToServer();
            buttonMove.clicked += () => Move();
        }

        private async Task LogToServer()
        {
            var gc = _gameManger.GameClient;

            await gc.AuthenticateWithUserId(1);
            await gc.Realm.JoinWorldWithCharacter(1);
            _root.style.display = DisplayStyle.None;
        }

        private void Move()
        {
            _gameManger.GameClient.Interaction.Move(30);
        }
    }
}
