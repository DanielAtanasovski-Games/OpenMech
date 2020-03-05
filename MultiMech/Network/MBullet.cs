using Godot;
using System;

public class MBullet : KinematicBody2D
{
    public override void _Ready() {
        Godot.GD.Print(Name + GetPath());
    }

    [Remote]
    private void SetPos(Vector2 pos){
        GlobalPosition = pos;
    }

    [Remote]
    private void SetRot(float rot) {
        GlobalRotation = rot;
    }

    [Remote]
    private void Destroy() {
        QueueFree();
    }
}
