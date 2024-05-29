using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    public abstract class OneTimeChaosEvent : ChaosEvent
    {
        private bool started = false;
        public OneTimeChaosEvent() { }
        public override sealed string Enable()
        {
            return Activate();
        }
        public override sealed void Disable()
        {
        }
        public sealed override void LevelStart()
        {
            if (started)
            {
                CHAOS.DeactivateSpecificChaos(Id);
            } else
            {
                started = true;
            }
        }
        public abstract string Activate();
    }
}
