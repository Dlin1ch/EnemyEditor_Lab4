using Clicker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyEditor
{
    public class CNormalEnemy : CEnemy
    {
        public CNormalEnemy(string name, string iconName, BigNumber baseLife, BigNumber gold)
    : base(name, iconName, baseLife, gold) { }
    }
}
