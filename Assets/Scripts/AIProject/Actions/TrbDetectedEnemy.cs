using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/TRBDetectedEnemy")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "TRBDetectedEnemy", message: "TRB notifies helper agent of [enemy] presence", category: "Events", id: "92887ea5a470aef7093af4ce6991b451")]
public sealed partial class TrbDetectedEnemy : EventChannel<GameObject> { }

