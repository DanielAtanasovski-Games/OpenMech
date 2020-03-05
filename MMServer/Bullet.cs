using Godot;
using System;

public class Bullet : KinematicBody2D
{
    [Export]
    public int Speed { get; set; }
    private Vector2 _velocity = new Vector2();

    private Vector2 lastPos;
    private float lastRot;

    public MMech MechParent {get; set;}

    public override void _Ready() {
        Godot.GD.Print(Name + GetPath());
    }

    public void Start(Vector2 pos, float dir)
    {
        GlobalRotation = dir;
        GlobalPosition = pos;
        _velocity = new Vector2(Speed, 0).Rotated(Rotation);
        Godot.GD.Print(Position + " Spawned");

        lastRot = GlobalRotation;
        lastPos = GlobalPosition;
    }

    public void SetCollision(int layer) {
        CollisionLayer = (uint)layer;
    } 

    public override void _PhysicsProcess(float delta)
    {
        KinematicCollision2D collsion = MoveAndCollide(_velocity * delta);
        if (collsion != null)
        {
            _velocity = _velocity.Bounce(collsion.Normal);
            // if (collsion.Collider is IHittable)
            // {
            //     IHittable hitObject = (IHittable)collsion.Collider;
            //     hitObject.Hit();
            // }
        }
        if (GlobalPosition != lastPos)
            SetPos();

        if (GlobalRotation != lastRot)
            SetRot();
        
        lastPos = GlobalPosition;
        lastRot = GlobalRotation;
    }

    private void SetPos() {
        Rpc("SetPos", GlobalPosition);
    }

    private void SetRot() {
        Rpc("SetRot", GlobalRotation);
    }

    public void OnVisibilityNotifier2DScreenExited()
    {
        // TODO: Gonna change this to lifetime
        MechParent.RemoveBullet(this);
        Rpc("Destroy");
        QueueFree();
    }
}
