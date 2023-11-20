using Berzerk.Logic.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Berzerk.Logic.GameObjects
{
    public class Bullet : PictureBox
    {
        private const string PLAYER_BULLET_IMAGE_LOCATION = "C:\\Users\\gvida\\Documents\\Visual Studio 2022\\Projects\\Berzerk\\playerbullet.png";
        private const string ENEMY_BULLET_IMAGE_LOCATION = "C:\\Users\\gvida\\Documents\\Visual Studio 2022\\Projects\\Berzerk\\enemybullet.png";
        private const int BULLET_WIDTH = 5;
        private const int BULLET_HEIGHT = 5;

        public BulletType BulletType { get; set; }
        public int MovementSpeed { get; set; } = 8;

        public Bullet(BulletType type, Point location)
        {
            this.BulletType = type;
            this.Size = new Size(BULLET_WIDTH, BULLET_HEIGHT);
            this.Location = location;
            this.Image = BulletType is BulletType.Player ? Image.FromFile(PLAYER_BULLET_IMAGE_LOCATION) : Image.FromFile(ENEMY_BULLET_IMAGE_LOCATION);
        }

        public void Fly(Point towards, Form form)
        {
            var angle = Math.Atan2(towards.Y - this.Location.Y, towards.X - this.Location.X);
            var velocity = new Point((int)(MovementSpeed * Math.Cos(angle)), (int)(MovementSpeed * Math.Sin(angle)));

            var flyingTimer = new Timer();
            flyingTimer.Interval = 16;
            flyingTimer.Tick += (sender, e) =>
            {
                Left += velocity.X;
                Top += velocity.Y;
                if (DidCollide(form, out List<PictureBox> collisions))
                {
                    DisposeShotEntities(collisions, form);
                    DisposeBullet(flyingTimer, form);
                }
            };

            flyingTimer.Start();
        }

        private void DisposeShotEntities(List<PictureBox> collisions, Form form)
        {
            collisions.OfType<Enemy>().ToList().ForEach(enemy => enemy.DisposeEnemy(form));
            collisions.OfType<Player>().ToList().ForEach(player => player.DisposePlayer(form));
        }

        private bool DidCollide(Form form, out List<PictureBox> collisions)
        {
            collisions = form.Controls.OfType<PictureBox>().ToList();
            if (BulletType is BulletType.Player)
                collisions = collisions.Where(control => control is not Bullet && control is not Player && control.Bounds.IntersectsWith(this.Bounds)).ToList();
            else if (BulletType is BulletType.Enemy)
                collisions = collisions.Where(control => control is not Bullet && control is not Enemy && control.Bounds.IntersectsWith(this.Bounds)).ToList();

            return collisions.Any();
        }

        private void DisposeBullet(Timer flyingTimer, Form form)
        {
            flyingTimer.Stop();
            flyingTimer.Dispose();
            form.Controls.Remove(this);
            Dispose();
        }
    }
}