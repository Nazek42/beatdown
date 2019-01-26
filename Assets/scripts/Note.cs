using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour {
    [SerializeField] private Sprite quarterSprite;
    [SerializeField] private Sprite eighthSprite;

    private AudioPlayer controller;
    private double beat;
    private NoteType type;

    private new SpriteRenderer renderer;

	// Use this for initialization
	void Start () {
        renderer = GetComponent<SpriteRenderer>();
        renderer.enabled = false;
	}

    public void Initialize(AudioPlayer c, double b, NoteType t)
    {
        renderer = GetComponent<SpriteRenderer>();
        controller = c;
        beat = b;
        type = t;

        renderer.sprite = DoubleIsInteger(beat) ? quarterSprite : eighthSprite;
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90 * (int)type));
        if(!DoubleIsInteger(b))
        {
            Destroy(gameObject);
        }
    }

    public static bool DoubleIsInteger(double x)
    {
        return (x - System.Math.Truncate(x)) < double.Epsilon;
    }

	// Update is called once per frame
	void Update () {
        double until = controller.beatLerper.TimeFromBeat(beat) - controller.SongTime();
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
