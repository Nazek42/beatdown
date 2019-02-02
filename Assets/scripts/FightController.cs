using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightController : MonoBehaviour {
    private ParticleManager particler;

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
/*1*/     { Outcome.Noop , Outcome.Noop , Outcome.Dodge, Outcome.Block, Outcome.Block, Outcome.Block, Outcome.Block, Outcome.Block, Outcome.Block, Outcome.Hit  , Outcome.Noop  },
/*2*/     { Outcome.Dodge, Outcome.Dodge, Outcome.Dodge, Outcome.Dodge, Outcome.Hit  , Outcome.Dodge, Outcome.Dodge, Outcome.Dodge, Outcome.Dodge, Outcome.Dodge, Outcome.Dodge },
/*3*/     { Outcome.Block, Outcome.Block, Outcome.Dodge, Outcome.Clash, Outcome.Hit  , Outcome.Hit, Outcome.Hit  , Outcome.Hit  , Outcome.Hit  , Outcome.Hit  , Outcome.Hit   },
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
        0.0f,       /* Projectile */
        3.0f,       /* Body Check */
        3.0f,       /* Uppercut */
        4.0f,       /* High Punch */
        5.0f,       /* Spin Kick */
        3.5f,       /* Low Kick */
        0.0f        /* Noop */
    };

    // Combo point damage multiplier for uppercuts and projectiles
    static private float combo_mult = 1.5f;

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
        right_damage = CalculateAttack(right_player, right_act, left_player);

        // Calculate left player damage
        left_damage = CalculateAttack(left_player, left_act, left_player);

        // Apply damage calculations
        left_player.ModHealth(-right_damage);
        right_player.ModHealth(-left_damage);

        // Do particle effects
        if (right_damage > 0)
        {
            if (right_act.IsHigh())
            {
                particler.SpawnEffectOnPlayer(right_damage >= 4 ? "heavy_high_hit" : "light_high_hit", left_player, right_player);
            }
            if (right_act.IsLow())
            {
                particler.SpawnEffectOnPlayer(right_damage >= 4 ? "heavy_low_hit" : "light_low_hit", left_player, right_player);
            }
            if (right_act.IsMid())
            {
                particler.SpawnEffectOnPlayer(right_damage >= 4 ? "heavy_mid_hit" : "light_mid_hit", left_player, right_player);
            }
        }

        if (left_damage > 0)
        {
            if (left_act.IsHigh())
            {
                particler.SpawnEffectOnPlayer(left_damage >= 4 ? "heavy_high_hit" : "light_high_hit", right_player, left_player);
            }                
            if (left_act.IsLow())
            {
                particler.SpawnEffectOnPlayer(left_damage >= 4 ? "heavy_low_hit" : "light_low_hit", right_player, left_player);
            }                
            if (left_act.IsMid())
            {
                particler.SpawnEffectOnPlayer(left_damage >= 4 ? "heavy_mid_hit" : "light_mid_hit", right_player, left_player);
            }
        }
    }

    private void Clash(Player.Attack left_act, Player.Attack right_act)
    {
        if(left_act == Player.Attack.Projectile)
        {
            // Calculate player damage (both known projectiles)
            float right_damage = CalculateBoostedAttack(right_player, right_act);
            float left_damage = CalculateBoostedAttack(left_player, left_act);

            float tide = right_damage - left_damage;

            // If left overpowers
            if (tide < 0)
                right_player.ModHealth(tide);
            else if (tide > 0)
                left_player.ModHealth(-tide);

        }
        
        else
        {
            if (left_act.IsHigh() || right_act.IsHigh())
                particler.SpawnEffectOnPlayer("high_clash", left_player, right_player);

            if (left_act.IsLow() || right_act.IsLow())
                particler.SpawnEffectOnPlayer("low_clash", left_player, right_player);

            if (left_act.IsMid() || right_act.IsMid())
                particler.SpawnEffectOnPlayer("mid_clash", left_player, right_player);
        }
        

        // If not projectiles, players neutralize each other's damage (noop)
    }

    private void Block(Player.Attack left_act, Player.Attack right_act)
    {
        Player blocker, attacker;
        Player.Attack atk;
        if(left_act == Player.Attack.HighBlock || left_act == Player.Attack.LowBlock)
        {
            blocker = left_player;
            attacker = right_player;
            atk = right_act;
        }
        else
        {
            blocker = right_player;
            attacker = left_player;
            atk = left_act;
        }

        if (blocker.GetComboPoints() == 8)
        {
            blocker.RefillBlock();
            blocker.DrainCombos();
        }

        float block_val = blocker.GetBlock();
        
        float antag_dmg;
        // Calculate attacker damage
        antag_dmg = CalculateAttack(attacker, atk, blocker);

        // Take remaining block value after hit
        float remainder = block_val - antag_dmg;

        // If damage exceeds block value
        if (remainder < 0.0f)
        {
            blocker.ModBlock(-block_val);
            blocker.ModHealth(remainder);
        }

        // If damage does not exceed block value
        else blocker.ModBlock(-antag_dmg);

        return;
    }

    // Utility functions
    private float CalculateAttack(Player attacker, Player.Attack action, Player other)
    {
        float dmg;
        if (action == Player.Attack.Projectile || action == Player.Attack.Uppercut)
        {
            dmg = CalculateBoostedAttack(attacker, action);
            if (action == Player.Attack.Projectile)
            {
                if (attacker.GetBlock() == 0)
                {
                    dmg = 0;
                } else
                {
                    //particler.SpawnEffectOnPlayer("beam", attacker, other);
                    attacker.DrainCombos();
                }
            }
            else
            {
                attacker.DrainCombos();
            }
        }
        else
        {
            dmg = BaseDmgTable[(int)action];
        }

        if (action == attacker.prev_action)
        {
            dmg *= 0.5f;
        }

        return dmg;
    }

    private static float CalculateBoostedAttack(Player attacker, Player.Attack action)
    {
        return BaseDmgTable[(int)action] + (attacker.GetComboPoints() * combo_mult);
    }

    // Use this for initialization
    void Start ()
    {
        particler = GetComponent<ParticleManager>();

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
