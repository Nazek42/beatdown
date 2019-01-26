using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    // Distance of player from central meeting point
    private short distance_from_center;

    // Number of key moves hit consecutively
    private short combo_points;

    // Timing notes
    private bool sync;      // Did the player match with a subset of the notes on the last beat?
    private short timing;   // 

    // Amount of health for the player (the game ends when this reaches zero)
    private float health;
    static private float max_health = 128.0f;

    private float block;
    static private float max_block = 48.0f;

    // Orientation variable: designates the player as rightward facing or leftward facing
    // Left player <=> Right-facing
    // Right player <=> Left-facing
    private bool right_facing;

    // Stores player's action decision
    private Attack action;

    // Get action state of player
    public Attack GetAction()
    {
        return action;
    }

    // Get distance of player from center 
    public short GetDistance()
    {
        return distance_from_center;
    }

    // Get combo points of player
    public short GetComboPoints()
    {
        return combo_points;
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

        return mod;
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

    // Modify timing term
    public short ModTiming(short mod)
    {
        if (mod >= 0 && mod <= 7)
            timing = mod;

        return timing;
    }

    // Gets the timing value
    public short GetTiming()
    {
        return timing;
    }

    // Checks whether the player input a command which is a subset of the arrows on the beat
    public bool CheckSync(Player.Command input, Player.Command[] synced_moves)
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

    // Enum for action types
    public enum Command
    {
        None,
        Up,
        Left,
        Right,
        Down,
        UpLeft,
        UpRight,
        LeftRight,
        UpDown,
        DownLeft,
        DownRight
    }
}
