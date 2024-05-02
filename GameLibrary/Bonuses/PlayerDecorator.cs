using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameLibrary.Players;
using GameLibrary.Tiles;
using GameLibrary.Foods;

namespace GameLibrary.Bonuses
{
    public abstract class PlayerDecorator : IPlayer
    {
        protected IPlayer _player;

        public Direction Direction
        {
            get
            {
                return _player.Direction;
            }
            set
            {
                _player.Direction = value;
            }
        }

        public List<Tile> Segments
        {
            get
            {
                return _player.Segments;
            }
            set
            {
            }
        }

        public int Size => _player.Size;

        public bool IsAlive => _player.IsAlive;

        public void CheckCollision(IPlayer player)
        {
            _player.CheckCollision(player);
        }

        public void Die()
        {
            _player.Die();  
        }

        public void Move()
        {
            _player.Move();
        }

        public void TryToCollectFood(List<Food> foods)
        {
            _player.TryToCollectFood(foods);
        }
    }
}
