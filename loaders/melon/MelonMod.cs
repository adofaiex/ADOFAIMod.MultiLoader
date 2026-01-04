using MelonLoader;

[assembly: MelonInfo(typeof(AdofaiMod.MultiLoader.Loaders.AdofaiMelonMod), "AdofaiMod.MultiLoader", "1.0.0", "Your Name")]
[assembly: MelonGame("7th Beat Games", "A Dance of Fire and Ice")]

namespace AdofaiMod.MultiLoader.Loaders
{
    public class AdofaiMelonMod : MelonMod
    {
        private MelonHandler? _handler;

        public override void OnInitializeMelon()
        {
            _handler = new MelonHandler(this);
            Main.Initialize(_handler);
            _handler.TriggerToggle(true);
        }

        public override void OnUpdate()
        {
            _handler?.TriggerUpdate(UnityEngine.Time.deltaTime);
        }

        public override void OnGUI()
        {
            _handler?.TriggerGUI();
        }
    }
}
