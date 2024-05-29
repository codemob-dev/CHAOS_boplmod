using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHAOS
{
    public abstract class ChaosEvent
    {
        public bool Active { get; internal set; } = false;
        public int Id { get; internal set; }
        public ChaosEvent() { }
        public abstract string Enable();
        public abstract void Disable();
        public abstract void LevelStart();
    }
}
