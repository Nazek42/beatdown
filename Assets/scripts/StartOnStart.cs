using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartOnStart : MonoBehaviour
{
    private AudioPlayer player;

    private void Start()
    {
        player = GetComponent<AudioPlayer>();
    }

    private void Update()
    {
        if (player.ready && !player.playing)
        {
            player.PlaySong();
        }
    }
}
