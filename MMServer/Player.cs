using System;

public class Player
{
    public int ID { get; set; }
    public string PlayerName { get; set; }
    public bool Host = false;

    public Player() {

    }

    public Player(int ID, string PlayerName, bool Host = false) {
        this.ID = ID;
        this.PlayerName = PlayerName;
        this.Host = Host;
    }

    public void SetInfo(int ID, string PlayerName, bool Host = false) {
        this.ID = ID;
        this.PlayerName = PlayerName;
        this.Host = Host;
    }

}
