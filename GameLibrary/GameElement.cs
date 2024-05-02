using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GameLibrary.Players;
using GameLibrary.Foods;
using GameLibrary.Tiles;
using Timer = GameLibrary.Time.Timer;
using GameLibrary.Time;

namespace GameLibrary
{
    public partial class GameElement : UserControl
    {
        private readonly SolidColorBrush _snake1Brush = Brushes.Green;
        private readonly SolidColorBrush _snake2Brush = Brushes.Blue;
        public TextBlock _snake1ScoreTextBlock;
        public TextBlock _snake2ScoreTextBlock;
        private Canvas _gameField;

        private const int _tileSize = 30;
        private const int _mapWidth = 750;
        private const int _mapHeight = 750;
        private readonly SolidColorBrush _tileBrush = new(Color.FromRgb(128, 128, 128));
        private readonly SolidColorBrush _tileBorderBrush = new(Color.FromRgb(0, 0, 0));
        private const double _tileBorderThickness = 1;

        private const int _appleAddLength = 1;
        private const int _grapeAddLength = 2;
        private const int _garbageRemoveLength = 1;
        private readonly int _snakeLength;

        private Map _map;
        private IPlayer _snake1;
        private IPlayer _snake2;
        private List<Food> _foods;
        private FoodFactory _foodFactory;

        private readonly TimeSpan _tick = TimeSpan.FromMilliseconds(150);
        private readonly TimeSpan _foodGenerateCooldown = TimeSpan.FromMilliseconds(1000);
        private Timer _timer;

        public bool isRunning;
        private Action _actionAfterEndOfGame;

        public GameElement(Action actionAfterEndOfGame, SettingsContainer settingsContainer)
        {
            _actionAfterEndOfGame = actionAfterEndOfGame;
            _snakeLength = settingsContainer.SnakeLength;

            // Создание и настройка StackPanel
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            stackPanel.Margin = new Thickness(10);
            stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
            stackPanel.VerticalAlignment = VerticalAlignment.Center;

            // Создание и настройка TextBlock для змейки 1
            _snake1ScoreTextBlock = new TextBlock();
            _snake1ScoreTextBlock.Name = "_snake1ScoreTextBlock";
            _snake1ScoreTextBlock.FontSize = 24;
            _snake1ScoreTextBlock.Margin = new Thickness(10);

            // Создание и настройка TextBlock для змейки 2
            _snake2ScoreTextBlock = new TextBlock();
            _snake2ScoreTextBlock.Name = "snake2ScoreTextBlock";
            _snake2ScoreTextBlock.FontSize = 24;
            _snake2ScoreTextBlock.Margin = new Thickness(10);

            // Добавление TextBlock'ов в StackPanel
            stackPanel.Children.Add(_snake1ScoreTextBlock);
            stackPanel.Children.Add(_snake2ScoreTextBlock);

            // Создание и настройка Canvas
            _gameField = new Canvas();
            _gameField.Name = "canvas";
            _gameField.Background = Brushes.LightGray;
            _gameField.Width = _mapWidth;
            _gameField.Height = _mapHeight;

            // Создание Grid для размещения StackPanel и Canvas
            Grid grid = new Grid();
            grid.Children.Add(stackPanel);
            grid.Children.Add(_gameField);

            // Установка строк и столбцов для правильного размещения элементов в Grid
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            // Размещение StackPanel в первой строке Grid
            Grid.SetRow(stackPanel, 0);
            // Размещение Canvas во второй строке Grid
            Grid.SetRow(_gameField, 1);

            // Установка Content элемента UserControl в Grid
            Content = grid;

            Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Arrange(new Rect(0, 0, DesiredSize.Width, DesiredSize.Height));
            this.KeyDown += OnKeyDown;
        }

        private void OnLoad()
        {
            var foodTextureApple = new ImageBrush();
            foodTextureApple.ImageSource = new BitmapImage(new Uri("Resources\\Images\\apple.png", UriKind.Relative));

            var foodTextureGarbage = new ImageBrush();
            foodTextureGarbage.ImageSource = new BitmapImage(new Uri("Resources\\Images\\garbage.png", UriKind.Relative));

            var foodTextureGrape = new ImageBrush();
            foodTextureGrape.ImageSource = new BitmapImage(new Uri("Resources\\Images\\grape.png", UriKind.Relative));


            var _snake1HeadTexture = new ImageBrush();
            _snake1HeadTexture.ImageSource = new BitmapImage(new Uri("Resources\\Images\\greensnakehead.png", UriKind.Relative));

            var snake2HeadTexture = new ImageBrush();
            snake2HeadTexture.ImageSource = new BitmapImage(new Uri("Resources\\Images\\bluesnakehead.png", UriKind.Relative));

            _map = new Map(_mapWidth, _mapHeight, _tileSize, _tileBrush, _tileBorderBrush, _tileBorderThickness);

            _snake1 = new Snake(Direction.Right, new Point(0, 0), _snakeLength, _snake1Brush, _snake1HeadTexture, _tileSize, _tileSize, 
                _map.Rows, _map.Columns);
            _snake1.Segments[0].Rectangle.LayoutTransform = new RotateTransform(90);

            _snake2 = new Snake(Direction.Left, new Point(_map.Rows - 1, _map.Columns - 1), _snakeLength, _snake2Brush, snake2HeadTexture, _tileSize, _tileSize,
                _map.Rows, _map.Columns);
            _snake2.Segments[0].Rectangle.LayoutTransform = new RotateTransform(-90);

            _foods = new List<Food>();

            var foodForFactory = new Food[]
            {
                new Apple(_tileSize, _tileSize, foodTextureApple, new Point(0, 0), _appleAddLength),
                new Grape(_tileSize, _tileSize, foodTextureGrape, new Point(0, 0), _grapeAddLength),
                new Garbage(_tileSize, _tileSize, foodTextureGarbage, new Point(0, 0), _garbageRemoveLength)
            };

            _foodFactory = new FoodFactory(foodForFactory, _foodGenerateCooldown);
            _timer = new Timer();

            UpdateScores();
        }

        private void UpdateScores()
        {
            _snake1ScoreTextBlock.Text = $"Длина зелёной змейки: {_snake1.Size}";
            _snake2ScoreTextBlock.Text = $"Длина синей змейки: {_snake2.Size}";
        }

        private void OnUpdate()
        {
            _timer.CheckEvents();

            _snake1.Move();
            _snake2.Move();

            _snake1.CheckCollision(_snake2);
            _snake2.CheckCollision(_snake1);

            _snake1.TryToCollectFood(_foods);
            _snake2.TryToCollectFood(_foods);
        }

        private void GenerateFood()
        {
            if (_map.Rows * _map.Columns > (_foods.Count + _snake1.Size + _snake2.Size))
            {
                // Поиск свободного места на карте для еды
                var random = new Random();
                Point position;


                do
                {
                    var X = random.Next(0, _map.Rows);
                    var Y = random.Next(0, _map.Columns);

                    position = new Point(X, Y);
                } while (IsPlaceTaken(position, [_snake1, _snake2]));

                var gameEvent = new GameEvent(() => { _foods.Add(_foodFactory.CreateFood(position)); GenerateFood(); }, _timer.TimeEllapsed + _foodFactory.Cooldown);
                _timer.Add(gameEvent);
            }
        }

        private bool IsPlaceTaken(Point position, IPlayer[] players)
        {
            foreach(Food food in _foods)
            {
                if (food.Position == position) 
                    return true;
            }

            foreach(IPlayer player in players)
            {
                foreach(Tile tile in player.Segments)
                {
                    if (tile.Position == position) 
                        return true;
                }
            }

            return false;
        }

        private void OnRender()
        {
            UpdateScores();

            //// Отрисовка карты
            // Отчистка предыдущей сетки
            _gameField.Children.Clear();


            for (int row = 0; row < _map.Rows; row++)
            {
                for (int column = 0; column < _map.Columns; column++)
                {
                    var tile = _map.Tiles[row, column];

                    Canvas.SetLeft(tile.Rectangle, tile.Position.X * tile.Rectangle.Width);
                    Canvas.SetTop(tile.Rectangle, tile.Position.Y * tile.Rectangle.Height);

                    _gameField.Children.Add(tile.Rectangle);
                }
            }

            foreach(Food food in _foods)
            {
                Canvas.SetLeft(food.Rectangle, food.Position.X * food.Rectangle.Width);
                Canvas.SetTop(food.Rectangle, food.Position.Y * food.Rectangle.Height);

                _gameField.Children.Add(food.Rectangle);
            }

            for(int i = _snake1.Size - 1; i >= 0; i--)
            {
                Canvas.SetLeft(_snake1.Segments[i].Rectangle, _snake1.Segments[i].Position.X * _snake1.Segments[i].Rectangle.Width);
                Canvas.SetTop(_snake1.Segments[i].Rectangle, _snake1.Segments[i].Position.Y * _snake1.Segments[i].Rectangle.Height);

                _gameField.Children.Add(_snake1.Segments[i].Rectangle);
            }

            for (int i = _snake2.Size - 1; i >= 0; i--)
            {
                Canvas.SetLeft(_snake2.Segments[i].Rectangle, _snake2.Segments[i].Position.X * _snake2.Segments[i].Rectangle.Width);
                Canvas.SetTop(_snake2.Segments[i].Rectangle, _snake2.Segments[i].Position.Y * _snake2.Segments[i].Rectangle.Height);

                _gameField.Children.Add(_snake2.Segments[i].Rectangle);
            }
        }
        
        private void OnUnload()
        {
            _timer.Reset();
            _foods.Clear();
        }

        public async void Run()
        {
            OnLoad();
            _timer.Start();
            _timer.Add(new GameEvent(GenerateFood, TimeSpan.Zero));
            isRunning = true;

            while (isRunning)
            {
                OnUpdate();
                OnRender();
                
                if(!_snake1.IsAlive || !_snake2.IsAlive)
                    isRunning = false;

                await Task.Delay(_tick);
            }

            OnUnload();
            End();
           
        }

       

        protected void OnKeyDown(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.Key)
            {
                case Key.K:
                    if (_snake1.Direction != Direction.Down)
                    {
                        _snake1.Direction = Direction.Up;
                        _snake1.Segments[0].Rectangle.LayoutTransform = new RotateTransform(180);
                    }
                    break;
                case Key.I:
                    if (_snake1.Direction != Direction.Up)
                    {
                        _snake1.Direction = Direction.Down;
                        _snake1.Segments[0].Rectangle.LayoutTransform = new RotateTransform(0);
                    }
                    break;
                case Key.J:
                    if (_snake1.Direction != Direction.Right)
                    {
                        _snake1.Direction = Direction.Left;
                        _snake1.Segments[0].Rectangle.LayoutTransform = new RotateTransform(-90);
                    }
                    break;
                case Key.L:
                    if (_snake1.Direction != Direction.Left)
                    {
                        _snake1.Direction = Direction.Right;
                        _snake1.Segments[0].Rectangle.LayoutTransform = new RotateTransform(90);
                    }
                    break;
                case Key.S:
                    if (_snake2.Direction != Direction.Down)
                    {
                        _snake2.Direction = Direction.Up;
                        _snake2.Segments[0].Rectangle.LayoutTransform = new RotateTransform(180);
                    }
                    break;
                case Key.W:
                    if (_snake2.Direction != Direction.Up)
                    {
                        _snake2.Direction = Direction.Down;
                        _snake2.Segments[0].Rectangle.LayoutTransform = new RotateTransform(0);
                    }
                    break;
                case Key.A:
                    if (_snake2.Direction != Direction.Right)
                    {
                        _snake2.Direction = Direction.Left;
                        _snake2.Segments[0].Rectangle.LayoutTransform = new RotateTransform(-90);
                    }
                    break;
                case Key.D:
                    if (_snake2.Direction != Direction.Left)
                    {
                        _snake2.Direction = Direction.Right;
                        _snake2.Segments[0].Rectangle.LayoutTransform = new RotateTransform(90);
                    }
                    break;
            }
        }

        public void End()
        {
            string message;
            if (!_snake1.IsAlive && !_snake2.IsAlive)
            {
                message = "Игра окончена! Ничья!\n\nХотите сыграть ещё?";
            }
            else
            {
                if (_snake1.IsAlive)
                {
                    message = $"Игра окончена! Победила зелёная змейка.\n\nСчет:\nЗелёная змейка: {_snake1.Size}\nСиняя змейка: {_snake2.Size}\n\n" +
                        $"Хотите сыграть ещё?";
                }
                else
                {
                    message = $"Игра окончена! Победила синяя змейка.\n\nСчет:\nЗелёная змейка: {_snake1.Size}\nСиняя змейка: {_snake2.Size}\n\n" +
                        $"Хотите сыграть ещё?";
                }
            }

            MessageBoxResult result = MessageBox.Show(message, "", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Run();
            }
            else
            {
                Keyboard.ClearFocus();
                _actionAfterEndOfGame.Invoke();
            }
        }
    }
}
