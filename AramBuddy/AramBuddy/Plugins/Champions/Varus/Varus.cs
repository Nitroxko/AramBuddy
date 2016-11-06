using System;
using System.Linq;
using AramBuddy.Plugins.KappaEvade;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using static AramBuddy.MainCore.Utility.MiscUtil.Misc;

namespace AramBuddy.Plugins.Champions.Varus
{
    internal class Varus : Base
    {
        public static new Spell.Chargeable Q = new Spell.Chargeable(SpellSlot.Q, 925, 1625, 2000, 0, 1900, 100) { AllowedCollisionCount = int.MaxValue };
        static Varus()
        {

            foreach (var spell in SpellList)
            {
                if (spell != W)
                {
                    ComboMenu.CreateCheckBox(spell.Slot, "Use " + spell.Slot);
                }
                if (spell != W && spell != R)
                {
                    HarassMenu.CreateCheckBox(spell.Slot, "Use " + spell.Slot);
                    HarassMenu.CreateSlider(spell.Slot + "mana", spell.Slot + " Mana Manager", 60);
                    LaneClearMenu.CreateCheckBox(spell.Slot, "Use " + spell.Slot);
                    LaneClearMenu.CreateSlider(spell.Slot + "mana", spell.Slot + " Mana Manager", 60);
                    KillStealMenu.CreateCheckBox(spell.Slot, "Use " + spell.Slot);
                }
            }
        }
        public override void Active()
        {
           
        }

        public override void Combo()
        {
            var qtar = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var etar = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            var rtar = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            if (ComboMenu.CheckBoxValue(R.Slot) && R.IsReady() && !Q.IsCharging)
                R.Cast(rtar);
            if (ComboMenu.CheckBoxValue(E.Slot) && E.IsReady() && etar.GetBuffCount("varuswdebuff") == 3 && !Q.IsCharging)
                E.Cast(etar);
            if (ComboMenu.CheckBoxValue(Q.Slot) && Q.IsReady())
            {
                if (Q.IsCharging)
                    Q.Cast(qtar);
                else
                    Q.StartCharging();
            }

        }

        public override void Flee()
        {
            
        }

        public override void Harass()
        {
            var qtar = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var etar = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (HarassMenu.CheckBoxValue(E.Slot) && E.IsReady() && !Q.IsCharging && etar != null)
                E.Cast(etar);
            if (HarassMenu.CheckBoxValue(Q.Slot) && Q.IsReady() && qtar != null)
            {
                if (Q.IsCharging)
                    Q.Cast(qtar);
                else
                    Q.StartCharging();
            }
        }

        public override void KillSteal()
        {
            foreach (var target in EntityManager.Heroes.Enemies.Where(t => t != null))
            {
                if (KillStealMenu.CheckBoxValue(E.Slot) && E.IsReady() && !Q.IsCharging && target.IsKillable(E.Range) && E.WillKill(target))
                    E.Cast(target);
                if (KillStealMenu.CheckBoxValue(Q.Slot) && Q.IsReady() && target.IsKillable(Q.Range) && Q.WillKill(target))
                {
                    if (Q.IsCharging)
                        Q.Cast(target);
                    else
                        Q.StartCharging();
                }

            }
        }

        public override void LaneClear()
        {
            var linefarmloc = Q.SetSkillshot().GetBestLinearCastPosition(Q.LaneMinions());
            if (Q.IsReady() && linefarmloc.HitNumber > 1 && LaneClearMenu.CheckBoxValue(SpellSlot.Q) && LaneClearMenu.CompareSlider(Q.Slot + "mana", user.ManaPercent))
            {
                if (Q.IsCharging)
                    Q.Cast(linefarmloc.CastPosition);
                else
                    Q.StartCharging();
            }
            foreach (
                var circFarmLoc in
                    EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m != null && m.IsKillable(1000)).Select(
                            target =>
                                EntityManager.MinionsAndMonsters.GetCircularFarmLocation(
                                    EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.IsKillable(E.Range)),
                                    E.SetSkillshot().Width, (int)E.Range))
                        .Where(
                            circFarmLoc =>
                                E.IsReady() && circFarmLoc.HitNumber > 1 && LaneClearMenu.CheckBoxValue(SpellSlot.E) &&
                                LaneClearMenu.CompareSlider(E.Slot + "mana", user.ManaPercent)))
            {
                E.Cast(circFarmLoc.CastPosition);
            }
        }
    }
}
