using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Time
{
    public class Timer
    {
        private List<GameEvent> _events;
        private Stopwatch _timer;
        public TimeSpan TimeEllapsed 
        { 
            get
            {
                return _timer.Elapsed;
            }
        }

        public Timer() 
        { 
            _events = new List<GameEvent>();
            _timer = new Stopwatch();   
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Reset()
        {
            _timer.Reset();
            _events.Clear();
        }

        public void Add(GameEvent e)
        {
            _events.Add(e);
        }

        public void Remove(GameEvent e)
        {
            _events.Remove(e);
        }

        public void CheckEvents()
        {
            for(int i = 0; i < _events.Count; i++) 
            {
                if (_events[i].TimeActivation <= TimeEllapsed)
                {
                    _events[i].Invoke();
                    Remove(_events[i]);
                    i--;
                }
            }
        }
    }
}
