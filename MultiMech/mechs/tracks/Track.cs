using Godot;
using System;

public class Track : KinematicBody2D
{

    
    public Vector2 MovementVector { get; set; }
    public int TurnVector { get; set; }

    [Export]
    public int RotationSpeed { get; set; }
    [Export]
    public int ForwardSpeed { get; set; }
    [Export]
    public int BackwardSpeed { get; set; }

    private bool moving = false;
    private AnimatedSprite animation;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        animation = (AnimatedSprite)GetChild(0);
    }

    private void GetInput() {
        TurnVector = 0;
        MovementVector = new Vector2();


        if (Input.IsActionPressed(InputHelper.TURN_LEFT))
            TurnVector -= 1;
        if (Input.IsActionPressed(InputHelper.TURN_RIGHT))
            TurnVector += 1;

        // Setup Vector
        if (Input.IsActionPressed(InputHelper.MOVE_FORWARD))
            MovementVector = new Vector2(ForwardSpeed, 0).Rotated(Rotation);
        if (Input.IsActionPressed(InputHelper.MOVE_BACKWARD))
            MovementVector = new Vector2(-ForwardSpeed, 0).Rotated(Rotation);

        // Normalize and set speed of movement
        MovementVector = MovementVector.Normalized() * ForwardSpeed;
    }

    public override void _PhysicsProcess(float delta) {
        GetInput();

        // Movement
        Move(delta);

        // Turning
        Turn(delta);
    }


    public virtual void Move(float delta)
    {
        if (MovementVector == Vector2.Zero)
            animation.Playing = false;
        else 
            animation.Playing = true;
        
        MovementVector = MoveAndSlide(MovementVector);
    }

    public virtual void Turn(float delta)
    {
        Rotation += TurnVector * RotationSpeed * delta;
    }


}
