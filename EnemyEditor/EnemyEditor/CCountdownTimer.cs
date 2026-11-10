using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyEditor
{
    public class CCountdownTimer
    {
        private double targetTime;
        private double currentTime;

        public CCountdownTimer(double targetTime)
        {
            this.targetTime = targetTime;
            this.currentTime = 0;
        }

        public void update(double delta)
        {
            if (currentTime < targetTime)
            {
                currentTime += delta;
                if (currentTime > targetTime) currentTime = targetTime;
            }
        }

        public void reset()
        {
            currentTime = 0;
        }

        public bool isReady()
        {
            return currentTime >= targetTime;
        }
    }
}
