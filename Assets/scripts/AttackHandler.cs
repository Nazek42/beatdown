using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHandler : MonoBehaviour
{
    public BeatInput.Attack p1Attack;
    public BeatInput.Attack p2Attack;
    public double p1Offset;
    public double p2Offset;
    public AudioPlayer player;

    private bool p1Ready;
    private bool p2Ready;

    private int p1Score = 0;
    private int p2Score = 0;

    private List<NoteType> p1Notes;
    private List<NoteType> p2Notes;

    // Start is called before the first frame update
    void Start()
    {
        p1Attack = BeatInput.Attack.None;
        p2Attack = BeatInput.Attack.None;
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

        string p1Attempted = BeatInput.AttackToArrows(p1Attack);
        string p2Attempted = BeatInput.AttackToArrows(p2Attack);

        string p1Message;
        if (p1Actual == "    ")
        {
            p1Message = "   ";
        }
        else if (p1Attempted == p1Actual)
        {
            p1Message = "HIT";
            p1Score += 1;
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
            p2Score += 1;
        }
        else
        {
            p2Message = "MISS";
        }

        // GetComponent<TextMesh>().text = p1Attempted + p2Attempted + "\n" + p1Actual + p2Actual + "\n" + Mathf.Round(1000F*(float)p1Offset).ToString() + " " + Mathf.Round(1000F * (float)p2Offset).ToString();
        GetComponent<TextMesh>().text = p1Message + "   " + p2Message + "\n" + p1Score + "    " + p2Score ;
        GetComponent<TextMesh>().text.Replace("\n", "\\n");

        p1Attack = BeatInput.Attack.None;
        p2Attack = BeatInput.Attack.None;
        p1Notes.Clear();
        p2Notes.Clear();
        
    }
    public void BeatReady(Player player)
    {
        switch (player)
        {
            case Player.Player1:
                p1Ready = true;
                break;
            case Player.Player2:
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

    public BeatInput.Attack GetAttack(Player player)
    {
        switch (player)
        {
            case Player.Player1:
                return p1Attack;
            case Player.Player2:
                return p2Attack;
        }
        return p1Attack;
    }

    public void SetAttack(Player player, BeatInput.Attack attack)
    {
        switch(player)
        {
            case Player.Player1:
                p1Attack = attack;
                break;
            case Player.Player2:
                p2Attack = attack;
                break;
        }
    }
    public double GetOffset(Player player)
    {
        switch (player)
        {
            case Player.Player1:
                return p1Offset;
            case Player.Player2:
                return p2Offset;
        }
        return p1Offset;
    }
    public void SetOffset(Player player, double offset)
    {
        switch (player)
        {
            case Player.Player1:
                p1Offset = offset;
                break;
            case Player.Player2:
                p2Offset = offset;
                break;
        }
    }

    public void AddNote(Player player, NoteType noteType)
    {
        switch (player)
        {
            case Player.Player1:
                p1Notes.Add(noteType);
                break;
            case Player.Player2:
                p2Notes.Add(noteType);
                break;
        }
        
    }
    public enum Player
    {
        Player1,
        Player2
    }
}
