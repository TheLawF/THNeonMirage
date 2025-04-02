using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace THNeonMirage
{
    public class GameProcess
    {
        public List<Action> Actions;

        public GameProcess(List<Action> actions)
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