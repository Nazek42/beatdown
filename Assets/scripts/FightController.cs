using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightController : MonoBehaviour {

    // Player variables
    public Player left_player;
    public Player right_player;
   

    // Scoring variables
    private short winner = 0;
    private bool gameover = false;

    // Outcome table for the states given each player
    // Row index corresponds to the left player's state, and 
    static private Outcome[,] InRangeOutcomes = new Outcome[11,11]
    {          /*0*/          /*1*/          /*2*/          /*3*/          /*4*/          /*5*/          /*6*/          /*7*/          /*8*/          /*9*/          /*10*/
/*0*/     { Outcome.Noop , Outcome.Noop , Outcome.Dodge, Outcome.Block, Outcome.Block, Outcome.Block, Outcome.Block, Outcome.Block, Outcome.Hit  , Outcome.Hit  , Outcome.Noop  },
/*1*/     { Outcome.Noop , Outcome.Noop , Outcome.Dodge, Outcome.Block, Outcome.Block, Outcome.Block, Outcome.Block, Outcome.Block, Outcome.Hit  , Outcome.Hit  , Outcome.Noop  },
/*2*/     { Outcome.Dodge, Outcome.Dodge, Outcome.Dodge, Outcome.Dodge, Outcome.Hit  , Outcome.Dodge, Outcome.Dodge, Outcome.Dodge, Outcome.Dodge, Outcome.Dodge, Outcome.Dodge },
/*3*/     { Outcome.Block, Outcome.Block, Outcome.Dodge, Outcome.Clash, Outcome.Hit  , Outcome.Hit  , Outcome.Hit  , Outcome.Hit  , Outcome.Hit  , Outcome.Hit  , Outcome.Hit   },
/*4*/     { Outcome.Block, Outcome.Block, Outcome.Hit  , Outcome.Hit  , Outcome.Clash, Outcome.Hit  , Outcome.Hit  , Outcome.Hit  , Outcome.Hit  , Outcome.Hit  , Outcome.Hit   },
/*5*/     { Outcome.Block, Outcome.Block, Outcome.Dodge, Outcome.Block, Outcome.Block, Outcome.Clash, Outcome.Block, Outcome.Block, Outcome.Hit  , Outcome.Hit  , Outcome.Hit   },
/*6*/     { Outcome.Block, Outcome.Hit  , Outcome.Dodge, Outcome.Block, Outcome.Block, Outcome.Block, Outcome.Clash, Outcome.Block, Outcome.Hit  , Outcome.Hit  , Outcome.Hit   },
/*7*/     { Outcome.Block, Outcome.Hit  , Outcome.Dodge, Outcome.Block, Outcome.Block, Outcome.Block, Outcome.Block, Outcome.Clash, Outcome.Hit  , Outcome.Hit  , Outcome.Hit   },
/*8*/     { Outcome.Hit  , Outcome.Block, Outcome.Dodge, Outcome.Block, Outcome.Block, Outcome.Block, Outcome.Block, Outcome.Block, Outcome.Clash, Outcome.Hit  , Outcome.Hit   },
/*9*/     { Outcome.Hit  , Outcome.Block, Outcome.Dodge, Outcome.Block, Outcome.Block, Outcome.Block, Outcome.Block, Outcome.Block, Outcome.Hit  , Outcome.Clash, Outcome.Hit   },
/*10*/    { Outcome.Noop , Outcome.Noop , Outcome.Dodge, Outcome.Hit  , Outcome.Hit  , Outcome.Hit  , Outcome.Hit  , Outcome.Hit  , Outcome.Hit  , Outcome.Hit  , Outcome.Hit   }
    };

    // Table of base damage for each attack
    static private float[] BaseDmgTable = new float[11]
    {
        0.0f,       /* High Block */
        0.0f,       /* Low Block */
        0.0f,       /* Backdash */
        2.0f,       /* Jab */
        2.0f,       /* Projectile */
        2.0f,       /* Body Check */
        4.0f,       /* Uppercut */
        4.0f,       /* High Punch */
        4.0f,       /* Spin Kick */
        2.0f,       /* Back Kick */
        0.0f        /* Noop */
    };

    // Combo point damage multiplier for uppercuts and projectiles
    static private float combo_mult = 0.125f;

    // Function called every beat
    void Beat(double b)
    {
        // Determine each player's move
        Player.Attack right_act = right_player.GetAction();
        Player.Attack left_act = left_player.GetAction();

        // Get player distances from center
        short right_dist = right_player.GetDistance();
        short left_dist = left_player.GetDistance();

        // Based on move, change the distance for each player
        // Right player distance
        if (right_act == Player.Attack.Backdash)
            right_dist = right_player.ModDistance(2);
        else if (right_act == Player.Attack.BodyCheck)
            right_dist = right_player.ModDistance(-2);
        else
            right_dist = right_player.ModDistance(-1);

        // Left player distance
        if (left_act == Player.Attack.Backdash)
            left_dist = left_player.ModDistance(2);
        else if (left_act == Player.Attack.BodyCheck)
            left_dist = left_player.ModDistance(-2);
        else
            left_dist = left_player.ModDistance(-1);
        // Determine outcome based on each player's move
        // If players are not in melee range of each other, then they can only land projectile hits
        if (right_dist != 0 || left_dist != 0)
        {
            // If both players fired projectiles, then the projectiles clash
            if (right_act == Player.Attack.Projectile && left_act == Player.Attack.Projectile) Clash(left_act, right_act);

            // If only one player fired a projectile, then there is no clash to be made
            else if (right_act == Player.Attack.Projectile || left_act == Player.Attack.Projectile) Hit(left_act, right_act);

            // If neither player fired projectiles, they are not in range to do anything meaningful
            else Noop();
        }

        // If both players are in melee range of each other
        else
        {
            // Refer to InRangeTable for results
            switch(InRangeOutcomes[(int)(left_act), (int)(right_act)])
            {
                case (Outcome.Noop):
                    Noop();
                    break;

                case (Outcome.Dodge):
                    Noop();
                    break;

                case (Outcome.Hit):
                    Hit(left_act, right_act);
                    break;

                case (Outcome.Clash):
                    Clash(left_act, right_act);
                    break;

                case (Outcome.Block):
                    Block(left_act, right_act);
                    break;

            }
        }

        // Clear combo points for any movement that did not sync correctly

        // Check if game is over
        // Case 1: One player dies
        if (left_player.GetHealth() == 0)
        {
            winner += 1;
            gameover = true;
        }

        if(right_player.GetHealth() == 0)
        {
            winner -= 1;
            gameover = true;
        }

        // Case 2: Song ends
        // TO BE INCORPORATED WITH BEAT/SIMFILE CONTENT

        if (gameover)
        {
            switch(winner)
            {
                case (-1):
                    // Left wins
                    break;
                case (0):
                    // Tie
                    break;
                case (1):
                    // Right wins
                    break;
            }
        }
    }

    // OUTCOME FUNCTIONS
    private void Noop()
    {
        return;
    }

    private void Hit(Player.Attack left_act, Player.Attack right_act)
    {
        float left_damage;
        float right_damage;

        // Calculate right player damage
        if (right_act == Player.Attack.Projectile || right_act == Player.Attack.Uppercut)
            right_damage = BaseDmgTable[(int)right_act] * (1.0f + right_player.GetComboPoints() * combo_mult);

        else right_damage = BaseDmgTable[(int)right_act];

        // Calculate left player damage
        if (left_act == Player.Attack.Projectile || left_act == Player.Attack.Uppercut)
            left_damage = BaseDmgTable[(int)left_act] * (1.0f + left_player.GetComboPoints() * combo_mult);

        else left_damage = BaseDmgTable[(int)left_act];
        // Apply damage calculations
        left_player.ModHealth(-right_damage);
        right_player.ModHealth(-left_damage);

        return;
    }

    private void Clash(Player.Attack left_act, Player.Attack right_act)
    {
        if(left_act == Player.Attack.Projectile)
        {
            // Calculate player damage (both known projectiles)
            float right_damage = BaseDmgTable[(int)right_act] * (1.0f + right_player.GetComboPoints() * combo_mult);
            float left_damage = BaseDmgTable[(int)left_act] * (1.0f + left_player.GetComboPoints() * combo_mult);

            float tide = right_damage - left_damage;

            // If left overpowers
            if (tide < 0)
                right_player.ModHealth(tide);
            else if (tide > 0)
                left_player.ModHealth(-tide);

        }

        // Players neutralize each other's damage (no op)

        return;
    }

    private void Block(Player.Attack left_act, Player.Attack right_act)
    {
        // Left is the blocker 
        if(left_act == Player.Attack.HighBlock || left_act == Player.Attack.LowBlock)
        {
            float block_val = left_player.GetBlock();


            float antag_dmg;
            // Calculate right player damage
            if (right_act == Player.Attack.Projectile || right_act == Player.Attack.Uppercut)
                antag_dmg = BaseDmgTable[(int)right_act] * (1.0f + right_player.GetComboPoints() * combo_mult);

            else antag_dmg = BaseDmgTable[(int)right_act];

            // Take remaining block value after hit
            float remainder = block_val - antag_dmg;

            // If damage exceeds block value
            if(remainder < 0.0f)
            {
                left_player.ModBlock(-block_val);
                left_player.ModHealth(remainder);
            }

            // If damage does not exceed block value
            else left_player.ModBlock(-remainder);

        }

        // Right is the blocker
        else
        {
            float block_val = right_player.GetBlock();

            float antag_dmg;
            // Calculate right player damage
            if (left_act == Player.Attack.Projectile || left_act == Player.Attack.Uppercut)
                antag_dmg = BaseDmgTable[(int)left_act] * (1.0f + left_player.GetComboPoints() * combo_mult);

            else antag_dmg = BaseDmgTable[(int)left_act];

            // Take remaining block value after hit
            float remainder = block_val - antag_dmg;

            // If damage exceeds block value
            if (remainder < 0.0f)
            {
                right_player.ModBlock(-block_val);
                right_player.ModHealth(remainder);
            }

            // If damage does not exceed block value
            else right_player.ModBlock(-remainder);
        }

        return;
    }

    // Use this for initialization
    void Start ()
    {
        // Sets orientation of players
        left_player.SetOrientation(true);
        right_player.SetOrientation(false);

        GetComponent<AudioPlayer>().onBeat += Beat;
	}
	
	// Update is called once per frame
	/*void Update () {
		
	}*/

    public enum Outcome
    {
        Noop,        // NoOp case:
        Dodge,       // Dodge case:
        Hit,         // Hit Check case:
        Clash,       // Clash Check case:
        Block,       // Block Check case: 
    }
}
