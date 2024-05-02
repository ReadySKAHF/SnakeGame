using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;
using GameLibrary.Tiles;
using GameLibrary.Foods;
using GameLibrary.Bonuses;

namespace GameLibrary.Players
{
    public class Snake : IPlayer
    {
        public Direction Direction { get; set; }
        public List<Tile> Segments { get; set; }

        public bool IsAlive { get; set; }   

        public int Size { get {  return Segments.Count; } }

        private int _mapWidth;
        private int _mapHeight;


        public Snake(Direction direction, Point position, int length, Brush bodyBrush, Brush headBrush, int snakeTileWidth, int snakeTileHeight,
                     int mapWidth, int mapHeight)
        {
            if (length <= 0)
                throw new Exception("Размер змеи должен быть положительным целым числом");

            Direction = direction;
            Segments = new List<Tile>();
            Segments.Add(new SnakeTile(snakeTileWidth, snakeTileHeight, headBrush, position));

            for (int i = 1; i < length; i++)
            {
                Segments.Add(new SnakeTile(snakeTileWidth, snakeTileHeight, bodyBrush, position));
            }

            _mapWidth = mapWidth;
            _mapHeight = mapHeight;
            IsAlive = true;
        }

        public void Move()
        {

            for (int i = Size - 1; i > 0; i--)
            {
                Segments[i].Position = Segments[i - 1].Position.Clone();
            }

            switch (Direction)
            {
                case Direction.Up:
                    Segments[0].Position.Y = Segments[0].Position.Y == _mapHeight - 1 ? 0 : Segments[0].Position.Y + 1;
                    break;
                case Direction.Down:
                    Segments[0].Position.Y = Segments[0].Position.Y == 0 ? _mapHeight - 1 : Segments[0].Position.Y - 1;
                    break;
                case Direction.Left:
                    Segments[0].Position.X = Segments[0].Position.X == 0 ? _mapWidth - 1 : Segments[0].Position.X - 1;
                    break;
                case Direction.Right:
                    Segments[0].Position.X = Segments[0].Position.X == _mapWidth - 1 ? 0 : Segments[0].Position.X + 1;
                    break;
            }

            for(int i = 1; i < Size; i++)
            {
                if (Segments[0].Position == Segments[i].Position)
                    Die();
            }
        }

        public void CheckCollision(IPlayer player)
        {
            if (Segments[0].Position == player.Segments[0].Position)
            {
                this.Die();
                player.Die();
            }
            else if (Segments[0].Position == player.Segments[^1].Position)
            {
                this.Die();
            }
            else if (Segments[0].Position == player.Segments[1].Position)
            {
                player.Die();
            }
            else
            {
                for (int i = 2; i < player.Size - 1; i++)
                {
                    if (Segments[0].Position == player.Segments[i].Position)
                    {
                        int length = player.Size - i;

                        for (int j = 0; j < length; j++)
                        {
                            Segments.Add(Segments[^1].Clone());
                        }

                        for (int j = 0; j < length; j++)
                        {
                            player.Segments.Remove(player.Segments[^1]);
                        }
                    }
                }
            }
        }

        public void TryToCollectFood(List<Food> foods)
        {
            for(int i = 0; i < foods.Count; i++)
            {
                if (Segments[0].Position == foods[i].Position)
                {
                    foods[i].ApplyEffect(this);
                    foods.Remove(foods[i]);
                    i--;    
                }
            }   
        }

        public void Die()
        {
            IsAlive = false;
        }
    }
}
