using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GameLibrary.Tiles
{
    public class SnakeTile : Tile
    {
        public SnakeTile(int width, int height, Brush texture, Point position) : base(width, height, texture, position)
        {
        }

        public override Tile Clone()
        {
            return new SnakeTile(_width, _height, _texture, Position);
        }
    }
}
