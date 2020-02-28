using Godot;
using System;

public class StandardTrack : Node, ITrack
{
    public int ForwardSpeed { get; set; }
    public int BackwardSpeed { get; set; }
    public float RotationSpeed { get; set; }

}
