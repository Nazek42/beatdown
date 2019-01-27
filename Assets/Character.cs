using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    Player player;
    public TextMesh healthText;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();   
    }

    // Update is called once per frame
    void Update()
    {
        float mult;
        if(player.right_facing)
        {
            mult = -1.0F;
        }
        else
        {
            mult = 1.0F;
        }
        transform.localPosition = new Vector3(mult * (player.distance_from_center + 0.5F), transform.localPosition.y, transform.localPosition.z);
        healthText.text = player.health.ToString() + "\n" + player.block.ToString();
        healthText.text.Replace("\n", "\\n");
    }
}
