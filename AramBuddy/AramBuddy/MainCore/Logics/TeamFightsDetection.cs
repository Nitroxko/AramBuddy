using AramBuddy.MainCore.Utility;
using EloBuddy;
using EloBuddy.SDK;

namespace AramBuddy.MainCore.Logics
{
    class TeamFightsDetection
    {
        public static void Init()
        {
            // Used for detecting targeted spells for TeamFights Detection
            Obj_AI_Base.OnProcessSpellCast += delegate (Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            {
                var from = sender as AIHeroClient;
                var target = args.Target as AIHeroClient;
                if (from != null)
                {
                    if (args.Slot == SpellSlot.R)
                    {
                        var lastAttack = new Misc.LastAttack(from, null) { Attacker = from, LastAttackSent = Core.GameTickCount, Target = null };
                        Misc.AutoAttacks.Add(lastAttack);
                        return;
                    }
                    if (target != null && from.Team != target.Team)
                    {
                        var lastAttack = new Misc.LastAttack(from, target) { Attacker = from, LastAttackSent = Core.GameTickCount, Target = target };
                        Misc.AutoAttacks.Add(lastAttack);
                    }
                }
            };

            // Used for detecting AutoAttacks for TeamFights Detection
            Obj_AI_Base.OnBasicAttack += delegate(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
                {
                    var from = sender as AIHeroClient;
                    var target = args.Target as AIHeroClient;
                    if (from != null && target != null)
                    {
                        var lastAttack = new Misc.LastAttack(from, target) { Attacker = from, LastAttackSent = Core.GameTickCount, Target = target };
                        Misc.AutoAttacks.Add(lastAttack);
                    }
                };
        }
    }
}
