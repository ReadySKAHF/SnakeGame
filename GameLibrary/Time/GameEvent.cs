using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Time
{
    public class GameEvent
    {
        private Action _action;
        public TimeSpan TimeActivation { get; private set; }

        public GameEvent(Action action, TimeSpan timeActivation)
        {
            _action = action;
            TimeActivation = timeActivation;
        }

        public void Invoke()
        {
            _action.Invoke();
        }
    }
}
