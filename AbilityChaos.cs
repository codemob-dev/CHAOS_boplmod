using BoplFixedMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHAOS
{
    public class AbilityScramble : OneTimeChaosEvent
    {
        public override string Activate()
        {
            foreach (var player in PlayerHandler.Get().PlayerList())
            {
                player.AbilityIcons.Insert(0, player.AbilityIcons.Last());
                player.AbilityIcons.RemoveAt(player.AbilityIcons.Count - 1);

                player.Abilities.Insert(0, player.Abilities.Last());
                player.Abilities.RemoveAt(player.Abilities.Count - 1);
            }
            return "Ability Scramble (credits to UnluckyCrafter)";
        }
    }
}
