using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour {
    [SerializeField] private Sprite quarterSprite;
    [SerializeField] private Sprite eighthSprite;

    private AudioPlayer controller;
    private float beat;
    private NoteType type;

    private new SpriteRenderer renderer;

	// Use this for initialization
	void Start () {
        renderer = GetComponent<SpriteRenderer>();
        renderer.enabled = false;
	}

    public void Initialize(AudioPlayer c, float b, NoteType t)
    {
        renderer = GetComponent<SpriteRenderer>();
        controller = c;
        beat = b;
        type = t;
        renderer.sprite = beat % 2 == 0 ? quarterSprite : eighthSprite;
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90 * (int)type));
    }

	// Update is called once per frame
	void Update () {
        float until = controller.TimeOfBeat(beat) - (float)controller.SongTime();
        if (until < 0)
        {
            Destroy(gameObject);
            return;
        }
        transform.position = Vector2.Lerp(controller.GetStartPos(type), controller.GetEndPos(type), (float)(1 - until / controller.note_time));
        if (!renderer.enabled && until <= controller.note_time)
        {
            renderer.enabled = true;
        }
    }
}
