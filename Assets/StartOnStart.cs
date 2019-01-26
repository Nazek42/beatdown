using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartOnStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    bool flag = false;
    // Update is called once per frame
    void Update()
    {
        if (!flag)
        {
            GetComponent<Button>().onClick.Invoke();
            flag = true;
        }
    }
}
