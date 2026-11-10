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
    public class CGoldGiver : CCollectable
    {
        private BigNumber goldValue;

        public CGoldGiver(Point position, double size, double lifetime, int playerLevel)
            : base(position, size, lifetime)
        {
            sprite.Fill = Brushes.Gold;
            goldValue = new BigNumber((playerLevel * 10).ToString());
        }

        public override bool onClick(Player player, Point mousePosition)
        {
            if (!isMouseOnObject(mousePosition)) return false;

            player.AddGold(goldValue);
            return true;
        }
    }
}
