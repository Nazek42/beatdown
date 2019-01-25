using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum NoteType { down = 0, right = 1, up = 2, left = 3 };

[System.Serializable]
public class NoteData
{
    public float beat;
    public NoteType direction;

    public NoteData(float beat_, NoteType direction_)
    {
        beat = beat_;
        direction = direction_;
    }
}

public class AudioPlayer : MonoBehaviour {
    AudioSource audio_source;

    [SerializeField] private BPMEvent[] events;
    [SerializeField] private float timeToEval;
    private BeatLerper beatLerper;

    /*[SerializeField]*/ private float BPM;
    private Dictionary<float, float> BPMs;
    private Dictionary<float, float> stops;
    private AnimationCurve BeatOverTime;
    private AnimationCurve TimeOverBeat;
    [SerializeField] private float offset;
    /*[SerializeField] private Renderer flash;*/

    public float left_x;
    public float start_y;
    public float end_y;
    public float note_separation;
    public float note_time;
    public int beats_ahead { get; private set; }

    private Song song;
    // /*[SerializeField]*/ private NoteData[] notes;
    private IEnumerable<NoteData> notes_iter;
    [SerializeField] private Note note_prefab;

    public float beat_time { get; private set; }

    public int song_pos { get; private set; }
    private float song_start_time;

    private readonly int[] distances = { 1, 3, 2, 0 };

    public delegate void OnBeat(int beat_num);
    public OnBeat onBeat;

    // Use this for initialization
    void Start()
    {
        audio_source = GetComponent<AudioSource>();
        /*
        song = BeatFile.ReadBeatfile("Assets/music/infectious.bf");
        BPM = song.quarterBPM;
        offset = song.offset;
        notes_iter = song.notes.OrderBy(note => note.beat);
        onBeat += SpawnNotes;
        beat_time = 60f / (2 * BPM);
        beats_ahead = Mathf.CeilToInt(note_time / beat_time);
        */
        beatLerper = new BeatLerper(events, offset);
        /*
        System.IO.File.WriteAllLines("beats.txt",
            Enumerable.Range(1, 300 * 32)
                .Select(t => beatLerper.TimeFromBeat((float)t / 32).ToString())
                .ToArray());
        */
        /*
        Debug.Log(System.Text.RegularExpressions.Regex.Replace(
        @"
        foo //bar
        baz // qux", 
        @"//[^\n]*$", "", System.Text.RegularExpressions.RegexOptions.Multiline));
        */
        BeatFile.ReadStepfile("Assets/music/Baby Bowser's Lullaby.sm");
    }

    public void PlaySong()
    {
        song_pos = 0;
        audio_source = GetComponent<AudioSource>();
        audio_source.Play(0);
        song_start_time = (float) AudioSettings.dspTime;
        /*
        Note note;
        foreach (NoteData data in notes.OrderBy(n => n.beat))
        {
            //Debug.Log("" + data.beat + " " + data.direction);
            Instantiate(note_prefab).Initialize(GetComponent<AudioPlayer>(), data.beat, data.direction);
        }
        */
        
    }
	
	// Update is called once per frame
	void Update () {
        /*
        int new_pos = (int)((SongTime() - offset) / beat_time);
        if (new_pos != song_pos && !Object.ReferenceEquals(onBeat, null)) {
            onBeat(new_pos);
        }
        song_pos = new_pos;
        */

	}

    private void SpawnNotes(int current_beat)
    {
        foreach (NoteData data in notes_iter.TakeWhile(note => note.beat - current_beat <= beats_ahead))
        {
            Instantiate(note_prefab).Initialize(GetComponent<AudioPlayer>(), data.beat, data.direction);
        }
    }

    public double SongTime ()
    {
        return AudioSettings.dspTime - song_start_time;
    }

    public float TimeOfBeat(float beat_num)
    {
        return offset + beat_time * beat_num;
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
}

public enum BPMEventType { BPMChange, Stop };

[System.Serializable]
public class BPMEvent
{
    public BPMEventType type;
    public float beat;
    public float newBPM;
    public float stopDuration;

    public BPMEvent(BPMEventType type, float beat_, float param)
    {
        beat = beat_;
        if (type == BPMEventType.BPMChange)
        {
            newBPM = param;
        }
        else
        {
            stopDuration = param;
        }
    }
}

public class BeatLerper
{
    [System.Serializable]
    public class BeatLine
    {
        public float start, end, bps, intercept;
        private float offset;

        public BeatLine(float start_, float end_, float bps_, float intercept_, float offset_)
        {
            start = start_; end = end_; bps = bps_; intercept = intercept_; offset = offset_;
        }

        public float BeatFromTime(float time)
        {
            return bps * (time - offset) + intercept;
        }

        public float TimeFromBeat(float beat)
        {
            return (beat - intercept) / bps + offset;
        }

        public bool TimeWithinBounds(float time)
        {
            return (time - offset) > start && (time - offset) <= end;
        }
    }

    private BeatLine[] lines;

    public BeatLerper(BPMEvent[] events_arr, float offset)
    {
        IEnumerable<BPMEvent> events = events_arr.OrderBy(ev => ev.beat);
        List<BeatLine> lines_l = new List<BeatLine>();

        BPMEvent ev1 = events.First();
        if (ev1.type == BPMEventType.Stop)
        {
            Debug.LogException(new System.Exception("First BPM event must be a BPM change"));
        }
        lines_l.Add(new BeatLine(0, float.PositiveInfinity, ev1.newBPM / 60, 0, offset));
        
        foreach (BPMEvent ev in events.Skip(1))
        {
            BeatLine left = lines_l.Last();
            left.end = left.TimeFromBeat(ev.beat);
            lines_l[lines_l.Count - 1] = left;

            if (ev.type == BPMEventType.BPMChange)
            {
                float new_bps = ev.newBPM / 60;
                lines_l.Add(new BeatLine(left.end, float.PositiveInfinity, new_bps, ev.beat - new_bps * left.end, offset));
            }
            else // it's a stop
            {
                lines_l.Add(new BeatLine(left.end, left.end + ev.stopDuration, 0, ev.beat, offset));
                lines_l.Add(new BeatLine(left.end + ev.stopDuration, float.PositiveInfinity, left.bps, ev.beat - left.bps * (left.end + ev.stopDuration), offset));
            }
        }

        lines = lines_l.ToArray();
    }

    public float BeatFromTime(float time)
    {
        return lines.AsEnumerable().Where( line => line.TimeWithinBounds( time ) )
                                   .First( )
                                   .BeatFromTime( time );
    }

    public float TimeFromBeat(float beat)
    {
        return lines.AsEnumerable().Where( line => line.TimeWithinBounds( line.TimeFromBeat( beat ) ) )
                                   .First( )
                                   .TimeFromBeat( beat );
    }
}