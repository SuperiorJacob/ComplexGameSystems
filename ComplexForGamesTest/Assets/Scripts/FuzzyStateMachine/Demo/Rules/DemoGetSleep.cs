using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine.Demo
{
    public class DemoGetSleep : FuzzyRuleSet
    {
        public DemoGetSleep() { }

        public DemoGetSleep(params Variable.FuzzyMember[] a_members)
        {
            weight = 0.6f;

            if (a_members.Length != 9)
            {
                throw new System.Exception($"You do not have enough members for this rule! {a_members.Length} / 9");
            }

            for (int i = 0; i < a_members.Length; i++)
            {
                this[i] = a_members[i];
            }
        }

        public override void SetupWeight()
        {
            weight = 0.9f;
        }

        public override float UnDesirableRules(out float a_max)
        {
            CheckDesireCategory();

            a_max = this["undesirable"].GetCenter();

            return Rule(
                FuzzyLogic.OR(Is("far"), Is("energised")) // if (bed is far) and (npc is engergised) then we do not desire sleep
            );
        }

        public override float DesirableRules(out float a_max)
        {
            CheckDesireCategory();

            a_max = this["desirable"].GetCenter();

            return Rule(
                FuzzyLogic.AND(FuzzyLogic.OR(Is("medium"), Is("close")), Is("tired")) // if (bed is medium or close) and (npc is tired) then we desire sleep
            );
        }

        public override float VeryDesirableRules(out float a_max)
        {
            CheckDesireCategory();

            a_max = this["verydesirable"].GetCenter();

            return Rule(
                Is("exhausted") // if (npc is exhausted) then we really want to sleep
            );
        }
    }
}