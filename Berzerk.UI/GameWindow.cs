using Berzerk.Logic.Entities.Enums;
using Berzerk.Logic.GameObjects;
using Berzerk.Logic.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Berzerk.UI
{
    public partial class GameWindow : Form
    {
        private Player Player;
        private List<Enemy> Enemies;

        private readonly IMapService _mapService;

        public int Level { get; set; } = 0;
        public Direction Direction { get; set; } = Direction.Unknown;

        public GameWindow(
            IMapService mapService
            )
        {
            InitializeComponent();
            _mapService = mapService;
        }

        private void GameWindow_Load(object sender, EventArgs e) => LoadNewLevel(true);

        private void Timer_Tick(object sender, EventArgs e)
        {
            Player.MovePlayer(Direction, this, out bool exitReached);
            if (exitReached)
                LoadNewLevel(true);

            Enemies.ForEach(enemy => enemy.MoveTowardsPlayer(Player.Location, this));

            var playerIsAlive = Controls.OfType<Player>().Any();
            if (!playerIsAlive)
            {
                ShowAlert(AlertType.Loss);
                Task.Delay(5000).Wait();
                LoadNewLevel(false);
            }
        }

        private void Screen_MouseDown(object sender, MouseEventArgs e)
        {
            var mousePosition = PointToClient(Cursor.Position);
            Player.ShootBullet(mousePosition, this);
        }

        private void GameWindow_KeyDown(object sender, KeyEventArgs e) => Direction = DetermineMovementDirection(e);

        private void InitializeControls()
        {
            Player = Controls.OfType<Player>().SingleOrDefault();
            if (Player is null)
                throw new NullReferenceException($"Player is missing!");

            Enemies = Controls.OfType<Enemy>().ToList();
            Enemies.ForEach(enemy => enemy.AddShootingBehavior(this));

            Controls.OfType<Control>().ToList().ForEach(x => x.MouseDown += Screen_MouseDown);
            this.MouseDown += Screen_MouseDown;
        }

        private void InitializeUserInterface()
        {
            var levelLabel = new Label();
            levelLabel.Text = $"LEVEL: {Level}";
            levelLabel.Dock = DockStyle.Bottom;
            levelLabel.Font = new Font("Algerian", 36F, FontStyle.Regular, GraphicsUnit.Point);
            levelLabel.ForeColor = Color.RoyalBlue;
            levelLabel.Size = new Size(296, 66);
            Controls.Add(levelLabel);
        }

        private Direction DetermineMovementDirection(KeyEventArgs keyEventArgs) => keyEventArgs.KeyCode switch
        {
            Keys.W => Direction.Up,
            Keys.S => Direction.Down,
            Keys.A => Direction.Left,
            Keys.D => Direction.Right,
            _ => Direction.Unknown
        };

        private void LoadNewLevel(bool levelPassed)
        {
            try
            {
                if (levelPassed)
                    Level++;

                Timer.Stop();

                if (_mapService.IsLastMap(Level))
                {
                    ShowAlert(AlertType.Win);
                    return;
                }

                this.Controls.Clear();
                Enemies?.ForEach(enemy => enemy.DisposeEnemy(this));

                _mapService.LoadMap(this, Level);
                InitializeControls();
                InitializeUserInterface();
                Timer.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occurred while loading the level! {ex.Message}");
                Application.Exit();
            }
        }

        private void ShowAlert(AlertType alertType)
        {
            var alertLabel = new Label();
            alertLabel.Text = alertType is AlertType.Win ? "YOU WIN!" : "YOU LOSE!";
            alertLabel.Font = new Font("Algerian", 80F, FontStyle.Regular, GraphicsUnit.Point);
            alertLabel.ForeColor = alertType is AlertType.Win ? Color.RoyalBlue : Color.Red;
            alertLabel.Size = ClientSize;
            alertLabel.TextAlign = ContentAlignment.MiddleCenter;
            alertLabel.Location = new Point((ClientSize.Width - alertLabel.Width) / 2, (ClientSize.Height - alertLabel.Height) / 2);
            Controls.Add(alertLabel);
            alertLabel.BringToFront();
        }
    }
}