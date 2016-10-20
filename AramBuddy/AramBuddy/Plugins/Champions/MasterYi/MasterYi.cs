using System;
using System.Collections.Generic;
using System.Linq;
using AramBuddy.MainCore.Utility.MiscUtil;
using AramBuddy.Plugins.KappaEvade;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using SharpDX;

namespace AramBuddy.Plugins.Champions.MasterYi
{
    internal class MasterYi : Base
    {
        static MasterYi()
        {
            MenuIni = MainMenu.AddMenu(MenuName, MenuName);
            AutoMenu = MenuIni.AddSubMenu("Auto");
            ComboMenu = MenuIni.AddSubMenu("Combo");
            HarassMenu = MenuIni.AddSubMenu("Harass");
            LaneClearMenu = MenuIni.AddSubMenu("LaneClear");
            KillStealMenu = MenuIni.AddSubMenu("KillSteal");
            foreach (var spell in SpellList)
            {
                ComboMenu.CreateCheckBox(spell.Slot, "Use " + spell.Slot);
                if (spell == Q || spell == E)
                {
                    HarassMenu.CreateCheckBox(spell.Slot, "Use " + spell.Slot);
                    HarassMenu.CreateSlider(spell.Slot + "mana", spell.Slot + " Mana Manager", 60);
                    LaneClearMenu.CreateCheckBox(spell.Slot, "Use " + spell.Slot);
                    LaneClearMenu.CreateSlider(spell.Slot + "mana", spell.Slot + " Mana Manager", 60);
                }
            }
        }

        public override void Active()
        {
            
        }

        public override void Combo()
        {
            
        }

        public override void Flee()
        {
            
        }

        public override void Harass()
        {
           
        }

        public override void KillSteal()
        {
            
        }

        public override void LaneClear()
        {
            
        }
    }
}