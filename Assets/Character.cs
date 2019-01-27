using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    Player player;
    SpriteRenderer sprend;
    public TextMesh healthText;

    [System.Serializable]
    public class SpriteTableEntry
    {
        public Player.Attack attack;
        public Sprite sprite;
    }
    public SpriteTableEntry[] spriteList;

    private Dictionary<Player.Attack, Sprite> spriteTable;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
        sprend = GetComponent<SpriteRenderer>();
        spriteTable = new Dictionary<Player.Attack, Sprite>();
        foreach (SpriteTableEntry entry in spriteList)
        {
            spriteTable.Add(entry.attack, entry.sprite);
        }
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
        transform.localPosition = new Vector3(mult * (player.distance_from_center * 1.8f + 1F), transform.localPosition.y, transform.localPosition.z);
        healthText.text = player.health.ToString() + "\n" + player.block.ToString();
        healthText.text.Replace("\n", "\\n");

        if (!ReferenceEquals(sprend.sprite, spriteTable[player.action]))
        {
            Debug.Log("changing sprite");
            sprend.sprite = spriteTable[player.action];
        }
    }
}
