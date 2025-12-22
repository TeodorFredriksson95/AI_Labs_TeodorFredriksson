using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.AIProject
{
    public class NPCUtility
    {
        public static bool IsTargetDetected(Transform origin, float radius, string tag)
        {
            Collider[] targetsInFov = Physics.OverlapSphere(origin.position, radius);

            foreach (Collider c in targetsInFov)
            {
                if (c.CompareTag(tag))
                {
                    Debug.Log("Found tag" + c.name + " within a " + radius + " radius");
                    return true;
                }
            }
            return false;
        }

    }
}
