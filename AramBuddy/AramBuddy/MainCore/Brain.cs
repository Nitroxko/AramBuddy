using System;
using System.Linq;
using AramBuddy.GenesisSpellDatabase;
using AramBuddy.MainCore.Logics;
using AramBuddy.MainCore.Logics.Casting;
using AramBuddy.MainCore.Utility;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using GenesisSpellLibrary;
using SharpDX;

namespace AramBuddy.MainCore
{
    internal class Brain
    {
        private static bool RunningItDownMid;

        /// <summary>
        ///     Init bot functions.
        /// </summary>
        public static void Init()
        {
            try
            {
                // Initialize Genesis Spell Library.
                SpellManager.Initialize();
                SpellLibrary.Initialize();

                // Initialize ObjectsManager.
                ObjectsManager.Init();

                SpecialChamps.Init();

                // Overrides Orbwalker Movements
                Orbwalker.OverrideOrbwalkPosition += OverrideOrbwalkPosition;

                // Initialize AutoLvlup.
                LvlupSpells.Init();

                // Initialize TeamFights Detector.
                TeamFightsDetection.Init();

                Obj_AI_Base.OnBasicAttack += Obj_AI_Base_OnBasicAttack;
                Gapcloser.OnGapcloser += SpellsCasting.GapcloserOnOnGapcloser;
                Interrupter.OnInterruptableSpell += SpellsCasting.Interrupter_OnInterruptableSpell;
                //Obj_AI_Base.OnBasicAttack += SpellsCasting.Obj_AI_Base_OnBasicAttack;
                //Obj_AI_Base.OnProcessSpellCast += SpellsCasting.Obj_AI_Base_OnProcessSpellCast;
            }
            catch (Exception ex)
            {
                Logger.Send("There was an Error While Initialize Brain", ex, Logger.LogLevel.Error);
            }
        }

        /// <summary>
        ///     Returns LastUpdate for the bot current postion.
        /// </summary>
        public static float LastUpdate;

        /// <summary>
        ///     Decisions picking for the bot.
        /// </summary>
        public static void Decisions()
        {
            // Picks best position for the bot.
            if (Core.GameTickCount - LastUpdate > Misc.ProtectFPS)
            {
                /*
                foreach (var hero in EntityManager.Heroes.AllHeroes.Where(a => a != null && a.IsValidTarget() && !a.Added()
                && (ObjectsManager.HealthRelics.Any(hr => a.Path.LastOrDefault().Distance(hr.Position) <= 1) || EntityManager.Heroes.AllHeroes.Any(b => !a.IdEquals(b) && a.Distance(b) <= 1))))
                {
                    hero.Add();
                    Logger.Send("BOT DETECTED: " + hero.BaseSkinName + " - " + hero.NetworkId, Logger.LogLevel.Warn);
                }
                */

                Pathing.BestPosition();
                LastUpdate = Core.GameTickCount;
            }

            // Ticks for the modes manager.
            ModesManager.OnTick();

            if (Config.FixedKite && !(Program.Moveto.Contains("Enemy") || Program.Moveto.Contains("AllySpawn")) && !(ModesManager.Flee || ModesManager.None) && ObjectsManager.NearestEnemy != null && Pathing.Position.CountEnemiesInRange(Misc.KiteDistance(ObjectsManager.NearestEnemy)) > 1)
            {
                Program.Moveto = "FixedToKitingPosition";
                Pathing.Position = ObjectsManager.NearestEnemy.Position.Extend(ObjectsManager.AllySpawn, Misc.KiteDistance(ObjectsManager.NearestEnemy)).To3D();
            }

            if (Config.TryFixDive && Pathing.Position.UnderEnemyTurret() && !Misc.SafeToDive)
            {
                Program.Moveto = "FixedToAntiDivePosition";
                Pathing.Position = ObjectsManager.EnemyTurretNearSpawn.ServerPosition.Extend(ObjectsManager.AllySpawn.Position.Random(), ObjectsManager.EnemyTurretNearSpawn.GetAutoAttackRange(Player.Instance) + 200).To3D();
            }

            if (Config.CreateAzirTower && ObjectsManager.AzirTower != null)
            {
                Program.Moveto = "CreateAzirTower";
                Player.UseObject(ObjectsManager.AzirTower);
            }

            RunningItDownMid = Config.Tyler1 && Player.Instance.Gold >= Config.Tyler1g
                && (Player.Instance.Distance(ObjectsManager.AllySpawn) > 4000 || EntityManager.Heroes.Enemies.Count(e => !e.IsDead) == 0)
                && EntityManager.Heroes.Allies.Count(a => a.IsValidTarget() && !a.IsMe) >= 2;
            if (RunningItDownMid)
            {
                Program.Moveto = "RUNNING IT DOWN MID";
                Pathing.Position = ObjectsManager.EnemySpawn.Position.Random();
            }

            // Moves to the Bot selected Position.
            if (Pathing.Position != Vector3.Zero && Pathing.Position.IsValid() && !Pathing.Position.IsZero)
            {
                Pathing.MoveTo(Pathing.Position);
            }

            Spellbook.OnCastSpell += delegate(Spellbook sender, SpellbookCastSpellEventArgs args)
                {
                    if (sender.Owner.IsMe && RunningItDownMid)
                        args.Process = false;
                };
        }

        /// <summary>
        ///     Bool returns true if the bot is alone.
        /// </summary>
        public static bool Alone()
        {
            return Player.Instance.CountAlliesInRange(4500) < 2 || Player.Instance.Path.Any(p => p.IsInRange(Game.CursorPos, 50))
                   || EntityManager.Heroes.Allies.All(a => !a.IsMe && (a.IsInShopRange() || a.IsInFountainRange() || a.IsDead));
        }

        /// <summary>
        ///     Last Turret Attack Time.
        /// </summary>
        public static float LastTurretAttack;

        /// <summary>
        ///     Checks Turret Attacks And saves Heros AutoAttacks.
        /// </summary>
        public static void Obj_AI_Base_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var turret = sender as Obj_AI_Turret;
            if (turret != null)
            {
                if (args.Target.IsMe)
                {
                    LastTurretAttack = Core.GameTickCount;
                }
                
                var target = args.Target as AIHeroClient;
                if (target != null && target.IsAlly && !target.IsMe)
                {
                    var lastAttack = new Misc.LastAttack(turret, target) { Attacker = turret, LastAttackSent = Core.GameTickCount, Target = target };
                    Misc.AutoAttacks.Add(lastAttack);
                }
            }
        }

        /// <summary>
        ///     Override orbwalker position.
        /// </summary>
        private static Vector3? OverrideOrbwalkPosition()
        {
            return Pathing.Position;
        }
    }
}
