using System;
using THNeonMirage.Manager;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace THNeonMirage.Event
{
    public delegate void ValueChangedHandler(object sender, object oldValue, object newValue);

    public delegate void SingleArgEventHandler(object arg);

    public delegate void NoParameterHandler();

    public delegate void GameEventHandler<in TArgs>(GameObject gameObject, TArgs args) where TArgs : IGameEventArgs;

    public delegate void ComponentEventHandler<in TArgs>(GameObject gameObject, MonoBehaviour script, TArgs args) where TArgs : IGameEventArgs;
    public delegate void ScriptEventHandler<in TArgs>(MonoBehaviour script, TArgs args) where TArgs : IGameEventArgs;
    public class RoundEventArgs : IGameEventArgs
    {
        private int round_activity;

        public PlayerManager PrevPlayer { get; }
        public PlayerManager NextPlayer { get; }

        public RoundEventArgs(PlayerManager prevPlayer, PlayerManager nextPlayer, int roundActivity)
        {
            PrevPlayer = prevPlayer;
            NextPlayer = nextPlayer;
            round_activity = roundActivity;
        }
    }

    public class ScriptComponentEventArgs : IGameEventArgs
    {
        public GameObject GameObject;

        public ScriptComponentEventArgs(GameObject gameObject)
        {
            GameObject = gameObject;
        }
        
    }
    public class ScriptEventArgs : IGameEventArgs
    {
        public MonoBehaviour Behaviour;

        public ScriptEventArgs(MonoBehaviour behaviour)
        {
            Behaviour = behaviour;
        }
    }
}