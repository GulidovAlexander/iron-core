namespace Game.Scripts.Core.Brains
{
    public interface IBrain
    {
        void Initialization();
        void Enable();
        void Disable();
        void Tick(float dt);
        bool IsEnabled { get; }
    }
}