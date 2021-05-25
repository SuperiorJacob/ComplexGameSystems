using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine.Demo
{
    public class DemoFightRule : FuzzyRuleSet
    {
        public DemoFightRule() { }

        public DemoFightRule(params Variable.FuzzyMember[] a_members)
        {
            if (a_members.Length < 9)
            {
                throw new System.Exception($"You do not have enough members for this rule! {a_members.Length} / 9");
            }

            for (int i = 0; i < a_members.Length; i++)
            {
                this[i] = a_members[i];
            }

            SetupWeight();
        }

        public override void SetupWeight()
        {
            weight = 0.7f;
        }

        public override float UnDesirableRules(out float a_max)
        {
            CheckDesireCategory();

            a_max = this["undesirable"].GetCenter();

            return Rule( // if (npc has no teammates OR broken OR starving OR dying) OR ((npc is socialite) AND (npc is broken)) then we will flee.
                FuzzyLogic.OR(
                    Is("none"),
                    Is("broken"),
                    Is("starving"),
                    Is("dying")
                ),
                FuzzyLogic.AND(
                    Is("socialite"),
                    Is("broken")
                )
            );
        }

        public override float DesirableRules(out float a_max)
        {
            CheckDesireCategory();

            a_max = this["desirable"].GetCenter();

            return Rule( // if (npc is normal OR average OR leader OR half) then we desire to fight.
                FuzzyLogic.OR(
                    Is("normal"), 
                    Is("average"), 
                    Is("leader"), 
                    Is("half")
                )
            );
        }

        public override float VeryDesirableRules(out float a_max)
        {
            CheckDesireCategory();

            a_max = this["verydesirable"].GetCenter();

            return Rule( // if (npc is enraged OR leader OR all members present) then we very much desire to fight.

                FuzzyLogic.OR(
                    Is("enraged"),
                    Is("leader"),
                    Is("all")
                )
            );
        }
    }
}
