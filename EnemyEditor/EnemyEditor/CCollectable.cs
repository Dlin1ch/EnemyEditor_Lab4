using Clicker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace EnemyEditor
{
    public abstract class CCollectable
    {
        protected Point position;
        protected double size;
        protected double lifetime;
        protected Ellipse sprite;

        public CCollectable(Point position, double size, double lifetime)
        {
            this.position = position;
            this.size = size;
            this.lifetime = lifetime;

            sprite = new Ellipse();
            sprite.Fill = Brushes.Gold;
            sprite.StrokeThickness = 2;
            sprite.Stroke = Brushes.Black;
            sprite.Width = this.size;
            sprite.Height = this.size;

            Canvas.SetLeft(sprite, position.X - size / 2);
            Canvas.SetTop(sprite, position.Y - size / 2);
        }

        public bool isMouseOnObject(Point mousePosition)
        {
            double left = position.X - size / 2;
            double top = position.Y - size / 2;
            double right = position.X + size / 2;
            double bottom = position.Y + size / 2;

            return mousePosition.X >= left && mousePosition.X <= right &&
                   mousePosition.Y >= top && mousePosition.Y <= bottom;
        }

        public Ellipse getSprite()
        {
            return sprite;
        }

        public bool updateLifetime(double delta)
        {
            lifetime -= delta;
            return lifetime <= 0;
        }

        public abstract bool onClick(Player player, Point mousePosition);
    }
}
