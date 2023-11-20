using Berzerk.Logic.Entities.Enums;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Berzerk.Logic.GameObjects
{
    public class Enemy : PictureBox
    {
        private const string ENEMY_IMAGE_LOCATION = "C:\\Users\\gvida\\Documents\\Visual Studio 2022\\Projects\\Berzerk\\enemy.png";

        public Random ShootingInterval { get; set; } = new Random();
        public Timer ShootingTimer { get; set; }
        public int MovementSpeed { get; set; } = 3;

        public Enemy(Point location, Size size)
        {
            this.Location = location;
            this.Size = size;
            this.Image = Image.FromFile(ENEMY_IMAGE_LOCATION);
        }

        public void MoveTowardsPlayer(Point playerLocation, Form form)
        {
            var playerIsLeft = Location.X > playerLocation.X;
            var playerIsUp = Location.Y > playerLocation.Y;

            if (playerIsLeft && !WillCollide(Direction.Left, form))
                Left -= MovementSpeed;
            else if (!playerIsLeft && !WillCollide(Direction.Right, form))
                Left += MovementSpeed;

            if (playerIsUp && !WillCollide(Direction.Up, form))
                Top -= MovementSpeed;
            else if (!playerIsUp && !WillCollide(Direction.Down, form))
                Top += MovementSpeed;
        }

        public void AddShootingBehavior(Form form)
        {
            ShootingTimer = new Timer();
            ShootingTimer.Interval = ShootingInterval.Next(500, 2000);
            ShootingTimer.Tick += (sender, e) => ShootBullet(form);
            ShootingTimer.Start();
        }

        public void DisposeEnemy(Form form)
        {
            ShootingTimer?.Stop();
            form.Controls.Remove(this);
            Dispose();
        }

        private void ShootBullet(Form form)
        {
            var player = form.Controls.OfType<Player>().SingleOrDefault();
            if (player is null)
                return;

            var bullet = new Bullet(BulletType.Enemy, this.Location);
            form.Controls.Add(bullet);
            bullet.Fly(player.Location, form);
        }

        private bool WillCollide(Direction direction, Form form)
        {
            var newLocation = GetNewLocation(direction);
            var enemyInNewLocation = new Rectangle(newLocation, this.Size);

            var willCollide = form.Controls.OfType<PictureBox>()
                .Where(control => control != this && control is not Bullet)
                .Any(control => enemyInNewLocation.IntersectsWith(control.Bounds));

            return willCollide;
        }

        private Point GetNewLocation(Direction direction) => direction switch
        {
            Direction.Up => new Point(Location.X, Location.Y - MovementSpeed),
            Direction.Down => new Point(Location.X, Location.Y + MovementSpeed),
            Direction.Left => new Point(Location.X - MovementSpeed, Location.Y),
            Direction.Right => new Point(Location.X + MovementSpeed, Location.Y),
            _ => throw new ArgumentOutOfRangeException($"Unknown direction {direction}"!)
        };
    }
}