using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine.Demo
{
    public class DemoFoodRule : FuzzyRuleSet
    {
        public DemoFoodRule() { }

        public DemoFoodRule(params Variable.FuzzyMember[] a_members)
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
            weight = 1f;
        }

        public override float UnDesirableRules(out float a_max)
        {
            CheckDesireCategory();

            a_max = this["undesirable"].GetCenter();

            return Rule(
                FuzzyLogic.OR( // if (average OR sated) then undesireable to get food
                    Is("average"), 
                    Is("sated")
                )
            );
        }

        public override float DesirableRules(out float a_max)
        {
            CheckDesireCategory();

            a_max = this["desirable"].GetCenter();

            return Rule( // if (npc is dying OR hurt) AND (npc is starving or average) then its desireable to get food
                FuzzyLogic.AND(
                    FuzzyLogic.OR(
                        Is("dying"),
                        Is("hurt")
                    ),
                    FuzzyLogic.OR(
                        Is("starving"),
                        Is("average")
                    )
                )
            );
        }

        public override float VeryDesirableRules(out float a_max)
        {
            CheckDesireCategory();

            a_max = this["verydesirable"].GetCenter();

            return Rule(
                Is("starving") // if (npc is starving) then we really want to eat
            );
        }
    }
}