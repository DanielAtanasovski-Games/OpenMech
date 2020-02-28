using Godot;
using System;

public class LobbyPlayer : PanelContainer
{
    private string playerName;
    private bool me = false;

    public int ID { get; set; }
    private Label playerNameLabel { get; set; }
    private Label playerHost { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        playerNameLabel = (Label)GetNode("Container/Name");
        playerHost = (Label)GetNode("Container/Host");
    }

    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;
        playerNameLabel.Text = this.playerName;
        if (me)
            playerNameLabel.Text = this.playerName + "    (You)";
    }

    public void SetMe(bool val)
    {
        me = val;
        if (me)
        {
            SelfModulate = new Color(0.6F, 0.6F, 1);
            playerNameLabel.Text = this.playerName + "    (You)";
        }
    }

    public void SetHost(bool value) {
        playerHost.Visible = value;
    }

}
