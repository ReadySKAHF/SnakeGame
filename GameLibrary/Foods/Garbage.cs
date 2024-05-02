using GameLibrary.Bonuses;
using GameLibrary.Players;
using GameLibrary.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GameLibrary.Foods;

public class Garbage : Food
{
    private int _snakeRemoveLength;

    public Garbage(int width, int height, Brush texture, Point position, int snakeRemoveLength) : base(width, height, texture, position)
    {
        _snakeRemoveLength = snakeRemoveLength;
    }

    public override void ApplyEffect(IPlayer player)
    {
        player = new RemoveLengthBonus(player, _snakeRemoveLength);    
    }

    public override Tile Clone()
    {
        return new Garbage(_width, _height, _texture, Position, _snakeRemoveLength);
    }
}
