using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BeatInput : MonoBehaviour
{
    double beatAccum = 0;
    public double bpm = 165;
    public double inputLatency;
    public double songLatency;
    double timePerBeat;
    double chordWindow;

    int beat = 0;

    float upState = 0.0F;
    float downState = 0.0F;
    float leftState = 0.0F;
    float rightState = 0.0F;


    AudioSource audioSource;
    public AudioClip sound;
    public AudioClip music;

    private Attack attack;

    private InputEvent lastInput;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = music;
        audioSource.Play();
        attack = Attack.None;
        timePerBeat = 60.0 / bpm;
        chordWindow = timePerBeat;
        lastInput = new InputEvent(InputType.None, timePerBeat);
        last = AudioSettings.dspTime;

        songLatency *= -1;

    }
    double last;
    // Update is called once per frame
    void Update()
    {
        if(audioSource.time < songLatency)
        {
            GetComponent<TextMesh>().text = "waiting for song";
            return;
        }
        // Update state of each step
        beatAccum += ((AudioSettings.dspTime)- last);
        last = (AudioSettings.dspTime);
        // If attack has been determined, don't bother checking inputs
        if (attack == Attack.None)
        {
            // Check to see if there is an existing input that has passed the chord window
            if (lastInput.input != InputType.None)
            {
                // Get current time to beat
                double timeUntilBeat = timePerBeat - beatAccum;
                // Determine how far current time to beat is from the last input
                double timeSinceInput = timeUntilBeat - lastInput.timeUntilBeat;
                // Determine if this time is too long

                if (timeSinceInput > chordWindow || beatAccum >= timePerBeat)
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
            }
            //else {
                // Check inputs
                // bool up = Input.GetKeyDown(KeyCode.UpArrow);
                //bool down = Input.GetKeyDown(KeyCode.DownArrow);
                //bool left = Input.GetKeyDown(KeyCode.LeftArrow);
                //bool right = Input.GetKeyDown(KeyCode.RightArrow);
                bool up = Input.GetButtonDown("Up");
                bool down = Input.GetButtonDown("Down");
                bool left = Input.GetButtonDown("Left");
                bool right = Input.GetButtonDown("Right");


                // Check if we can just set the input
                if (lastInput.input == InputType.None)
                {
                    if (up)
                    {
                        lastInput.input = InputType.Up;
                        lastInput.timeUntilBeat = timePerBeat - beatAccum;
                        up = false;
                    }
                    else if (down)
                    {
                        lastInput.input = InputType.Down;
                        lastInput.timeUntilBeat = timePerBeat - beatAccum;
                        down = false;
                    }
                    else if (left)
                    {
                        lastInput.input = InputType.Left;
                        lastInput.timeUntilBeat = timePerBeat - beatAccum;
                        left = false;
                    }
                    else if (right)
                    {
                        lastInput.input = InputType.Right;
                        lastInput.timeUntilBeat = timePerBeat - beatAccum;
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
            //}
        }
        else
        {
            Debug.Log("Attack determined, not bothering");
        }
        if (beatAccum >= timePerBeat)
        {
            beatAccum -= timePerBeat;
           // audioSource.PlayOneShot(sound);

            // Print out the attack
            //Debug.Log(attack);
            beat += 1;
            lastInput.timeUntilBeat += inputLatency;
            GetComponent<TextMesh>().text = beat.ToString() + " " + attack.ToString() + " " + TimeToName(attack, lastInput.timeUntilBeat) + " " + lastInput.timeUntilBeat.ToString();
            //Debug.Log(lastInput.input);
            //Debug.Log(lastInput.timeUntilBeat);
            // Reset the attack and input stack
            attack = Attack.None;
            lastInput.input = InputType.None;
            lastInput.timeUntilBeat = timePerBeat;
        }
    }

    string TimeToName(Attack attack, double time)
    {
        if(attack == Attack.None)
        {
            return "Miss";
        }
        if(time < 0)
        {
            return "Bad";
        }
        if(time < timePerBeat * 0.05)
        {
            return "Marvelous";
        }
        else if(time < timePerBeat * 0.10)
        {
            return "Perfect";
        }
        else if(time < timePerBeat * 0.20)
        {
            return "Great";
        }
        else if(time < timePerBeat * 0.40)
        {
            return "Good";
        }
        else
        {
            return "Bad";
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
        public double timeUntilBeat;

        public InputEvent(InputType input, double timeUntilBeat)
        {
            this.input = input;
            this.timeUntilBeat = timeUntilBeat;
        }
    }
}
