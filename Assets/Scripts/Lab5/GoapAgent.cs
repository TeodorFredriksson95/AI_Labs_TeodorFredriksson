using Assets.Scripts.Lab5;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class GoapAgent : MonoBehaviour
{
    public enum GoapFact
    {
        SeesPlayer,
        WeaponExists,
        HasWeapon,
        AtWeapon,
        AtPlayer,
        PatrolStepDone,
        PlayerTagget
    }

    public readonly struct GoapState : IEquatable<GoapState>
    {
        public readonly ulong Bits;
        public GoapState(ulong bits) => Bits = bits;
        public bool Has(GoapFact fact) => (Bits & (1UL << (int)fact)) != 0; // Look this line up.

        public GoapState With(GoapFact fact) => new GoapState(Bits | (1UL << (int)fact));
        public GoapState Without(GoapFact fact) => new GoapState(Bits & ~(1UL << (int)fact));
        public bool Satisfies(ulong goalMask) => (Bits & goalMask) == goalMask;

        public bool Equals(GoapState other) => Bits == other.Bits;
        public override bool Equals(object obj) => obj is GoapState s && Equals(s);
    }

    public static class GoapBits
    {
        public static ulong Mask(params GoapFact[] facts)
        {
            ulong m = 0;
            foreach (var f in facts) m |= 1UL << (int)f;
            return m;
        }
    }

    public sealed class GoapPlanResult
    {
        public readonly List<GoapActionBase> Actions = new();
        public float TotalCost;
    }

    public static class GoapPlanner
    {
        private struct CameFrom
        {
            public GoapState Prev;
            public GoapActionBase Action;
            public bool HasPrev;
        }

        // Dijkstra in state-space (small action sets => simple open list is fine)
        public static GoapPlanResult Plan(GoapState start, ulong goalMask, List<GoapActionBase> actions)
        {
            List<GoapState> open = new List<GoapState> { start };
            Dictionary<GoapState, float> cost = new Dictionary<GoapState, float> { [start] = 0f };
            Dictionary<GoapState, CameFrom> came = new Dictionary<GoapState, CameFrom>();

            while (open.Count > 0)
            {
                // Pick lowest-cost state (0(n); Ok for small demos)
                int bestIdx = 0;
                float bestCost = cost[open[0]];
                for (int i = 1; i < open.Count; i++)
                {
                    float c = cost[open[i]];
                    if (c < bestCost)
                    {
                        bestCost = c;
                        bestIdx = i;
                    }
                }

                GoapState current = open[bestIdx];
                open.RemoveAt(bestIdx);

                if (current.Satisfies(goalMask))
                    return Reconstruct(current, came, cost[current]);

                foreach (GoapActionBase a in actions)
                {
                    if (!a.CanApplyTo(current)) continue;

                    var next = a.ApplyTo(current);
                    float newCost = cost[current] + a.cost;

                    if (!cost.TryGetValue(next, out float old) ||
                            newCost < old)
                    {
                        cost[next] = newCost;
                        came[next] = new CameFrom { Prev = current, Action = a, HasPrev = true };

                        if (!open.Contains(next)) open.Add(next);
                    }
                }

            }
                return null;
        }
        private static GoapPlanResult Reconstruct(GoapState goalState, Dictionary<GoapState, CameFrom> came, float totalCost)
        {
            GoapPlanResult result = new GoapPlanResult { TotalCost = totalCost };

            GoapState current = goalState;
            while (came.TryGetValue(current, out var step) && step.HasPrev)
            {
                result.Actions.Add(step.Action);
                current = step.Prev;
            }

            result.Actions.Reverse();
            return result;
        }
    }


}
