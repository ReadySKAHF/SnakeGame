using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameLibrary.Tiles;
using GameLibrary.Foods;

namespace GameLibrary.Players
{
    public interface IPlayer
    {
        public abstract Direction Direction { get; set; }

        public abstract List<Tile> Segments { get; protected set; }

        public abstract int Size { get; }

        public bool IsAlive { get; }

        public void Move();

        public void CheckCollision(IPlayer player);

        public void TryToCollectFood(List<Food> foods);

        public void Die();

    }
}
