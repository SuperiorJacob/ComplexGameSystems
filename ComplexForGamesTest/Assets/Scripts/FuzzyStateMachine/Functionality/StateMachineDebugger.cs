using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine
{
    public class StateMachineDebugger : MonoBehaviour
    {
        public enum RuleTest
        {
            And,
            Or,
            Not
        }

        public enum Statement
        {
            IsCold,
            IsWarm,
            IsHot
        }


        [Header("Test")]
        public float input = 60;
        public RuleTest ruleTest;
        public Statement statement;
        public Statement secondStatement;
        public Statement veryDesirableStatement;
        public Statement unDesirableStatement;
        public float deffuziedOutput = 0;
        public Vector3 desirability;

        public Dictionary<string, Variable.FuzzyMember> fuzzies = new Dictionary<string, Variable.FuzzyMember>();

        [ContextMenu("Perform")]
        void Test()
        {
            fuzzies["cold"] = new Variable.FuzzyMember(Variable.FuzzyMember.FuzzyShapeType.LeftShoulder, Color.cyan, 0, 0, 20, 40);
            fuzzies["warm"] = new Variable.FuzzyMember(Variable.FuzzyMember.FuzzyShapeType.Triangle, Color.yellow, 30, 50, 70);
            fuzzies["hot"] = new Variable.FuzzyMember(Variable.FuzzyMember.FuzzyShapeType.RightShoulder, Color.red, 50, 80, 100, 100);

            float isCold = fuzzies["cold"].GetMembership(input);
            float isWarm = fuzzies["warm"].GetMembership(input);
            float isHot = fuzzies["hot"].GetMembership(input);

            float maxVeryDesirable = desirability.z;
            float maxDesirable = desirability.y;
            float maxUndesirable = desirability.x;

            float veryDesirable = veryDesirableStatement == Statement.IsCold ? isCold : (veryDesirableStatement == Statement.IsWarm ? isWarm : isHot);

            float statementDeclare = statement == Statement.IsCold ? isCold : (statement == Statement.IsWarm ? isWarm : isHot);
            float statementDeclare2 = secondStatement == Statement.IsCold ? isCold : (secondStatement == Statement.IsWarm ? isWarm : isHot);

            float desirable = ruleTest == RuleTest.Not ? FuzzyLogic.NOT(statementDeclare) : (ruleTest == RuleTest.And ? FuzzyLogic.AND(statementDeclare, statementDeclare2) : FuzzyLogic.OR(statementDeclare, statementDeclare2));

            float undesirable = unDesirableStatement == Statement.IsCold ? isCold : (unDesirableStatement == Statement.IsWarm ? isWarm : isHot);

            // Centroid defuzz
            float desire = maxVeryDesirable * veryDesirable + maxDesirable * desirable + maxUndesirable * undesirable;
            desire /= (veryDesirable + desirable + undesirable);

            deffuziedOutput = desire;

            Debug.Log($"It's {input} (crisp) degrees, should I get a blanket? (fuzzification)");
            Debug.Log($"IsCold: {isCold}");
            Debug.Log($"IsWarm: {isWarm}");
            Debug.Log($"IsHot: {isHot}");
            Debug.Log($"Desirability to get a blanket: {desirable} {desire} (defuzzied)");
        }
    }
}