using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class DetectUnknownControllerMappings : MonoBehaviour {


    //buttons
    public Text button0Value;
    public Text button1Value;
    public Text button2Value;
    public Text button3Value;
    public Text button4Value;
    public Text button5Value;
    public Text button6Value;
    public Text button7Value;
    public Text button8Value;
    public Text button9Value;
    public Text button10Value;
    public Text button11Value;
    public Text button12Value;
    public Text button13Value;
    public Text button14Value;
    public Text button15Value;
    public Text button16Value;
    public Text button17Value;
    public Text button18Value;
    public Text button19Value;

    private void Start()
    {
        Debug.Log(Input.GetJoystickNames()[1]);
    }

    // Update is called once per frame
    void Update () {

        
       


        //buttons
        if (Input.GetKey(KeyCode.JoystickButton0) == true)
            button0Value.text = "pressed";
        else
            button0Value.text = "";

        if (Input.GetKey(KeyCode.JoystickButton1) == true)
            button1Value.text = "pressed";
        else
            button1Value.text = "";

        if (Input.GetKey(KeyCode.JoystickButton2) == true)
            button2Value.text = "pressed";
        else
            button2Value.text = "";

        if (Input.GetKey(KeyCode.JoystickButton3) == true)
            button3Value.text = "pressed";
        else
            button3Value.text = "";

        if (Input.GetKey(KeyCode.JoystickButton4) == true)
            button4Value.text = "pressed";
        else
            button4Value.text = "";

        if (Input.GetKey(KeyCode.JoystickButton5) == true)
            button5Value.text = "pressed";
        else
            button5Value.text = "";

        if (Input.GetKey(KeyCode.JoystickButton6) == true)
            button6Value.text = "pressed";
        else
            button6Value.text = "";

        if (Input.GetKey(KeyCode.JoystickButton7) == true)
            button7Value.text = "pressed";
        else
            button7Value.text = "";

        if (Input.GetKey(KeyCode.JoystickButton8) == true)
            button8Value.text = "pressed";
        else
            button8Value.text = "";

        if (Input.GetKey(KeyCode.JoystickButton9) == true)
            button9Value.text = "pressed";
        else
            button9Value.text = "";

        if (Input.GetKey(KeyCode.JoystickButton10) == true)
            button10Value.text = "pressed";
        else
            button10Value.text = "";

        if (Input.GetKey(KeyCode.JoystickButton11) == true)
            button11Value.text = "pressed";
        else
            button11Value.text = "";

        if (Input.GetKey(KeyCode.JoystickButton12) == true)
            button12Value.text = "pressed";
        else
            button12Value.text = "";

        if (Input.GetKey(KeyCode.JoystickButton13) == true)
            button13Value.text = "pressed";
        else
            button13Value.text = "";

        if (Input.GetKey(KeyCode.JoystickButton14) == true)
            button14Value.text = "pressed";
        else
            button14Value.text = "";

        if (Input.GetKey(KeyCode.JoystickButton15) == true)
            button15Value.text = "pressed";
        else
            button15Value.text = "";

        if (Input.GetKey(KeyCode.JoystickButton16) == true)
            button16Value.text = "pressed";
        else
            button16Value.text = "";

        if (Input.GetKey(KeyCode.JoystickButton17) == true)
            button17Value.text = "pressed";
        else
            button17Value.text = "";

        if (Input.GetKey(KeyCode.JoystickButton18) == true)
            button18Value.text = "pressed";
        else
            button18Value.text = "";

        if (Input.GetKey(KeyCode.JoystickButton19) == true)
            button19Value.text = "pressed";
        else
            button19Value.text = "";


	
	}
}
