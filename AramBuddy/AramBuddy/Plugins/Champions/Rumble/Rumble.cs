using System;
using System.Collections.Generic;
using System.Linq;
using AramBuddy.MainCore.Utility;
using AramBuddy.MainCore.Utility.MiscUtil;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace AramBuddy.Plugins.Champions.Rumble
{
    internal class Rumble : Base
    {
        static Rumble()
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
                if (spell != R && spell != W)
                {
                    HarassMenu.CreateCheckBox(spell.Slot, "Use " + spell.Slot);
                    LaneClearMenu.CreateCheckBox(spell.Slot, "Use " + spell.Slot);
                    KillStealMenu.CreateCheckBox(spell.Slot, "Use " + spell.Slot);
                }
            }

        }



        public override void Active()
        {
            
        }

        public override void Combo()
        {
            var etarget = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (etarget == null || !etarget.IsKillable(E.Range)) return;
            if (ComboMenu.CheckBoxValue(SpellSlot.E) && E.IsReady())
                E.Cast(etarget);
            var rtarget = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            if (rtarget == null || !rtarget.IsKillable(R.Range)) return;
            if (ComboMenu.CheckBoxValue(SpellSlot.R) && R.IsReady())
                rtarget.RCast();
            var qtarget = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (qtarget == null || !qtarget.IsKillable(Q.Range)) return;
            if (ComboMenu.CheckBoxValue(Q.Slot) && Q.IsReady())
                Q.Cast();
            if (ComboMenu.CheckBoxValue(W.Slot) && W.IsReady())
                W.Cast();
        }

        public override void Flee()
        {
            if (W.IsReady())
                W.Cast();
        }

        public override void Harass()
        {
            var etarget = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (etarget == null || !etarget.IsKillable(E.Range)) return;
            if (HarassMenu.CheckBoxValue(SpellSlot.E) && E.IsReady())
                E.Cast(etarget);
            var qtarget = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (qtarget == null || !qtarget.IsKillable(Q.Range)) return;
            if (HarassMenu.CheckBoxValue(Q.Slot) && Q.IsReady())
                Q.Cast();

        }

        public override void KillSteal()
        {
            foreach (var target in EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget(Q.Range) && Q.WillKill(e)))
            {
                if (KillStealMenu.CheckBoxValue(Q.Slot) && Q.IsReady())
                    Q.Cast();
            }
            foreach (var target in EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget(E.Range) && E.WillKill(e)))
            {
                if (KillStealMenu.CheckBoxValue(E.Slot) && E.IsReady())
                    E.Cast();
            }
        }

        public override void LaneClear()
        {
            foreach (var target in EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m != null && m.IsValidTarget()))
            {
                if (target.IsKillable(Q.Range) && Q.IsReady() && LaneClearMenu.CheckBoxValue(Q.Slot))
                    Q.Cast();
                if (target.IsKillable(E.Range) && E.IsReady() && LaneClearMenu.CheckBoxValue(E.Slot))
                    E.Cast(target);
            }
        }
    }
    internal static class Vectors
    {
        public static void RCast(this AIHeroClient target, int HitCount = 1)
        {
            if (!Base.R.IsReady()) return;

            var rectlist = new List<Geometry.Polygon.Rectangle>();
            rectlist.Clear();
            var pred = Base.R.GetPrediction(target);

            if (pred.HitChance < HitChance.Low) return;

            Vector3 Start = pred.CastPosition.Distance(Player.Instance) > 1625 ? Player.Instance.ServerPosition.Extend(pred.CastPosition, 1625).To3D() : target.ServerPosition;
            Vector3 End = pred.CastPosition;

            foreach (var A in EntityManager.Heroes.Enemies.OrderBy(o => o.PredictHealth()).Where(e => e.IsKillable(Base.R.Range) && e.NetworkId != target.NetworkId))
            {
                var predmobB = Base.R.GetPrediction(A);
                End = Start.Extend(predmobB.CastPosition, 1700).To3D();
                rectlist.Add(new Geometry.Polygon.Rectangle(Start, End, Base.R.SetSkillshot().Width));
            }

            var bestpos = rectlist.OrderByDescending(r => EntityManager.Heroes.Enemies.OrderBy(o => o.PredictHealth()).Count(m => r.IsInside(m) && m.IsKillable(Base.R.Range))).FirstOrDefault();

            if (bestpos != null)
            {
                Start = bestpos.Start.To3D();
                End = bestpos.End.To3D();

                if (HitCount > 1)
                {
                    if (EntityManager.Heroes.Enemies.OrderBy(o => o.PredictHealth()).Count(m => bestpos.IsInside(m) && m.IsKillable(Base.R.Range)) >= HitCount)
                    {
                        Base.R.CastStartToEnd(End, Start);
                    }
                }
                else
                {
                    Base.R.CastStartToEnd(End, Start);
                }
            }
            else
            {
                Base.R.CastStartToEnd(End, Start);
            }
        }


        public static void RCast(int HitCount = 1)
        {
            if (!Base.R.IsReady()) return;

            var rectlist = new List<Geometry.Polygon.Rectangle>();
            rectlist.Clear();
            Vector3 Start;
            Vector3 End;
            var mobs = EntityManager.MinionsAndMonsters.EnemyMinions.OrderBy(o => o.PredictHealth()).Where(e => e.IsKillable(Base.R.Range));

            foreach (var A in mobs)
            {
                var predmob = Base.R.GetPrediction(A);
                Start = predmob.CastPosition.Distance(Player.Instance) > 1625 ? Player.Instance.ServerPosition.Extend(predmob.CastPosition, 1625).To3D() : A.ServerPosition;
                var mobs2 = EntityManager.MinionsAndMonsters.EnemyMinions.OrderBy(o => o.PredictHealth()).Where(e => e.IsKillable(Base.R.Range) && e.NetworkId != A.NetworkId && e.IsInRange(A, 600));
                foreach (var B in mobs2)
                {
                    var predmobB = Base.R.GetPrediction(B);
                    End = Start.Extend(predmobB.CastPosition, 1700).To3D();
                    rectlist.Add(new Geometry.Polygon.Rectangle(Start, End, Base.R.SetSkillshot().Width));
                }
            }

            var bestpos = rectlist.OrderByDescending(r => EntityManager.MinionsAndMonsters.EnemyMinions.OrderBy(o => o.PredictHealth()).Count(m => r.IsInside(m) && m.IsKillable(Base.R.Range))).FirstOrDefault();

            if (bestpos != null)
            {
                var mobs3 = EntityManager.MinionsAndMonsters.EnemyMinions.OrderBy(o => o.PredictHealth()).Count(m => bestpos.IsInside(m) && m.IsKillable(Base.R.Range));
                if (mobs3 >= HitCount)
                {
                    Start = bestpos.Start.To3D();
                    End = bestpos.End.To3D();
                    Base.R.CastStartToEnd(End, Start);
                }
            }
        }
    }
}
