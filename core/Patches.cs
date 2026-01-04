using HarmonyLib;

namespace AdofaiMod.MultiLoader
{
    [HarmonyPatch(typeof(scrController), "Start")]
    public static class ControllerStartPatch
    {
        public static void Prefix()
        {
            Main.Handler.Log("Controller start");
        }

        public static void Postfix()
        {
            Main.Handler.Log("Controller started");

            if (Main.Settings.EnableFeature)
            {
                Main.Handler.Log($"Feature enabled, example value: {Main.Settings.ExampleValue}");
            }
        }
    }
}
