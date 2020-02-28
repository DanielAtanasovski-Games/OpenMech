using Godot;
using System;

public class Bullet : KinematicBody2D
{
    [Export]
    public int Speed { get; set; }
    private Vector2 _velocity = new Vector2();

    public void Start(Vector2 pos, float dir)
    {
        Rotation = dir;
        Position = pos;
        _velocity = new Vector2(Speed, 0).Rotated(Rotation);
        Godot.GD.Print(Position + " Spawned");
    }

    public override void _PhysicsProcess(float delta)
    {
        KinematicCollision2D collsion = MoveAndCollide(_velocity * delta);
        if (collsion != null)
        {
            _velocity = _velocity.Bounce(collsion.Normal);
            if (collsion.Collider is IHittable)
            {
                IHittable hitObject = (IHittable)collsion.Collider;
                hitObject.Hit();
            }
        }
    }

    public void OnVisibilityNotifier2DScreenExited()
    {
        QueueFree();
    }
}
