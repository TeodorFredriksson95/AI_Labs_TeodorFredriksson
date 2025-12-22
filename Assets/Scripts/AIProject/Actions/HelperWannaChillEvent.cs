using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/HelperWannaChillEvent")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "HelperWannaChillEvent", message: "Helper notifies that it wants to relax", category: "Events", id: "676f1963d511f9b1324ba44f82770f15")]
public sealed partial class HelperWannaChillEvent : EventChannel { }

