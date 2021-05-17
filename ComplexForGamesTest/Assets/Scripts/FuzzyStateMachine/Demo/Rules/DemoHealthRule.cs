using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine.Demo
{
    public class DemoHealthRule : FuzzyRuleSet
    {
        public DemoHealthRule() { }

        public DemoHealthRule(params Variable.FuzzyMember[] a_members)
        {
            if (a_members.Length < 6)
            {
                throw new System.Exception($"You do not have enough members for this rule! {a_members.Length} / 6");
            }

            for (int i = 0; i < a_members.Length; i++)
            {
                this[i] = a_members[i];
            }

            SetupWeight();
        }

        public override void SetupWeight()
        {
            weight = 1f;
        }

        public override float UnDesirableRules(out float a_max)
        {
            CheckDesireCategory();

            a_max = this["undesirable"].GetCenter();

            return Rule(
                Is("healthy") // if (npc is healthy) then do not seek health
            );
        }

        public override float DesirableRules(out float a_max)
        {
            CheckDesireCategory();

            a_max = this["desirable"].GetCenter();

            return Rule(
                Is("hurt") // if (npc is injured) then seek health
            );
        }

        public override float VeryDesirableRules(out float a_max)
        {
            CheckDesireCategory();

            a_max = this["verydesirable"].GetCenter();

            return Rule( // if ((npc is dying OR hurt) AND (npc is none OR half OR broken OR normal)) OR (npc is dying) then VeryDesireable
                FuzzyLogic.AND(
                    FuzzyLogic.OR( 
                        Is("dying"), // Health
                        Is("hurt")
                    ),
                    FuzzyLogic.OR( 
                        Is("none"), // Leadership
                        Is("half"),
                        Is("broken"), // Courage
                        Is("normal")
                    )
                ), 
                Is("dying")
            );
        }
    }
}