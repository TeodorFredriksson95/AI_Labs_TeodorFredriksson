using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/TRBWannaChillEvent")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "TRBWannaChillEvent", message: "TRB notifies that it wants to chill", category: "Events", id: "74834d3685bd64b475383563bf52964e")]
public sealed partial class TrbWannaChillEvent : EventChannel { }

