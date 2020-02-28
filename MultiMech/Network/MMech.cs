using Godot;
using System;
using Newtonsoft.Json;

public class MMech : Node2D
{
    public int MovementVector { get; set; }
    public int TurnVector { get; set; }

    private int lastTurn = 0;
    private int lastMovement = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    [Remote]
    public void SetPos(Vector2 position) {
        // Vector2 converted = JsonConvert.DeserializeObject<Vector2>(position);
        // GlobalPosition = converted;
        Godot.GD.Print("GotPos: " + position);
        GlobalPosition = position;
    }

    [Remote]
    public void SetRot(float rotation) {
        // float converted = JsonConvert.DeserializeObject<float>(rotation);
        // Rotation = converted;
        Godot.GD.Print("GotRot: " + rotation);
        Rotation = rotation;
    }

    public override void _Input(InputEvent e) {
        if (Name != GetTree().NetworkPeer.GetUniqueId().ToString())
            return;
            
        TurnVector = 0;
        MovementVector = 0;

        if (Input.IsActionPressed(InputHelper.TURN_LEFT))
            TurnVector -= 1;
        if (Input.IsActionPressed(InputHelper.TURN_RIGHT))
            TurnVector += 1;

        // Setup Vector
        if (Input.IsActionPressed(InputHelper.MOVE_FORWARD))
            MovementVector += 1;
        if (Input.IsActionPressed(InputHelper.MOVE_BACKWARD))
            MovementVector -= 1;

        // RpcId(1, "OnRecieveInput", MovementVector, TurnVector);
        // More Efficient
        if (TurnVector != lastTurn || MovementVector != lastMovement){
            RpcId(1, "OnRecieveInput", MovementVector, TurnVector);
            lastTurn = TurnVector;
            lastMovement = MovementVector;
        }
    }
}
