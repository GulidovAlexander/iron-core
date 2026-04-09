namespace Game.Scripts.Core.Brains
{
    public abstract class Brain : IBrain
    {
        public bool IsEnabled { get; private set; }

        public virtual void Initialization() { }
        public virtual void Enable() => IsEnabled = true;
        public virtual void Disable() => IsEnabled = false;

        public void Tick(float dt)
        {
            if (!IsEnabled) return;
            UpdateLogic(dt);
        }

        protected abstract void UpdateLogic(float dt);
    }
}