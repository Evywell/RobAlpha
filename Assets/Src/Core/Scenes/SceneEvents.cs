using System;

namespace UnityClientSources.Core.Scenes
{
    /// <summary>
    /// Public static delegates to manage scene loading/unloading (note these are "events" in the conceptual sense
    /// and not the strict C# sense).
    /// </summary>
    public static class SceneEvents
    {
        // Sequence events:
        public static Action ExitApplication;
        public static Action PreloadCompleted;
        public static Action<float> LoadProgressUpdated;

        // Scene management events
        public static Action ReloadScene;
        public static Action LoadNextScene;

        // Additively load the scene with the given path
        public static Action<string> LoadSceneByPath;

        // Additively load the scene with the given path
        public static Action<string> UnloadSceneByPath;

        // Additively load the scene with given index
        public static Action<int> SceneIndexLoaded;

        // Unload the previous scene
        public static Action LastSceneUnloaded;
    }
}
