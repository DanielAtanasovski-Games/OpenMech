using Godot;
using Godot.Collections;
using static Godot.GD;
using System.Collections.Generic;


public class Server : Node
{
     // References
    [Export]
    public PackedScene LobbyScene;
    private Lobby LobbyInstance;
    [Export]
    public PackedScene RemoteClient;

    // Server
    [Export]
    private int ServerPort = 4444;
    [Export]
    private int maxPlayers = 8;

    public List<Player> PlayerList { get; set; }
   
    public override void _Ready()
    {
        // Create Server
        NetworkedMultiplayerENet peer = new NetworkedMultiplayerENet();
        peer.CreateServer(ServerPort, maxPlayers);
        GetTree().NetworkPeer = peer;

        // Events
        GetTree().Connect("network_peer_connected", this, nameof(ClientConnected));
        GetTree().Connect("network_peer_disconnected", this, nameof(ClientDisconnected));

        PlayerList = new List<Player>();
        
        CallDeferred(nameof(PrepareLobby));
    }

    private void PrepareLobby() {
        // Create Lobby
        LobbyInstance = (Lobby)LobbyScene.Instance();
        GetTree().Root.AddChild(LobbyInstance);
    }

    private void ClientConnected(int id)
    {
        Print("Client " + id + " Connected to the Server");
        // var newClient = RemoteClient.Instance();
        // newClient.Name = (id.ToString());
        // GetTree().Root.AddChild(newClient);
        bool host = false;
        if (PlayerList.Count <= 0)
            host = true;

        // TODO: WHAT IF INGAME
        Player p = new Player(id, "Random" + id, host);
        for (int i = 0; i < PlayerList.Count; i++)
        {
            LobbyInstance.SendPlayerListUpdate(PlayerList[i].ID, p, host);
        }
        PlayerList.Add(p);        
        LobbyInstance.UpdatePlayerListGUI();
    }

    private void ClientDisconnected(int id)
    {
        Print("Client " + id + " Disconnected from the Server");
        int index = PlayerList.FindIndex(x => x.ID == id);
        Player p = PlayerList[index];
        PlayerList.RemoveAt(index);
        if (p.Host)
            Print("Host Quit");
            // Change to a different Host

        // TODO: WHAT IF INGAME
        MMech m = (MMech)LobbyInstance.LevelInstance.GetNode("Players/"+ p.ID.ToString());
        m.Destroy();
        // Inform Players of Client Disconnect
        for (int i = 0; i < PlayerList.Count; i++)
        {
            LobbyInstance.SendPlayerListUpdate(PlayerList[i].ID, p, true);
        }
        LobbyInstance.UpdatePlayerListGUI();
    }

    private void SendPlayerList(int id)
    {
        RpcId(id, "OnRecievePlayerList", PlayerList);
    }


    private void SendPlayerListUpdate(int id, Player player)
    {
        RpcId(id, "OnUpdatePlayerList", player);
    }




}
