using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine.Rules.Example
{
    public class GetBlanket : FuzzyRuleSet
    {
        public GetBlanket() { }

        public GetBlanket(params Variable.FuzzyMember[] a_members)
        {
            if (a_members.Length != 9)
            {
                throw new System.Exception($"You do not have enough variables for this rule! {a_members.Length} / 9");
            }

            for (int i = 0; i < a_members.Length; i++)
            {
                this[i] = a_members[i];
            }
        }

        public void CheckDesireCategory()
        {
            int desireCount = 0;
            if (categories.TryGetValue("desire", out desireCount))
            {
                if (desireCount < 1)
                    throw new System.Exception($"You have not setup the required DESIRE category!");
            }
        }

        public override float UnDesirableRules(out float a_max)
        {
            CheckDesireCategory();

            a_max = this["undesirable"].GetCenter();

            return Rule(
                FuzzyLogic.OR(Is("far"), Is("hot")) // Rule
            );
        }

        public override float DesirableRules(out float a_max)
        {
            CheckDesireCategory();

            a_max = this["desirable"].GetCenter();

            return Rule(
                FuzzyLogic.AND(Is("medium"), Is("cold")), // First rule
                FuzzyLogic.AND(Is("medium"), Is("warm")) // Second rule
            );
        }

        public override float VeryDesirableRules(out float a_max)
        {
            CheckDesireCategory();

            a_max = this["verydesirable"].GetCenter();

            return Rule(
                FuzzyLogic.AND(Is("close"), Is("cold")), // First rule
                FuzzyLogic.AND(Is("close"), Is("warm")) // Second rule
            );
        }
    }
}