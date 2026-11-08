using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace Mod_PandaPlayerMachineEnhancedMod
{

    [HarmonyPatch(typeof(Card), nameof(Card.LevelUp))]
    public static class MachineLevelUpPatch
    {
        private static readonly Dictionary<int, int> _awardedMilestones = new Dictionary<int, int>();

        [HarmonyPrefix]
        static void Prefix(Card __instance)
        {
            Chara pc;
            try { pc = EMono.pc; } catch { return; }
            if (pc == null || __instance != pc) return;
            if (!IsMachineRace(pc)) return;

            int newLevel = pc.LV + 1;
            if (newLevel <= 0) return;

            int milestones = newLevel / 5;
            if (milestones <= 0) return;

            if (!_awardedMilestones.TryGetValue(pc.uid, out int awarded))
            {
                _awardedMilestones[pc.uid] = milestones;
                return;
            }

            if (milestones <= awarded) return;

            int toGrant = milestones - awarded;
            _awardedMilestones[pc.uid] = milestones;

            for (int i = 0; i < toGrant; i++)
            {
                if (!pc.HasElement(1248))
                    pc.SetFeat(1248, 1);
                else
                    pc.SetFeat(1248, pc.Evalue(1248) + 1);
            }
        }

        private static bool IsMachineRace(Chara c)
        {
            try
            {
                return c.IsMachine;
            }
            catch { }
            try
            {
                var id = c.race?.id;
                if (string.Equals(id, "machine", StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            catch { }
            return false;
        }
    }
}
