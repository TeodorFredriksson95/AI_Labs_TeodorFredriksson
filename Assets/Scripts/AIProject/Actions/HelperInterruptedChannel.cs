using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/HelperInterruptedChannel")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "HelperInterruptedChannel", message: "Helper notifies that it was interrupted", category: "Events", id: "ff9b369ad75ba1c6e6b35e5a9901b3ac")]
public sealed partial class HelperInterruptedChannel : EventChannel { }

