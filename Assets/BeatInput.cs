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


    public AudioPlayer controller;

    private Attack attack;
    private InputEvent lastInput;

    // Start is called before the first frame update
    void Start()
    {
        attack = Attack.None;
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
        //beatAccum -= timePerBeat;
        // audioSource.PlayOneShot(sound);
        if(attack == Attack.None)
        {
            switch (lastInput.input)
            {
                case InputType.Down:
                    attack = Attack.Down;
                    break;
                case InputType.Up:
                    attack = Attack.Up;
                    break;
                case InputType.Left:
                    attack = Attack.Left;
                    break;
                case InputType.Right:
                    attack = Attack.Right;
                    break;
                default:
                    break;
            }
        }
        // Print out the attack
        //Debug.Log(attack);
        //lastInput.timeUntilBeat += inputLatency;
        GetComponent<TextMesh>().text = Mathf.RoundToInt((float)beatNum).ToString() + " " + attack.ToString() + " " + OffsetToCheer(offset).ToString();// + " " + TimeToName(attack, lastInput.timeUntilBeat) + " " + lastInput.timeUntilBeat.ToString();
        //Debug.Log(lastInput.input);
        //Debug.Log(lastInput.timeUntilBeat);
        // Reset the attack and input stack
        attack = Attack.None;
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
        if (attack == Attack.None)
        {
            // Check to see if there is an existing input that has passed the chord window
            if (lastInput.input != InputType.None)
            {
                // Get current time to beat
                //double timeUntilBeat = timePerBeat - beatAccum;
                // Determine how far current time to beat is from the last input
               //double timeSinceInput = timeUntilBeat - lastInput.timeUntilBeat;
                // Determine if this time is too long

                /*if (timeSinceInput > chordWindow || beatAccum >= timePerBeat)
                {
                    switch (lastInput.input)
                    {
                        case InputType.Down:
                            attack = Attack.Down;
                            break;
                        case InputType.Up:
                            attack = Attack.Up;
                            break;
                        case InputType.Left:
                            attack = Attack.Left;
                            break;
                        case InputType.Right:
                            attack = Attack.Right;
                            break;
                        default:
                            break;
                    }
                }*/
            }
            //else {
                // Check inputs
                // bool up = Input.GetKeyDown(KeyCode.UpArrow);
                //bool down = Input.GetKeyDown(KeyCode.DownArrow);
                //bool left = Input.GetKeyDown(KeyCode.LeftArrow);
                //bool right = Input.GetKeyDown(KeyCode.RightArrow);
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
                        attack = Attack.UpLeft;
                    }
                    else if(right)
                    {
                        attack = Attack.UpRight;
                    }
                    else if(down)
                    {
                        attack = Attack.UpDown;
                    }
                }
                else if (lastInput.input == InputType.Down)
                {
                    if (left)
                    {
                        attack = Attack.DownLeft;
                    }
                    else if (right)
                    {
                        attack = Attack.DownRight;
                    }
                    else if (down)
                    {
                        attack = Attack.UpDown;
                    }
                }
                else if (lastInput.input == InputType.Left)
                {
                    if (up)
                    {
                        attack = Attack.UpLeft;
                    }
                    else if (right)
                    {
                        attack = Attack.LeftRight;
                    }
                    else if (down)
                    {
                        attack = Attack.DownLeft;
                    }
                }
                else if (lastInput.input == InputType.Right)
                {
                    if (up)
                    {
                        attack = Attack.UpRight;
                    }
                    else if (left)
                    {
                        attack = Attack.LeftRight;
                    }
                    else if (down)
                    {
                        attack = Attack.DownRight;
                    }
                }
        }
        else
        {
            Debug.Log("Attack determined, not bothering");
        }

    }

    private enum Attack
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
}
