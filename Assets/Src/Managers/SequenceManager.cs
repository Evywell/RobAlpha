using UnityEngine;
using UnityClientSources.Core;
using UnityClientSources.Events;
using UnityClientSources.Core.Scenes;
using Zenject;
using RobClient.Network;
using RobClient;

namespace UnityClientSources
{
    /// <summary>
    /// A SequenceManager controls the overall flow of the application using a state machine.
    /// 
    /// Use this class to define how each State will transition to the next. Each state can
    /// transition to the next state when receiving an event or reaching a specific condition.
    ///
    /// Note: this class currently is only used for demonstration/diagnostic purposes. You can use
    ///       the start and end of each state to instantiate GameObjects/play effects. Another simple
    ///       state machine for UI screens (UIManager) actually drives most of the quiz gameplay.
    /// 
    /// </summary>

    public class SequenceManager : MonoBehaviour
    {
        // Inspector fields
        [Header("Preload")]
        [Tooltip("Prefab assets that load first. These can include level management Prefabs or textures, sounds, etc.")]
        [SerializeField] GameObject[] m_PreloadedAssets;

        [Space(10)]
        [Tooltip("Debug state changes in the console")]
        [SerializeField] bool m_Debug;

        StateMachine m_StateMachine = new StateMachine();

        // Define all States here
        IState m_MainMenuState;         // Show the main menu screens
        IState m_GameLoadingState;
        IState m_GamePlayState;         // Play the game

        public GameClient GameClient
        { get; private set; }

        // Access to the StateMachine's CurrentState
        public IState CurrentState => m_StateMachine.CurrentState;

        private GatewayCommunication _gatewayCommunication;

        [Inject]
        public void Construct(GatewayCommunication gatewayCommunication, GameClient gameClient)
        {
            _gatewayCommunication = gatewayCommunication;
            GameClient = gameClient;
        }

        async private void OnDestroy()
        {
            await _gatewayCommunication.Disconnect();
        }

        #region MonoBehaviour event messages
        private void Start()
        {
            // Set this MonoBehaviour to control the coroutines - unused in this demo
            Coroutines.Initialize(this);

            // Checks for required fields in the Inspector
            NullRefChecker.Validate(this);

            // Instantiates any assets needed before gameplay
            InstantiatePreloadedAssets();

            // Sets up States and transitions, runs initial State
            Initialize();
        }

        // Subscribe to event channels
        private void OnEnable()
        {
            SceneEvents.ExitApplication += SceneEvents_ExitApplication;
        }

        // Unsubscribe from event channels to prevent errors
        private void OnDisable()
        {
            SceneEvents.ExitApplication -= SceneEvents_ExitApplication;
        }
        #endregion

        #region Methods

        public void Initialize()
        {
            // Define the Game States
            SetStates();
            AddLinks();

            // Run first state
            m_StateMachine.Run(m_MainMenuState);
            UIEvents.MainMenuShown?.Invoke();
        }

        // Define the state machine's states
        private void SetStates()
        {
            // Create States for the game. Pass in an Action to execute or null to do nothing

            // Optional names added for debugging

            m_MainMenuState = new State(null, "MainMenuState", m_Debug);
            m_GameLoadingState = new State(null, "GameLoadingState", m_Debug);
            m_GamePlayState = new State(null, "GamePlayState", m_Debug);
        }


        // Define links between the states
        private void AddLinks()
        {
            // EventLinks listen for the UI/game event messages to activate the transition to the next state
            ActionWrapper worldJoiningWrapper = new()
            {
                Subscribe = handler => GameEvents.WorldJoining += handler,
                Unsubscribe = handler => GameEvents.WorldJoining -= handler,
            };

            ActionWrapper worldEnteredWrapper = new()
            {
                Subscribe = handler => GameEvents.WorldEntered += handler,
                Unsubscribe = handler => GameEvents.WorldEntered -= handler
            };

            // Once you have wrappers defined around the events, set up the EventLinks

            m_MainMenuState.AddLink(new EventLink(worldJoiningWrapper, m_GameLoadingState));
            m_GameLoadingState.AddLink(new EventLink(worldEnteredWrapper, m_GamePlayState));
        }

        // Use this to preload any assets. The QuizU sample only loads a few prefabs, but this is an
        // opportunity to load any textures, models, etc. in advance to avoid loading during gameplay 
        private void InstantiatePreloadedAssets()
        {
            foreach (var asset in m_PreloadedAssets)
            {
                if (asset != null)
                    Instantiate(asset);
            }
        }
        #endregion

        // Event-handling methods
        private void SceneEvents_ExitApplication()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
