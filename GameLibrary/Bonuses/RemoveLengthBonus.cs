using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameLibrary.Players;

namespace GameLibrary.Bonuses
{
    public class RemoveLengthBonus : PlayerDecorator
    {

        public RemoveLengthBonus(IPlayer player, int length) 
        { 
            if (length < 0)
            {
                throw new Exception("Длина должна быть целым положительным числом");
            }

            _player = player;

            for(int i = 0; i < length; i++)
            {
                if (_player.Size == 2)
                {
                    _player.Die();
                    break;
                }

                _player.Segments.RemoveAt(_player.Size - 1);    
            }
        }
    }
}
