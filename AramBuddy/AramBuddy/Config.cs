using AramBuddy.MainCore.Utility;
using static AramBuddy.Program;

namespace AramBuddy
{
    internal class Config
    {
        // Main
        public static bool EnableActivator => MenuIni.CheckBoxValue("activator");
        public static bool EnableDebug => MenuIni.CheckBoxValue("debug");
        public static bool DisableSpellsCasting => MenuIni.CheckBoxValue("DisableSpells");
        public static bool QuitOnGameEnd => MenuIni.CheckBoxValue("quit");
        public static bool DontStealHR => MenuIni.CheckBoxValue("stealhr");
        public static bool EnableChat => MenuIni.CheckBoxValue("chat");
        public static bool DisableTexture => MenuIni.CheckBoxValue("texture");
        public static int SafeValue => MenuIni.SliderValue("Safe");
        public static int HealthRelicHP => MenuIni.SliderValue("HRHP");
        public static int HealthRelicMP => MenuIni.SliderValue("HRMP");

        // Misc
        public static bool EnableAutoLvlUP => MiscMenu.CheckBoxValue("autolvl");
        public static bool EnableAutoShop => MiscMenu.CheckBoxValue("autoshop");
        public static bool TryFixDive => MiscMenu.CheckBoxValue("fixdive");
        public static bool FixedKite => MiscMenu.CheckBoxValue("kite");
        public static bool PickDravenAxe => MiscMenu.CheckBoxValue("dravenaxe");
        public static bool PickBardChimes => MiscMenu.CheckBoxValue("bardchime");
        public static bool PickCorkiBomb => MiscMenu.CheckBoxValue("corkibomb");
        public static bool PickZacBlops => MiscMenu.CheckBoxValue("zacpassive");
        public static bool CreateAzirTower => MiscMenu.CheckBoxValue("azirtower");
        public static bool EnableTeleport => MiscMenu.CheckBoxValue("tp");
        public static bool EnableLogs => MiscMenu.CheckBoxValue("logs");
        public static bool SaveChat => MiscMenu.CheckBoxValue("savechat");
        public static bool Tyler1 => MiscMenu.CheckBoxValue("bigbrother");
        public static float Tyler1g => MiscMenu.SliderValue("gold");
    }
}
