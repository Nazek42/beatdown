using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackHandler : MonoBehaviour
{
    public BeatInput.Command p1Command;
    public BeatInput.Command p2Command;
    public double p1Offset;
    public double p2Offset;
    public AudioPlayer player;

    public Player leftPlayer;
    public Player rightPlayer;
    public FightController fightController;

    private bool p1Ready;
    private bool p2Ready;

    private List<NoteType> p1Notes;
    private List<NoteType> p2Notes;

    public int maxSync = 8;

    public Slider p1HealthBar;
    public Slider p2HealthBar;
    public Slider p1ShieldBar;
    public Slider p2ShieldBar;
    public SuperMeter p1SuperBar;
    public SuperMeter p2SuperBar;

    // Start is called before the first frame update
    void Start()
    {
        p1Command = BeatInput.Command.None;
        p2Command = BeatInput.Command.None;
        p1Ready = false;
        p2Ready = false;
        p1Offset = 0.0;
        p2Offset = 0.0;

        p1Notes = new List<NoteType>();
        p2Notes = new List<NoteType>();
    }
    // Update is called once per frame
    void Update()
    {
        p1HealthBar.value = leftPlayer.health / Player.max_health;
        p2HealthBar.value = rightPlayer.health / Player.max_health;
        p1ShieldBar.value = leftPlayer.block / Player.max_block;
        p2ShieldBar.value = rightPlayer.block / Player.max_block;
        p1SuperBar.meterValue = leftPlayer.combo_points;
        p2SuperBar.meterValue = rightPlayer.combo_points;
    }
    void Beat(double beatNum)
    {
        // Aggregate note types
        char[] p1ActualC = "    ".ToCharArray();
        foreach(NoteType note in p1Notes)
        {
            switch(note)
            {
                case NoteType.left:
                    p1ActualC[0] = '<';
                    break;
                case NoteType.up:
                    p1ActualC[1] = '^';
                    break;
                case NoteType.down:
                    p1ActualC[2] = 'v';
                    break;
                case NoteType.right:
                    p1ActualC[3] = '>';
                    break;
                default:
                    break;
            }
        }
        string p1Actual = new string(p1ActualC);
        // Aggregate note types
        char[] p2ActualC = "    ".ToCharArray();
        foreach (NoteType note in p2Notes)
        {
            switch (note)
            {
                case NoteType.left:
                    p2ActualC[0] = '<';
                    break;
                case NoteType.up:
                    p2ActualC[1] = '^';
                    break;
                case NoteType.down:
                    p2ActualC[2] = 'v';
                    break;
                case NoteType.right:
                    p2ActualC[3] = '>';
                    break;
                default:
                    break;
            }
        }
        string p2Actual = new string(p2ActualC);

        string p1Attempted = BeatInput.CommandToArrows(p1Command);
        string p2Attempted = BeatInput.CommandToArrows(p2Command);

        string p1Message;
        if (p1Actual == "    ")
        {
            p1Message = "   ";
        }
        else if (p1Attempted == p1Actual)
        {
            p1Message = "HIT";
            if(leftPlayer.combo_points < maxSync)
            {
                leftPlayer.combo_points += 1;
            }
        }
        else
        {
            p1Message = "MISS";
        }
        string p2Message;
        if(p2Actual == "    ")
        {
            p2Message = "   ";
        }
        else if (p2Attempted == p2Actual)
        {
            p2Message = "HIT";
            if (rightPlayer.combo_points < maxSync)
            {
                rightPlayer.combo_points += 1;
            }
        }
        else
        {
            p2Message = "MISS";
        }


        // GetComponent<TextMesh>().text = p1Attempted + p2Attempted + "\n" + p1Actual + p2Actual + "\n" + Mathf.Round(1000F*(float)p1Offset).ToString() + " " + Mathf.Round(1000F * (float)p2Offset).ToString();
        GetComponent<TextMesh>().text = p1Message + "   " + p2Message + "\n" + leftPlayer.combo_points + "    " + rightPlayer.combo_points;
        GetComponent<TextMesh>().text.Replace("\n", "\\n");

        leftPlayer.DetermineAction(p1Command);
        rightPlayer.DetermineAction(p2Command);


        p1Command = BeatInput.Command.None;
        p2Command = BeatInput.Command.None;
        p1Notes.Clear();
        p2Notes.Clear();
        
    }
    public void BeatReady(PlayerNum player)
    {
        switch (player)
        {
            case PlayerNum.Player1:
                p1Ready = true;
                break;
            case PlayerNum.Player2:
                p2Ready = true;
                break;
        }
        if(p1Ready && p2Ready)
        {
            Beat(0);
            p1Ready = false;
            p2Ready = false;
        }
    }

    public BeatInput.Command GetCommand(PlayerNum player)
    {
        switch (player)
        {
            case PlayerNum.Player1:
                return p1Command;
            case PlayerNum.Player2:
                return p2Command;
        }
        return p1Command;
    }

    public void SetCommand(PlayerNum player, BeatInput.Command Command)
    {
        switch(player)
        {
            case PlayerNum.Player1:
                p1Command = Command;
                break;
            case PlayerNum.Player2:
                p2Command = Command;
                break;
        }
    }
    public double GetOffset(PlayerNum player)
    {
        switch (player)
        {
            case PlayerNum.Player1:
                return p1Offset;
            case PlayerNum.Player2:
                return p2Offset;
        }
        return p1Offset;
    }
    public void SetOffset(PlayerNum player, double offset)
    {
        switch (player)
        {
            case PlayerNum.Player1:
                p1Offset = offset;
                break;
            case PlayerNum.Player2:
                p2Offset = offset;
                break;
        }
    }

    public void AddNote(PlayerNum player, NoteType noteType)
    {
        switch (player)
        {
            case PlayerNum.Player1:
                p1Notes.Add(noteType);
                break;
            case PlayerNum.Player2:
                p2Notes.Add(noteType);
                break;
        }
        
    }
    public enum PlayerNum
    {
        Player1,
        Player2
    }
}
