using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BeatInput : MonoBehaviour
{
    public double inputLatency;
    public double chordWindow;

    public string upButton;
    public string downButton;
    public string leftButton;
    public string rightButton;

    public AttackHandler.Player player;
    public AttackHandler attackHandler;

    public AudioPlayer controller;

    // private Attack attack;
    private InputEvent lastInput;

    public Attack GetAttack()
    {
        return attackHandler.GetAttack(player);
    }

    public void SetAttack(Attack attack)
    {
        attackHandler.SetAttack(player, attack);
    }
    public double GetOffset()
    {
        return attackHandler.GetOffset(player);
    }
    public void SetOffset(double offset)
    {
        attackHandler.SetOffset(player, offset);
    }
    // Start is called before the first frame update
    void Start()
    {
        SetAttack(Attack.None);
        //chordWindow = timePerBeat;
        lastInput = new InputEvent(InputType.None, AudioSettings.dspTime);

        controller.onBeat += Beat;
    }

    void Beat(double beatNum)
    {
        // Get current time
        double curTime = AudioSettings.dspTime;
        // Get offset
        double offset = curTime - lastInput.time;
        SetOffset(offset);
        // audioSource.PlayOneShot(sound);
        if (GetAttack() == Attack.None)
        {
            switch (lastInput.input)
            {
                case InputType.Down:
                    SetAttack(Attack.Down);
                    break;
                case InputType.Up:
                    SetAttack(Attack.Up);
                    break;
                case InputType.Left:
                    SetAttack(Attack.Left);
                    break;
                case InputType.Right:
                    SetAttack(Attack.Right);
                    break;
                default:
                    break;
            }
        }
        
        // Print out the attack
        //Debug.Log(attack);
        //lastInput.timeUntilBeat += inputLatency;
        if (GetAttack() == Attack.None)
        {
            GetComponent<TextMesh>().text = "";
        }
        else
        {
            GetComponent<TextMesh>().text = OffsetToCheer(offset).ToString();
        }
        
        attackHandler.BeatReady(player);


        //Debug.Log(lastInput.input);
        //Debug.Log(lastInput.timeUntilBeat);
        // Reset the attack and input stack
        // SetAttack(Attack.None);
        lastInput.input = InputType.None;
        lastInput.time = AudioSettings.dspTime;
    }
    // Update is called once per frame
    void Update()
    {
        // Update state of each step
        //beatAccum += ((AudioSettings.dspTime)- last);
        //last = (AudioSettings.dspTime);
        // If attack has been determined, don't bother checking inputs
        if (GetAttack() == Attack.None)
        {
            // Check inputs
            bool up = Input.GetButtonDown(upButton);
            bool down = Input.GetButtonDown(downButton);
            bool left = Input.GetButtonDown(leftButton);
            bool right = Input.GetButtonDown(rightButton);

            // Check if we can just set the input
            if (lastInput.input == InputType.None)
            {
                if (up)
                {
                    lastInput.input = InputType.Up;
                    lastInput.time = AudioSettings.dspTime;
                    up = false;
                }
                else if (down)
                {
                    lastInput.input = InputType.Down;
                    lastInput.time = AudioSettings.dspTime;
                    down = false;
                }
                else if (left)
                {
                    lastInput.input = InputType.Left;
                    lastInput.time = AudioSettings.dspTime;
                    left = false;
                }
                else if (right)
                {
                    lastInput.input = InputType.Right;
                    lastInput.time = AudioSettings.dspTime;
                    right = false;
                }
            }
            // Check for chords with the last input
            if(lastInput.input == InputType.Up)
            {
                if(left)
                {
                    SetAttack(Attack.UpLeft);
                }
                else if(right)
                {
                    SetAttack(Attack.UpRight);
                }
                else if(down)
                {
                    SetAttack(Attack.UpDown);
                }
            }
            else if (lastInput.input == InputType.Down)
            {
                if (left)
                {
                    SetAttack(Attack.DownLeft);
                }
                else if (right)
                {
                    SetAttack(Attack.DownRight);
                }
                else if (down)
                {
                    SetAttack(Attack.UpDown);
                }
            }
            else if (lastInput.input == InputType.Left)
            {
                if (up)
                {
                    SetAttack(Attack.UpLeft);
                }
                else if (right)
                {
                    SetAttack(Attack.LeftRight);
                }
                else if (down)
                {
                    SetAttack(Attack.DownLeft);
                }
            }
            else if (lastInput.input == InputType.Right)
            {
                if (up)
                {
                    SetAttack(Attack.UpRight);
                }
                else if (left)
                {
                    SetAttack(Attack.LeftRight);
                }
                else if (down)
                {
                    SetAttack(Attack.DownRight);
                }
            }
        }

    }

    public enum Attack
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
    private enum InputType
    {
        None,
        Up,
        Left,
        Right,
        Down
    }
    private struct InputEvent
    {
        public InputType input;
        public double time;

        public InputEvent(InputType input, double time)
        {
            this.input = input;
            this.time = time;
        }
    }

    private string OffsetToCheer(double offset)
    {
        offset /= 2;
        if(offset < 0.01666666)
        {
            return "Marvelous";
        }
        else if(offset < 0.033333)
        {
            return "Perfect";
        }
        else if(offset < 0.092)
        {
            return "Great";
        }
        else if(offset < 0.142)
        {
            return "Good";
        }
        else
        {
            return "Bad";
        }
    }

    public static string AttackToArrows(Attack attack)
    {
        switch(attack)
        {
            case Attack.None:
                return "    ";
            case Attack.Up:
                return " ^  ";
            case Attack.Left:
                return "<   ";
            case Attack.Right:
                return "   >";
            case Attack.Down:
                return "  v ";
            case Attack.UpLeft:
                return "<^  ";
            case Attack.UpRight:
                return " ^ >";
            case Attack.LeftRight:
                return "<  >";
            case Attack.UpDown:
                return " ^v ";
            case Attack.DownLeft:
                return "< v ";
            case Attack.DownRight:
                return "  v>";
            default:
                return "    ";
        }
    }
}
