using System;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GameLibrary.Tiles
{
    public abstract class Tile
    {
        protected int _width;
        protected int _height;
        protected Brush _texture;
        public Point Position { get; set; }

        public Rectangle Rectangle { get; set; }

        public Tile(int width, int height, Brush texture, Point position)
        {
            _width = width;
            _height = height;
            _texture = texture;

            Rectangle = new Rectangle()
            {
                Width = width,
                Height = height,
                Fill = texture
            };
            Position = position;
        }

        public abstract Tile Clone();
    }
}
