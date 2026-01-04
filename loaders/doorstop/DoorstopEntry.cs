using UnityEngine;

namespace AdofaiMod.MultiLoader.Loaders
{
    public static class DoorstopEntry
    {
        public static void EntryPoint()
        {
            var go = new GameObject("AdofaiMod_Doorstop");
            Object.DontDestroyOnLoad(go);
            go.AddComponent<DoorstopComponent>();
        }
    }

    public class DoorstopComponent : MonoBehaviour
    {
        private DoorstopHandler? _handler;

        public void Awake()
        {
            _handler = new DoorstopHandler();
            Main.Initialize(_handler);
            _handler.TriggerToggle(true);
        }

        public void Update()
        {
            _handler?.TriggerUpdate(Time.deltaTime);
        }

        public void OnGUI()
        {
            _handler?.TriggerGUI();
        }
    }
}
