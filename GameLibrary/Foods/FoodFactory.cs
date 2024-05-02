using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameLibrary.Tiles;

namespace GameLibrary.Foods;


public class FoodFactory
{
    private Random _random;
    private Food[] _foods;
    public TimeSpan Cooldown { get; private set; }

    public FoodFactory(Food[] foods, TimeSpan cooldown)
    {
        _random = new Random();
        _foods = foods;
        Cooldown = cooldown;
    }

    public Food CreateFood(Point position)
    {
        var randInd = _random.Next(0, _foods.Length);
        var food = (Food)_foods[randInd].Clone();
        food.Position = position;

        return food;
    }
}

