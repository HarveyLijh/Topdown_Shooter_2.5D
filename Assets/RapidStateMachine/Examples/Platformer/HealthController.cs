using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSM
{
    public class HealthController : MonoBehaviour
    {
        public ConditionTrigger dieTrigger;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag != "Spike") return;
            dieTrigger.Trigger();
        }
    }
}