using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Clear Target", story: "[Agent] clears [target]", category: "Action/Sensing", id: "150a2a17efff954cf0312bab03c23fa2")]
public partial class ClearTargetAction : Action
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Clear Target",
        description: "Clears Target and resets LOS memory.",
        story: "Forget the target and reset perception flags.",
        category: "Action/Sensing",
        id: "d1b6d9f2-7c9b-4e7b-a8e5-4c4ff1c2a4dd"
        )]
    public class ClearTarget : Action
    {
        [SerializeReference]
        public BlackboardVariable<GameObject>
        Target;
        [SerializeReference]
        public BlackboardVariable<bool>
        HasLineOfSight;
        [SerializeReference]
        public BlackboardVariable<float>
        TimeSinceLastSeen;
        protected override Status OnUpdate()
        {
            if (Target != null) Target.Value = null;
            if (HasLineOfSight != null) HasLineOfSight.Value = false;
            if (TimeSinceLastSeen != null) TimeSinceLastSeen.Value = 9999f;
            return Status.Success;
        }
    }


}

