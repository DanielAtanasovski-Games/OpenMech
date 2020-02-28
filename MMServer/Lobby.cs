using Godot;
using static Godot.GD;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Lobby : Control
{
    private Server serverInstance;
    [Export]
    public PackedScene MLevelScene;
    [Export]
    public PackedScene MMechScene;

    // Start Game
    [Remote]
    public void OnStartGame()
    {
        // TODO: Pass all arguments of level to load, types of players to spawn
        Rpc("LoadGame");
    }

    [RemoteSync]
    public void LoadGame()
    {
        Godot.GD.Print("LoadGame");
        MLevel level = (MLevel)MLevelScene.Instance();
        GetTree().Root.AddChild(level);
        level.Players = new List<Player>(serverInstance.PlayerList);
        for (int i = 0; i < serverInstance.PlayerList.Count; i++)
        {
            MMech mech = (MMech)MMechScene.Instance();
            level.GetNode("Players").AddChild(mech);
            mech.Name = serverInstance.PlayerList[i].ID.ToString();
        }
        level.AssignSpawns();
    }


    // GUI
    private const string playerListHeader = "Players: \n";
    private Label playerList;

    public override void _Ready()
    {
        serverInstance = (Server)GetTree().Root.GetNode("Server");
        playerList = (Label)GetChild(0);
    }

    public void UpdatePlayerListGUI()
    {
        playerList.Text = playerListHeader;
        for (int i = 0; i < serverInstance.PlayerList.Count; i++)
        {
            playerList.Text = playerList.Text + serverInstance.PlayerList[i].PlayerName + "\n";
        }
    }

    public void SendPlayerList(int id)
    {
        string send = JsonConvert.SerializeObject(serverInstance.PlayerList);
        RpcId(id, "OnRecievePlayerList", send);
    }

    public void SendPlayerListUpdate(int id, Player player, bool removal)
    {
        string sendPlayer = JsonConvert.SerializeObject(player);
        RpcId(id, "OnRecievePlayerListUpdate", sendPlayer, removal);
    }

    [Remote]
    private void RequestPlayerList(int recievedFrom)
    {
        Print("Request from " + recievedFrom + " for PlayerList");
        SendPlayerList(recievedFrom);
    }


}
