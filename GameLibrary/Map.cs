using GameLibrary.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GameLibrary
{
    public class Map
    {
        public EmptyTile[,] Tiles;
        public int Rows;
        public int Columns;
        public int TileSize;
        public int MapWidth;
        public int MapHeight;

        public Map(int mapWidth, int mapHeight, int tileSize, Brush tileColor, Brush tileBorder, double borderThickness)
        {
            MapWidth = mapWidth;
            MapHeight = mapHeight;
            TileSize = tileSize;

            Rows = MapWidth / tileSize;
            Columns = MapHeight / tileSize;
            Tiles = new EmptyTile[Columns, Rows];

            for(int i = 0; i < Rows; i++)
            {
                for(int j = 0; j < Columns; j++)
                {
                    Tiles[i, j] = new EmptyTile(TileSize, TileSize, tileColor, new Point(i, j), tileBorder, borderThickness);
                }
            }
        }
    }
}
