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

    // Start is called before the first frame update
    void Start()
    {
        p1Attack = BeatInput.Attack.None;
        p2Attack = BeatInput.Attack.None;
        p1Ready = false;
        p2Ready = false;
        p1Offset = 0.0;
        p2Offset = 0.0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Beat(double beatNum)
    {
        GetComponent<TextMesh>().text = BeatInput.AttackToArrows(p1Attack) + BeatInput.AttackToArrows(p2Attack) + "\n" + Mathf.Round(1000F*(float)p1Offset).ToString() + " " + Mathf.Round(1000F * (float)p2Offset).ToString();
        GetComponent<TextMesh>().text.Replace("\n", "\\n");
        p1Attack = BeatInput.Attack.None;
        p2Attack = BeatInput.Attack.None;
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

    public enum Player
    {
        Player1,
        Player2
    }
}
