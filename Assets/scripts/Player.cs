using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    // Distance of player from central meeting point
    public short distance_from_center;

    // Number of key moves hit consecutively
    public short combo_points;

    // Timing notes
    public bool sync;      // Did the player match with a subset of the notes on the last beat?
    public short timing;   // How well did the play match his or her move with the beat?

    // Amount of health for the player (the game ends when this reaches zero)
    public float health;
    static public float max_health = 128.0f;

    public float block;
    static public float max_block = 48.0f;

    // Orientation variable: designates the player as rightward facing or leftward facing
    // Left player <=> Right-facing
    // Right player <=> Left-facing
    public bool right_facing;

    // Stores player's action decision
    public Attack action;
    public Attack prev_action;

    

    // Get action state of player
    public Attack GetAction()
    {
        return action;
    }

    // Takes a keyboard/DDR pad BeatInput.Command and converts it into the appropriate move
    // for the player
    public Attack DetermineAction(BeatInput.Command comm_code)
    {
        prev_action = action;
        switch(comm_code)
        {
            case (BeatInput.Command.None):
                action = Attack.Noop;       // Both players
                break;

            case (BeatInput.Command.Up):
                action = Attack.HighBlock;  // Both players
                break;

            case (BeatInput.Command.Right):

                if (right_facing) action = Attack.Jab;  // Left player
                else action = Attack.Backdash;          // Right player

                break;

            case (BeatInput.Command.Left):

                if (right_facing) action = Attack.Backdash;  // Left player
                else action = Attack.Jab;                    // Right player

                break;

            case (BeatInput.Command.Down):
                action = Attack.LowBlock;   // Both players
                break;

            case (BeatInput.Command.UpLeft):

                if (right_facing) action = Attack.Uppercut;  // Left player
                else action = Attack.HighPunch;              // Right player

                break;

            case (BeatInput.Command.UpRight):

                if (right_facing) action = Attack.HighPunch;  // Left player
                else action = Attack.Uppercut;                // Right player

                break;

            case (BeatInput.Command.LeftRight):
                action = Attack.BodyCheck;  // Both players
                break;

            case (BeatInput.Command.UpDown):
                action = Attack.Projectile; // Both players
                break;

            case (BeatInput.Command.DownLeft):

                if (right_facing) action = Attack.SpinKick;  // Left player
                else action = Attack.LowKick;                // Right player

                break;

            case (BeatInput.Command.DownRight):

                if (right_facing) action = Attack.LowKick;  // Left player
                else action = Attack.SpinKick;              // Right player

                break;

        }

        return action;
    }

    // Get distance of player from center 
    public short GetDistance()
    {
        return distance_from_center;
    }

    // Distance modifier function
    public short ModDistance(short mod)
    {
        if (distance_from_center + mod < 0)
            distance_from_center = 0;
        else if (distance_from_center + mod > 8)
            distance_from_center = 8;
        else
            distance_from_center += mod;

        return distance_from_center;
    }

    // Modifies the number of combo points the player has
    public short ModCombos(short mod)
    {
        if (combo_points + mod < 0)
            combo_points = 0;

        else if (combo_points + mod > 8)
        {
            // Reset block if max combo points hit
            if (combo_points != 8) block = max_block;
            combo_points = 8;
        }

        else
            combo_points += mod;

        return mod;
    }

    // Resets the combo points to zero
    public void DrainCombos()
    {
        combo_points = 0;
    }

    // Get combo points of player
    public short GetComboPoints()
    {
        return combo_points;
    }

    // Returns health of player
    public float GetHealth()
    {
        return health;
    }

    // Member function for modifying health.
    // Adds the input parameter value to the health of the health of the player
    // clamps between maxhealth and 0
    public float ModHealth(float mod)
    {
        if (health + mod < 0.0f)
            health = 0.0f;
        else if (health + mod > max_health)
            health = max_health;
        else
            health += mod;

        return health;
    }

    // Returns block value for player
    public float GetBlock()
    {
        return block;
    }

    // Modifies block value for player
    public float ModBlock(float mod)
    {
        if (block + mod <= 0.0f)
        {
            block = 0.0f;
        }

        else if (block + mod > max_block)
        {
            block = max_block;
        }

        else
            block += mod;

        return block;
    }

    public float RefillBlock()
    {
        block = max_block;
        return block;
    }

    // Gets the timing value
    public short GetTiming()
    {
        return timing;
    }

    // Modify timing term
    public short ModTiming(short mod)
    {
        if (mod >= 0 && mod <= 7)
            timing = mod;

        return timing;
    }

    // Checks whether the player input a BeatInput.Command which is a subset of the arrows on the beat
    public bool CheckSync(BeatInput.Command input, BeatInput.Command[] synced_moves)
    {
        sync = false;

        for(int i = 0; i < synced_moves.GetLength(0); ++i)
        {
            if (input == synced_moves[i])
            {
                sync = true;
                break;
            }
        }

        return sync;
    }

    // Get the synchronization value
    public bool GetSync()
    {
        return sync;
    }

    // Set the orientation of the player
    public bool SetOrientation(bool set_right)
    {
        return (right_facing = set_right);
    }

	// Use this for initialization
	void Start ()
    {
        health = 128.0f;
        block = 48.0f;
        combo_points = 0;
        distance_from_center = 8;
        action = Attack.Noop;
        sync = false;
        timing = 0;
	}
	
	// Update is called once per frame
	/*void Update ()
    {
		
	}*/

    public enum Attack
    {
        HighBlock,
        LowBlock,
        Backdash,
        Jab,
        Projectile,
        BodyCheck,
        Uppercut,
        HighPunch,
        SpinKick,
        LowKick,
        Noop
    }
}
