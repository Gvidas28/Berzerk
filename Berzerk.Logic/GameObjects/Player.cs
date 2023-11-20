using System.Windows.Forms;
using System.Drawing;
using Berzerk.Logic.Entities.Enums;
using System;
using System.Linq;

namespace Berzerk.Logic.GameObjects
{
    public class Player : PictureBox
    {
        private const string PLAYER_IMAGE_LOCATION = "C:\\Users\\gvida\\Documents\\Visual Studio 2022\\Projects\\Berzerk\\player.png";

        public int MovementSpeed { get; set; } = 7;

        public Player(Point location, Size size)
        {
            this.Location = location;
            this.Size = size;
            this.Image = Image.FromFile(PLAYER_IMAGE_LOCATION);
        }

        public void MovePlayer(Direction direction, Form form, out bool exitReached)
        {
            exitReached = false;
            if (direction is not Direction.Unknown && !WillCollide(direction, form, out Point newLocation, out exitReached))
                Location = newLocation;
        }

        public void ShootBullet(Point mousePosition, Form form)
        {
            var playerExists = form.Controls.OfType<Player>().Any();
            if (!playerExists)
                return;

            var bullet = new Bullet(BulletType.Player, this.Location);
            form.Controls.Add(bullet);
            bullet.Fly(mousePosition, form);
        }
        public void DisposePlayer(Form form)
        {
            form.Controls.Remove(this);
            Dispose();
        }

        private bool WillCollide(Direction direction, Form form, out Point newLocation, out bool exitReached)
        {
            newLocation = GetNewLocation(direction);
            var playerInNewLocation = new Rectangle(newLocation, this.Size);

            var willCollide = form.Controls.OfType<PictureBox>()
                .Where(control => control is not Bullet && control is not Player)
                .Any(control => playerInNewLocation.IntersectsWith(control.Bounds));

            exitReached = form.Controls.OfType<PictureBox>()
                .Where(control => control.Tag is "exit")
                .Any(control => playerInNewLocation.IntersectsWith(control.Bounds));

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