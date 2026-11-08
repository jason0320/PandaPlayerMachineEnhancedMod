using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using HarmonyLib;

namespace Mod_PandaPlayerMachineEnhancedMod
{
    [BepInPlugin("panda.playermachineenhanced.mod", "Panda's Player Machine Enhanced Mod", "1.0.0.0")]

    public class PandaPlayerMachineEnhancedMod : BaseUnityPlugin
    {
        private void OnStartCore()
        {
            var harmony = new Harmony("Panda's Player Machine Enhanced Mod");
            harmony.PatchAll();
        }

    }
}
