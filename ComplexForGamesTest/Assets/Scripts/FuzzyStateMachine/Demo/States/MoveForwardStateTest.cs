using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine.States
{
    public class MoveForwardStateTest : StateMachineState
    {
        public MoveForwardStateTest() { executionType = StateExecuteType.Update; }

        public override void Execute(GameObject a_obj)
        {
            base.Execute(a_obj);

            StateController.transform.position = Vector3.Lerp(StateController.transform.position, StateController.transform.position + StateController.transform.forward * 10, Time.deltaTime);
        }

        public override void Finish()
        {

        }
    }
}