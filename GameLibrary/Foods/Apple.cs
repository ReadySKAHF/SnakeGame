using GameLibrary.Bonuses;
using GameLibrary.Players;
using GameLibrary.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GameLibrary.Foods
{
    public class Apple : Food
    {
        private int _snakeAddLength;

        public Apple(int width, int height, Brush texture, Point position, int snakeAddLength) : base(width, height, texture, position)
        {
            _snakeAddLength = snakeAddLength;   
        }

        public override void ApplyEffect(IPlayer player)
        {
            player = new AddLengthBonus(player, _snakeAddLength);   
        }

        public override Tile Clone()
        {
            return new Apple(_width, _height, _texture, Position, _snakeAddLength);
        }
    }
}
