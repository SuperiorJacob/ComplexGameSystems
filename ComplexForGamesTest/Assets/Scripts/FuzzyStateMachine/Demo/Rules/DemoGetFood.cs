using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine.Demo
{
    public class DemoGetFood : FuzzyRuleSet
    {
        public DemoGetFood() { }

        public DemoGetFood(params Variable.FuzzyMember[] a_members)
        {
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
            weight = 0.7f;
        }

        public override float UnDesirableRules(out float a_max)
        {
            CheckDesireCategory();

            a_max = this["undesirable"].GetCenter();

            return Rule(
                FuzzyLogic.OR(Is("far"), Is("full")) // if (food is far) or (npc is full) then we do not desire food
            );
        }

        public override float DesirableRules(out float a_max)
        {
            CheckDesireCategory();

            a_max = this["desirable"].GetCenter();

            return Rule(
                FuzzyLogic.AND(Is("hungry"), FuzzyLogic.OR(Is("medium"), Is("close"))) // if (npc is hungry) and (food is close or medium) then we desire food
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