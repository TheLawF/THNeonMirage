using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

namespace THNeonMirage
{
    public class GameMain : MonoBehaviour
    {
        public List<Action> Actions;

        public GameMain(List<Action> actions)
        {
            Actions = actions;
        }

        public void StartActions()
        {
            Actions.Add(() => {});
            // home input name -> login clicked -> show lobby -> add room clicked
        }
    }
}