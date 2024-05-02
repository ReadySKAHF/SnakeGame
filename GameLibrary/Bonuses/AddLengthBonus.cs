using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameLibrary.Players;

namespace GameLibrary.Bonuses
{
    public class AddLengthBonus : PlayerDecorator
    {
        public AddLengthBonus(IPlayer player, int length) 
        { 
            if (length < 0)
            {
                throw new Exception("Длина должна быть целым положительным числом");
            }

            _player = player;
            var tile = _player.Segments[^1];

            for(int i = 0; i < length; i++)
            {
                _player.Segments.Add(tile.Clone()); 
            }
        }

    }
}
