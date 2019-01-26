using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NoteController : MonoBehaviour
{
    [SerializeField] private LineRenderer topLine, bottomLine;
    [SerializeField] private Note note_prefab;

    [SerializeField] private bool reversed;
    public double note_time;

    private const double note_pre_time = 0.2;

    private AudioPlayer controller;
    private IEnumerable<NoteData> notes;

    private float left_x, start_y, end_y, note_separation;

    private readonly int[] distances = { 1, 3, 2, 0 };

    // Start is called before the first frame update
    void Start()
    {
        float line_left = topLine.GetPosition(0).x;
        float line_right = topLine.GetPosition(1).x;
        left_x = line_left + 1;
        note_separation = (line_right - line_left) / 4;
        start_y = bottomLine.GetPosition(0).y;
        end_y = topLine.GetPosition(0).y;
    }

    public void Initialize(AudioPlayer controller_, IEnumerable<NoteData> notes_)
    {
        controller = controller_;
        notes = reversed ? notes_.Select(ReverseNote) : notes_;
        controller.onBeat += SpawnNotes;
        controller.onBeat += AnimateLines;
        Debug.Log("Note controller " + gameObject.name + " ready with " + notes.Count() + " notes");
    }

    public static NoteData ReverseNote(NoteData note)
    {
        switch (note.direction)
        {
            case NoteType.right:
                return new NoteData(note.beat, NoteType.left);
            case NoteType.left:
                return new NoteData(note.beat, NoteType.right);
            default:
                return note;
        }
    }

    private void SpawnNotes(double current_beat)
    {
        foreach (NoteData note in notes.Where(
            note => Note.DoubleIsInteger(note.beat)
                    && (controller.beatLerper.TimeFromBeat(note.beat) - controller.SongTime()) < (note_time + note_pre_time)))
        {
            //Debug.Log("spawning note for beat " + note.beat);
            Instantiate(note_prefab).Initialize(controller, this, note.beat, note.direction);
        }
    }

    private void AnimateLines(double _)
    {
        StartCoroutine(LineBulge());
    }

    private IEnumerator LineBulge()
    {
        topLine.widthMultiplier = .2f;
        bottomLine.widthMultiplier = .2f;
        yield return new WaitForSeconds(0.06f);
        topLine.widthMultiplier = .1f;
        bottomLine.widthMultiplier = .1f;
    }

    public float GetNoteX(NoteType type)
    {
        return left_x + note_separation * distances[(int)type];
    }

    public Vector2 GetStartPos(NoteType type)
    {
        return new Vector2(GetNoteX(type), start_y);
    }

    public Vector2 GetEndPos(NoteType type)
    {
        return new Vector2(GetNoteX(type), end_y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
