using Clicker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace EnemyEditor
{
    public class CDamageBooster : CCollectable
    {
        public CDamageBooster(Point position, double size, double lifetime)
            : base(position, size, lifetime)
        {
            sprite.Fill = Brushes.Red;
        }

        public override bool onClick(Player player, Point mousePosition)
        {
            if (!isMouseOnObject(mousePosition)) return false;

            player.ApplyDamageBoost(5.0); // 5 секунд баффа
            return true;
        }
    }
}
