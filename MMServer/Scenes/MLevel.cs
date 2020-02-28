using Godot;
using System.Collections.Generic;

public class MLevel : Node2D
{
    public List<Player> Players { get; set; }
    public Position2D SpawnPoints { get; set; }

    //TODO: Spawn Level specified in Level
    // AKA Instance a scene with the obstacles & Spawn Point
    public void AssignSpawns() {
        Node players = GetNode("Players");
        Node spawns = GetNode<Node2D>("SpawnPoints");
        for (int i = 0; i < players.GetChildCount(); i++)
        {
            players.GetChild<Node2D>(i).GlobalPosition = spawns.GetChild<Node2D>(i).GlobalPosition;
            players.GetChild<Node2D>(i).Rotation = spawns.GetChild<Node2D>(i).Rotation;
        }
    }

    
}
