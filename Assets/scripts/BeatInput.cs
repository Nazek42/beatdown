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

    public AttackHandler.PlayerNum player;
    public AttackHandler attackHandler;

    public AudioPlayer controller;

    // private Attack attack;
    private InputEvent lastInput;

    public Command GetCommand()
    {
        return attackHandler.GetCommand(player);
    }

    public void SetCommand(Command attack)
    {
        attackHandler.SetCommand(player, attack);
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
        SetCommand(Command.None);
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
        if (GetCommand() == Command.None)
        {
            switch (lastInput.input)
            {
                case InputType.Down:
                    SetCommand(Command.Down);
                    break;
                case InputType.Up:
                    SetCommand(Command.Up);
                    break;
                case InputType.Left:
                    SetCommand(Command.Left);
                    break;
                case InputType.Right:
                    SetCommand(Command.Right);
                    break;
                default:
                    break;
            }
        }
        
        // Print out the attack
        //Debug.Log(attack);
        //lastInput.timeUntilBeat += inputLatency;
        if (GetCommand() == Command.None)
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
        if (GetCommand() == Command.None)
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
                    SetCommand(Command.UpLeft);
                }
                else if(right)
                {
                    SetCommand(Command.UpRight);
                }
                else if(down)
                {
                    SetCommand(Command.UpDown);
                }
            }
            else if (lastInput.input == InputType.Down)
            {
                if (left)
                {
                    SetCommand(Command.DownLeft);
                }
                else if (right)
                {
                    SetCommand(Command.DownRight);
                }
                else if (down)
                {
                    SetCommand(Command.UpDown);
                }
            }
            else if (lastInput.input == InputType.Left)
            {
                if (up)
                {
                    SetCommand(Command.UpLeft);
                }
                else if (right)
                {
                    SetCommand(Command.LeftRight);
                }
                else if (down)
                {
                    SetCommand(Command.DownLeft);
                }
            }
            else if (lastInput.input == InputType.Right)
            {
                if (up)
                {
                    SetCommand(Command.UpRight);
                }
                else if (left)
                {
                    SetCommand(Command.LeftRight);
                }
                else if (down)
                {
                    SetCommand(Command.DownRight);
                }
            }
        }

    }

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

    public static string CommandToArrows(Command command)
    {
        switch(command)
        {
            case Command.None:
                return "    ";
            case Command.Up:
                return " ^  ";
            case Command.Left:
                return "<   ";
            case Command.Right:
                return "   >";
            case Command.Down:
                return "  v ";
            case Command.UpLeft:
                return "<^  ";
            case Command.UpRight:
                return " ^ >";
            case Command.LeftRight:
                return "<  >";
            case Command.UpDown:
                return " ^v ";
            case Command.DownLeft:
                return "< v ";
            case Command.DownRight:
                return "  v>";
            default:
                return "    ";
        }
    }
}
