using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace THNeonMirage.Event
{
    public delegate void ValueChangedHandler(object sender, object oldValue, object newValue);

    public abstract class GameEventHandler
    {
    }
}