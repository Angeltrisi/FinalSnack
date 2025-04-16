namespace FinalSnack.GameContent.Physics
{
    public readonly record struct Hit(bool IsHit, float Time, Vector2 Position);
    public readonly record struct Ray(Vector2 Position, Vector2 Magnitude);
}
