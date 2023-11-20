using Berzerk.Logic.GameObjects;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Berzerk.Logic.Services
{
    public class MapService : IMapService
    {
        private const string MAP_FILES = "C:\\Users\\gvida\\Documents\\Visual Studio 2022\\Projects\\Berzerk\\maps\\map{0}.txt";
        private const string WALL_IMAGE_LOCATION = "C:\\Users\\gvida\\Documents\\Visual Studio 2022\\Projects\\Berzerk\\wall.png";
        private const string EXIT_IMAGE_LOCATION = "C:\\Users\\gvida\\Documents\\Visual Studio 2022\\Projects\\Berzerk\\exit.png";
        private const int TILE_WIDTH = 50;
        private const int TILE_HEIGHT = 50;

        public void LoadMap(Form form, int level)
        {
            var mapString = File.ReadAllText(string.Format(MAP_FILES, level));
            if (string.IsNullOrWhiteSpace(mapString))
                throw new FileNotFoundException("Failed to load the map from a file as it is empty!");

            var location = new Point(0, 0);
            foreach (var symbol in mapString)
            {
                if (symbol is '#')
                    SpawnWall(location, form);
                else if (symbol is 'X')
                    SpawnExit(location, form);
                else if (symbol is 'P')
                    SpawnPlayer(location, form);
                else if (symbol is 'E')
                    SpawnEnemy(location, form);

                location = GetNextLocation(symbol, location);
            }

            SetScreenSize(location, form);
        }

        public bool IsLastMap(int level) => !File.Exists(string.Format(MAP_FILES, level));

        private void SpawnWall(Point location, Form form)
        {
            var wall = new PictureBox();
            wall.Size = new Size(TILE_WIDTH, TILE_HEIGHT);
            wall.Location = location;
            wall.Image = Image.FromFile(WALL_IMAGE_LOCATION);
            form.Controls.Add(wall);
        }

        private void SpawnExit(Point location, Form form)
        {
            var exit = new PictureBox();
            exit.Size = new Size(TILE_WIDTH, TILE_HEIGHT);
            exit.Location = location;
            exit.Image = Image.FromFile(EXIT_IMAGE_LOCATION);
            exit.Tag = "exit";
            form.Controls.Add(exit);
        }

        private void SpawnPlayer(Point location, Form form)
        {
            var player = new Player(location, new Size(TILE_WIDTH, TILE_HEIGHT));
            form.Controls.Add(player);
        }

        private void SpawnEnemy(Point location, Form form)
        {
            var enemy = new Enemy(location, new Size(TILE_WIDTH, TILE_HEIGHT));
            form.Controls.Add(enemy);
        }

        private void SetScreenSize(Point lastLocation, Form form)
        {
            var xMapSize = lastLocation.X + (TILE_WIDTH * 3);
            var yMapSize = lastLocation.Y + (TILE_HEIGHT * 3);
            form.Size = new Size(xMapSize, yMapSize);
        }

        private Point GetNextLocation(char symbol, Point currentLocation) => symbol switch
        {
            '#' or ' ' or 'P' or 'E' or 'X' => new Point(currentLocation.X + TILE_WIDTH, currentLocation.Y),
            '\r' => new Point(currentLocation.X, currentLocation.Y),
            '\n' => new Point(currentLocation.X = 0, currentLocation.Y + TILE_HEIGHT),
            _ => throw new ArgumentOutOfRangeException($"Unexpected symbol {symbol} found inside the map file!")
        };
    }
}