using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GameLibrary.Tiles
{
    public class EmptyTile : Tile
    {
        public EmptyTile(int width, int height, Brush texture, Point position, Brush border, double borderThickness) : base(width, height, texture, position)
        {
            Rectangle.Stroke = border;
            Rectangle.StrokeThickness = borderThickness;
        }

        public override Tile Clone()
        {
            return new EmptyTile(_width, _height, _texture, Position, Rectangle.Stroke, Rectangle.StrokeThickness);
        }
    }
}
