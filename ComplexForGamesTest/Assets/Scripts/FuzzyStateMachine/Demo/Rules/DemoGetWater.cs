using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine.Demo
{
    public class DemoGetWater : FuzzyRuleSet
    {
        public DemoGetWater() { }

        public DemoGetWater(params Variable.FuzzyMember[] a_members)
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

        public override float UnDesirableRules(out float a_max)
        {
            CheckDesireCategory();

            a_max = this["undesirable"].GetCenter();

            return Rule(
                FuzzyLogic.AND(Is("far"), Is("quenched")) // if (water is far) and (npc is quenched) then we dont desire water
            );
        }

        public override float DesirableRules(out float a_max)
        {
            CheckDesireCategory();

            a_max = this["desirable"].GetCenter();

            return Rule(
                FuzzyLogic.AND(Is("medium"), Is("thirsty")) // if (bed is medium or close) and (npc is awake or tired) then we desire water
            );
        }

        public override float VeryDesirableRules(out float a_max)
        {
            CheckDesireCategory();

            a_max = this["verydesirable"].GetCenter();

            return Rule(
                FuzzyLogic.AND(FuzzyLogic.OR(Is("medium"), Is("close")), FuzzyLogic.OR(Is("thirsty"), Is("dehydrated"))) // if (water is medium or close) and (npc is thirsty or dehydrated) then we desire water
            );
        }
    }
}