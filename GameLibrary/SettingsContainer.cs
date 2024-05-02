using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary
{
    public class SettingsContainer
    {
        public int SnakeLength { get; set; }

        public SettingsContainer(int snakeLength)
        {
            SnakeLength = snakeLength;
        }
    }
}
