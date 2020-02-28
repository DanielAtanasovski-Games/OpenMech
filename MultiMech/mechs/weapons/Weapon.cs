using Godot;
using System;

public class Weapon : Node2D
{
    [Export]
    public float AttackSpeed { get; set; }
    [Export]
    public int Damage { get; set; }
    [Export]
    public PackedScene Bullet { get; set; }

    // Attack speed
    private Timer attackTimer;
    private Position2D muzzle;
    private bool canFire = true;
    private Track track;
    // Animation
    private Timer animationReset;
    private float animationTime;
    private AnimatedSprite animation;
    

    public override void _Ready() {
        animation = (AnimatedSprite)GetNode("Animation");
        muzzle = (Position2D)GetNode("Muzzle");
        animationTime = AttackSpeed / 2;

        attackTimer = new Timer();
        attackTimer.Connect("timeout", this, nameof(AttackReset));
        AddChild(attackTimer);

        animationReset = new Timer();
        animationReset.Connect("timeout", this, nameof(AnimReset));
        AddChild(animationReset);

        track = (Track)GetParent();
    }

    public override void _Process(float delta) {
        GetInput();
    }

    public virtual void GetInput() {
        if (Input.IsActionPressed(InputHelper.SHOOT)){
            if (canFire) {
                canFire = false;
                Fire();

                // Attack Speed Timer
                attackTimer.Start(AttackSpeed);
                // Animation Timer
                animation.Frame = 1;
                animationReset.Start(animationTime);
            }
        }

    }

    public virtual void AnimReset() {
        animation.Frame = 0;
    }

    public virtual void AttackReset() {
        canFire = true;
    }

    public virtual void Fire() {
        Bullet bullet = (Bullet)Bullet.Instance();
        bullet.Start(muzzle.GlobalPosition, track.Rotation);
        GetTree().Root.AddChild(bullet);
    }
}
