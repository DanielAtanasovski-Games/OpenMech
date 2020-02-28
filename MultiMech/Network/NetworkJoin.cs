using Godot;
using System;


public class NetworkJoin : Control
{
    [Export]
    private int ServerPort = 4444;
    [Export]
    private string ServerAddress = "127.0.0.1";

    private LineEdit serverInput;
    private Button serverConfirm;

    [Export]
    public PackedScene LobbyScene;

    public override void _Ready()
    {
        serverInput = (LineEdit)GetNode("Panel/ServerInput");
        serverConfirm = (Button)GetNode("Panel/ServerConfirm");
        serverConfirm.Connect("button_up", this, nameof(ButtonPressed));
        serverConfirm.Disabled = false;
    }

    private void CreateClient() {
        NetworkedMultiplayerENet peer = new NetworkedMultiplayerENet();
        peer.CreateClient(ServerAddress, ServerPort);
        GetTree().NetworkPeer = peer;
        peer.Connect("connection_failed", this, nameof(OnConnectionFailed));
        peer.Connect("connection_succeeded", this, nameof(OnConnectionSuccess));
    }

    private void ButtonPressed() {
        serverConfirm.Disabled = true;
        ServerAddress = serverInput.Text;
        CreateClient();
    }

    private void OnConnectionFailed() {
        serverConfirm.Disabled = false;
        Godot.GD.Print("Failed!");
    }

    private void OnConnectionSuccess() {
        Godot.GD.Print("Succeeded!");
        // Create Lobby Client Side
        PrepareLobby();
    }

    private void PrepareLobby() {
        // Create Lobby
        Lobby l = (Lobby)LobbyScene.Instance();
        GetTree().Root.AddChild(l);
        GetTree().Root.RemoveChild(this);
        QueueFree();
    }

}
