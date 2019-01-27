using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuperMeter : MonoBehaviour
{
    //Used to set the color of stuff if wanted
    public Color myColor;
    
    public Color MyColor
    {
        get
        {
            return myColor;
        }

        set
        {
            myColor = value;
        }
    }

    //Current value of the super meter
    public int meterValue = 0;

    public int MeterValue
    {
        set
        {
            meterValue = value;
        }
    }

    private int meterMax;
    private int meterDiv = 1;   //Hold the 8 part divitions of the meter

    //Maximum value of the super meter
    public int MeterMax
    {
        set
        {
            meterMax = value;
            meterDiv = value / 8;
        }
    }

    Graphic m_MyGraphic;

    public Image pip_0;
    public Image pip_1;
    public Image pip_2;
    public Image pip_3;
    public Image pip_4;
    public Image pip_5;
    public Image pip_6;
    public Image pip_7;

    // Start is called before the first frame update
    void Start()
    {
        //Fetch the Graphic from the GameObject
        //m_MyGraphic = GetComponent<Graphic>();
        //Create a new Color that starts as red
        //MyColor = Color.white;
        //Change the Graphic Color to the new Color
        //m_MyGraphic.color = MyColor;

       pip_0.color = myColor;



    }

    // Update is called once per frame
    void Update()
    {
        if (meterValue > 0)
        {
            pip_0.color = Color.white;
        }
        else
        {
            pip_0.color = Color.black;
        }

        if (meterValue > meterDiv)
        {
           pip_1.color = Color.white;
        }

        else
        {
           pip_1.color = Color.black;
        }

        if (meterValue > meterDiv*2)
        {
            pip_2.color = Color.white;
        }

        else
        {
            pip_2.color = Color.black;
        }

        if (meterValue > meterDiv*3)
        {
           pip_3.color = Color.white;
        }

        else
        {
            pip_3.color = Color.black;
        }

        if (meterValue > meterDiv*4)
        {
            pip_4.color = Color.white;
        }

        else
        {
            pip_4.color = Color.black;
        }

        if (meterValue > meterDiv*5)
        {
           pip_5.color = Color.white;
        }

        else
        {
            pip_5.color = Color.black;
        }

        if (meterValue > meterDiv*6)
        {
            pip_6.color = Color.white;
        }

        else
        {
            pip_6.color = Color.black;
        }

        if (meterValue > meterDiv*7)
        {
            pip_7.color = Color.white;
        }

        else
        {
            pip_7.color = Color.black;
        }
    }
}
