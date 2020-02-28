using Godot;
using System;

interface ITrack
{
    float RotationSpeed { get; set; }
    int ForwardSpeed { get; set; }
    int BackwardSpeed { get; set; }
}