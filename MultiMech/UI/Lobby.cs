using Godot;
using System.Collections.Generic;
using static Godot.GD;
using Newtonsoft.Json;

public class Lobby : Control
{
    [Export]
    public PackedScene LobbyPlayerScene;
    [Export]
    public string playerListContainerDir;
    private BoxContainer playerListContainer;
    private List<Player> lobbyPlayers;
    private Button startButton;
    private bool IsHost;
    [Export]
    public PackedScene MLevelScene;
    [Export]
    public PackedScene MMechScene;

    public override void _Ready()
    {
        // Request Playerlist from server
        startButton = (Button)GetNode("Panel/Seperation/InfoPanel/Start");
        startButton.Connect("button_up", this, nameof(StartGame));
        RpcId(1, "RequestPlayerList", GetTree().NetworkPeer.GetUniqueId());
        playerListContainer = (BoxContainer)GetNode(playerListContainerDir);
    }

    // Network

    [Remote]
    private void OnRecievePlayerList(string playersString)
    {
        lobbyPlayers = JsonConvert.DeserializeObject<List<Player>>(playersString);
        SetupLobby();
    }

    [Remote]
    private void OnRecievePlayerListUpdate(string playerstr, bool removal)
    {
        Player player = JsonConvert.DeserializeObject<Player>(playerstr);
        if (removal)
        {
            int index = lobbyPlayers.FindIndex(x => x.ID == player.ID);
            lobbyPlayers.RemoveAt(index);
        }
        else
        {
            lobbyPlayers.Add(player);
        }
        UpdateLobby(player, removal);
    }

    // GUI

    private void SetupLobby()
    {
        for (int i = 0; i < lobbyPlayers.Count; i++)
        {
            LobbyPlayer p = (LobbyPlayer)LobbyPlayerScene.Instance();
            playerListContainer.AddChild(p);
            p.SetPlayerName(lobbyPlayers[i].PlayerName);
            p.ID = lobbyPlayers[i].ID;
            if (p.ID == GetTree().NetworkPeer.GetUniqueId())
            {
                p.SetMe(true);
                if (lobbyPlayers[i].Host)
                {
                    IsHost = true;
                    startButton.Disabled = false;
                }
            }

        }
        UpdateHost();
    }

    private void UpdateLobby(Player player, bool removal)
    {
        if (removal)
        {
            // Look for it in the GUI list
            for (int i = 0; i < playerListContainer.GetChildCount(); i++)
            {
                LobbyPlayer lp = (LobbyPlayer)playerListContainer.GetChild(i);
                if (lp.ID == player.ID)
                {
                    // Queue for deletion
                    lp.QueueFree();
                }
            }
        }
        else
        {
            // Add a new instance to gui list
            LobbyPlayer p = (LobbyPlayer)LobbyPlayerScene.Instance();
            playerListContainer.AddChild(p);
            p.SetPlayerName(player.PlayerName);
            p.ID = player.ID;
        }
    }

    private void UpdateHost()
    {
        for (int i = 0; i < playerListContainer.GetChildCount(); i++)
        {
            LobbyPlayer lp = (LobbyPlayer)playerListContainer.GetChild(i);
            lp.SetHost(lobbyPlayers[i].Host);
        }
    }

    // lobby
    private void StartGame()
    {
        // TODO: PASS Game settings here ? or on change of value in lobby?
        RpcId(1, "OnStartGame");
    }

    [Remote]
    public void LoadGame()
    {
        MLevel level = (MLevel)MLevelScene.Instance();
        GetTree().Root.AddChild(level);
        for (int i = 0; i < lobbyPlayers.Count; i++)
        {
            MMech mech = (MMech)MMechScene.Instance();
            level.GetNode("Players").AddChild(mech);
            mech.Name = lobbyPlayers[i].ID.ToString();
        }
        Visible = false;
    }
}
