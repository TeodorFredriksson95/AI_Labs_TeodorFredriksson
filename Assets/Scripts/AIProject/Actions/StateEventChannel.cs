using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/StateEventChannel")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "StateEventChannel", message: "Agent notifies of their [state]", category: "Events", id: "a45d55480ae8014b677222a84376dbc9")]
public sealed partial class StateEventChannel : EventChannel<States> { }

