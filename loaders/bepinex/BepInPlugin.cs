using BepInEx;

namespace AdofaiMod.MultiLoader.Loaders
{
    [BepInPlugin(ModId, "AdofaiMod.MultiLoader", "1.0.0")]
    [BepInProcess("A Dance of Fire and Ice.exe")]
    public class AdofaiBepInPlugin : BaseUnityPlugin
    {
        private const string ModId = "AdofaiMod.MultiLoader";
        private BepInHandler? _handler;

        private void Awake()
        {
            _handler = new BepInHandler(Logger);
            Main.Initialize(_handler);
            _handler.TriggerToggle(true);
        }

        private void Update()
        {
            _handler?.TriggerUpdate(UnityEngine.Time.deltaTime);
        }

        private void OnGUI()
        {
            _handler?.TriggerGUI();
        }
    }
}
