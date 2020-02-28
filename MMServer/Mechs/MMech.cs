using Godot;
using System;

public class MMech : KinematicBody2D
{
    private bool canMove;

    public Vector2 MovementVector { get; set; }

    private int recievedMovement;
    private int recievedTurn;

    private Vector2 lastPosition;
    private float lastRotation;

    // TODO: Replace this with json of stats
    private int ForwardSpeed = 100;
    private int RotationSpeed = 5;


    public override void _Ready()
    {
        lastPosition = GlobalPosition;
        lastRotation = Rotation;
    }

    [Remote]
    public void OnRecieveInput(int movement, int turn)
    {
        recievedTurn = turn;
        recievedMovement = movement;    
    }

    public override void _PhysicsProcess(float delta)
    {
        Move(delta);
        Turn(delta);
    }

    public void Move(float delta)
    {
        // if (MovementVector == Vector2.Zero)
        //     animation.Playing = false;
        // else
        //     animation.Playing = true;
        // TODO: Check if going backwards
        MovementVector = new Vector2(ForwardSpeed * recievedMovement, 0).Rotated(Rotation);
        MovementVector = MovementVector.Normalized() * ForwardSpeed;

        MovementVector = MoveAndSlide(MovementVector);
        if (lastPosition != Position){
            SetPos(GlobalPosition);
        }
    }

    public void SetPos(Vector2 pos) {
        GlobalPosition = pos;
        Rpc("SetPos", pos);
    }

    public void Turn(float delta)
    {
        Rotation += recievedTurn * RotationSpeed * delta;
        if (lastRotation != Rotation){
            SetRot(Rotation);
        }
    }

    public void SetRot(float rot) {
        Rotation = rot;
        Rpc("SetRot", rot);
    }
}
