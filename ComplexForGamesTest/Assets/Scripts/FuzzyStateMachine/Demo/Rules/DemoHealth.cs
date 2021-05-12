using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine.Demo
{
    public class DemoHealth : FuzzyRuleSet
    {
        public DemoHealth() { }

        public DemoHealth(params Variable.FuzzyMember[] a_members)
        {
            if (a_members.Length != 6)
            {
                throw new System.Exception($"You do not have enough members for this rule! {a_members.Length} / 6");
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
                Is("healthy") // if (npc is healthy) then do not seek health
            );
        }

        public override float DesirableRules(out float a_max)
        {
            CheckDesireCategory();

            a_max = this["desirable"].GetCenter();

            return Rule(
                Is("injured") // if (npc is injured) then seek health
            );
        }

        public override float VeryDesirableRules(out float a_max)
        {
            CheckDesireCategory();

            a_max = this["verydesirable"].GetCenter();

            return Rule(
                Is("nearDeath") // if (npc is near death) then we desire to seek health badly
            );
        }
    }
}