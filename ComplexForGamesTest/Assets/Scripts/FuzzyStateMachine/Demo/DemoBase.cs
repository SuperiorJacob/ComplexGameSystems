using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FuzzyStateMachine.Demo
{
    public class DemoBase : DemoDamageable
    {
        void Update()
        {
            if (IsDead)
            {
                Destroy(gameObject);
            }
        }
    }
}