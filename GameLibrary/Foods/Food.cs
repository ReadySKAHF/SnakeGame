using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using GameLibrary.Players;
using GameLibrary.Tiles;

namespace GameLibrary.Foods;

public abstract class Food : Tile
{
    public Food(int width, int height, Brush texture, Point position) : base(width, height, texture, position) { }

    public abstract void ApplyEffect(IPlayer player);
}
