using System.Linq;
using AramBuddy.MainCore.Utility;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;

namespace AramBuddy.Plugins.Champions.Sona
{
    internal class Sona : Base
    {
        static Sona()
        {
            MenuIni = MainMenu.AddMenu(MenuName, MenuName);
            AutoMenu = MenuIni.AddSubMenu("Auto");
            ComboMenu = MenuIni.AddSubMenu("Combo");
            HarassMenu = MenuIni.AddSubMenu("Harass");
            LaneClearMenu = MenuIni.AddSubMenu("LaneClear");
            KillStealMenu = MenuIni.AddSubMenu("KillSteal");

            foreach (var spell in SpellList.Where(spell => spell == Q))
            {
                ComboMenu.CreateCheckBox(spell.Slot, "Use " + spell.Slot);
                HarassMenu.CreateCheckBox(spell.Slot, "Use " + spell.Slot);
                HarassMenu.CreateSlider(spell.Slot + "mana", spell.Slot + " Mana Manager", 60);
                LaneClearMenu.CreateCheckBox(spell.Slot, "Use " + spell.Slot);
                LaneClearMenu.CreateSlider(spell.Slot + "mana", spell.Slot + " Mana Manager", 60);
                KillStealMenu.CreateCheckBox(spell.Slot, "Use " + spell.Slot);
            }
            AutoMenu.CreateCheckBox("FleeE", "Flee E");
            AutoMenu.CreateCheckBox("GapE", "Anti-GapCloser E");
            AutoMenu.CreateCheckBox("AutoHeal", "Heal Allies");
            AutoMenu.CreateCheckBox("AutoR", "Auto Ult");
            AutoMenu.CreateSlider("AutoR#", "Auto Ult {0} Enemies", 3, 1, 5);

            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Dash.OnDash += Dash_OnDash;
            Events.OnIncomingDamage += Events_OnIncomingDamage;
        }

        private static void Events_OnIncomingDamage(Events.InComingDamageEventArgs args)
        {
            if (!W.IsReady()) return;
            foreach (
                var ally in
                    EntityManager.Heroes.Allies.Where(a =>
                        a.IsKillable(W.Range) && args.Target == a && args.Target.Health <= args.InComingDamage)
                        .Where(ally => AutoMenu.CheckBoxValue("AutoHeal")))
            {
                W.Cast();
            }
        }

        private static void Dash_OnDash(Obj_AI_Base sender, Dash.DashEventArgs e)
        {
            if (sender == null || !sender.IsEnemy || !sender.IsKillable())
                return;

            if (!E.IsReady() || !AutoMenu.CheckBoxValue("GapE") || !sender.IsKillable(user.GetAutoAttackRange(sender)))
                return;

            E.Cast();
            Player.IssueOrder(GameObjectOrder.AttackUnit, sender);
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (sender == null || !sender.IsEnemy || !sender.IsKillable())
                return;

            if (!E.IsReady() || !AutoMenu.CheckBoxValue("GapE") ||
                !sender.IsKillable(user.GetAutoAttackRange(sender)))
                return;

            E.Cast();
            Player.IssueOrder(GameObjectOrder.AttackUnit, sender);
        }

        public override void Active()
        {
            foreach (
                var ally in
                    EntityManager.Heroes.Allies.Where(a => a.IsKillable(W.Range) && a.HealthPercent < 50)
                        .Where(ally => AutoMenu.CheckBoxValue("AutoHeal")).Where(ally => W.IsReady()))
            {
                W.Cast();
            }

            if (AutoMenu.CheckBoxValue("AutoR"))
            {
                R.SetSkillshot().CastAOE(AutoMenu.SliderValue("AutoR#"));
            }
        }

        public override void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target == null || !target.IsKillable(Q.Range))
                return;

            foreach (
                var spell in
                    SpellList.Where(
                        s =>
                            s.IsReady() && target.IsKillable(s.Range) &&
                            ComboMenu.CheckBoxValue(s.Slot)))
            {
                spell.Cast(target, HitChance.Medium);
            }
        }

        public override void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target == null || !target.IsKillable(Q.Range))
                return;

            foreach (
                var spell in
                    SpellList.Where(
                        s =>
                            s.IsReady() && target.IsKillable(s.Range) &&
                            ComboMenu.CheckBoxValue(s.Slot)))
            {
                spell.Cast(target, HitChance.Medium);
            }
        }

        public override void LaneClear()
        {
            var minions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.IsKillable(Q.Range));
            {
                if (minions.Count() >= 3 && Q.IsReady())
                    Q.Cast();
            }
        }

        public override void Flee()
        {
            if (E.IsReady() && AutoMenu.CheckBoxValue("FleeE"))
                E.Cast();
        }

        public override void KillSteal()
        {
            foreach (
                var spell in
                    from spell in
                        SpellList.Where(s => s.IsReady() && KillStealMenu.CheckBoxValue(s.Slot))
                    from target in
                        EntityManager.Heroes.Enemies.Where(
                            m => m != null && m.IsKillable(spell.Range) && spell.WillKill(m))
                    select spell)
            {
                spell.Cast();
            }
        }
    }
}