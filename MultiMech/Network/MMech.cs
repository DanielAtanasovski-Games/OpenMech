using Godot;
using System;
using Newtonsoft.Json;

public class MMech : Node2D
{
    public int MovementVector { get; set; }
    public int TurnVector { get; set; }

    public bool Firing { get; set; }

    private int lastTurn = 0;
    private int lastMovement = 0;
    private bool lastFire = false;

    private MLevel levelInstance;

    // TODO: Specific bullets would be loaded based on what server says
    [Export]
    private PackedScene mBullet;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        levelInstance = (MLevel) GetParent().GetParent();
    }

    [Remote]
    public void SetPos(Vector2 position)
    {
        // Vector2 converted = JsonConvert.DeserializeObject<Vector2>(position);
        // GlobalPosition = converted;
        // Godot.GD.Print("GotPos: " + position);
        GlobalPosition = position;
    }

    [Remote]
    public void SetRot(float rotation)
    {
        // float converted = JsonConvert.DeserializeObject<float>(rotation);
        // Rotation = converted;
        // Godot.GD.Print("GotRot: " + rotation);
        GlobalRotation = rotation;
    }

    [Remote]
    private void SpawnBullet(Vector2 pos, float rot, string id) {
        MBullet bullet = (MBullet) mBullet.Instance();
        levelInstance.AddChild(bullet);
        bullet.Name = "bullet" + id;
        Godot.GD.Print(bullet.Name);
        bullet.Position = pos;
        bullet.Rotation = rot;
    }

    [Remote]
    private void Destroy() {
        QueueFree();
    }

    public override void _Input(InputEvent e)
    {
        if (Name != GetTree().NetworkPeer.GetUniqueId().ToString())
            return;

        TurnVector = 0;
        MovementVector = 0;
        Firing = false;

        if (Input.IsActionPressed(InputHelper.TURN_LEFT))
            TurnVector -= 1;
        if (Input.IsActionPressed(InputHelper.TURN_RIGHT))
            TurnVector += 1;

        // Setup Vector
        if (Input.IsActionPressed(InputHelper.MOVE_FORWARD))
            MovementVector += 1;
        if (Input.IsActionPressed(InputHelper.MOVE_BACKWARD))
            MovementVector -= 1;

        if (Input.IsActionPressed(InputHelper.SHOOT))
            Firing = true;

        // RpcId(1, "OnRecieveInput", MovementVector, TurnVector);
        // More Efficient
        if (TurnVector != lastTurn || MovementVector != lastMovement || Firing != lastFire )
        {
            RpcId(1, "OnRecieveInput", MovementVector, TurnVector, Firing);
            lastTurn = TurnVector;
            lastMovement = MovementVector;
            lastFire = Firing;
        }
    }
}
