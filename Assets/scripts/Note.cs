using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour {
    [SerializeField] private Sprite quarterSprite;
    [SerializeField] private Sprite eighthSprite;

    private AudioPlayer audioPlayer;
    private NoteController noteController;
    private double beat;
    private NoteType type;

    private new SpriteRenderer renderer;

	// Use this for initialization
	void Start () {
        renderer = GetComponent<SpriteRenderer>();
        renderer.enabled = false;
	}

    public void Initialize(AudioPlayer c, NoteController n, double b, NoteType t)
    {
        renderer = GetComponent<SpriteRenderer>();
        audioPlayer = c;
        noteController = n;
        beat = b;
        type = t;

        renderer.sprite = DoubleIsInteger(beat) ? quarterSprite : eighthSprite;
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90 * (int)type));
    }

    public static bool DoubleIsInteger(double x)
    {
        return (x - System.Math.Truncate(x)) < float.Epsilon;
    }

	// Update is called once per frame
	void Update () {
        double until = audioPlayer.beatLerper.TimeFromBeat(beat) - audioPlayer.SongTime();
        if (until < 0)
        {
            Debug.Log("Note destroying self");
            Destroy(gameObject);
            return;
        }
        transform.position = Vector2.Lerp(noteController.GetStartPos(type), noteController.GetEndPos(type), (float)(1 - until / noteController.note_time));
        if (!renderer.enabled && until <= noteController.note_time)
        {
            renderer.enabled = true;
        }
    }
}
