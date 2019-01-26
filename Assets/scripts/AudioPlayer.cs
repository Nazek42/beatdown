using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum NoteType { down = 0, right = 1, up = 2, left = 3 };

[System.Serializable]
public class NoteData
{
    public double beat;
    public NoteType direction;

    public NoteData(double beat_, NoteType direction_)
    {
        beat = beat_;
        direction = direction_;
    }
}

public class AudioPlayer : MonoBehaviour {
    AudioSource audio_source;

    public string path;
    public string simfileName;

    [SerializeField] private NoteController[] boards;
    
    public BeatLerper beatLerper { get; private set; }

    private bool playing = false;
    private bool ready = false;

    private Song song;
    private IEnumerable<NoteData> notes_iter;

    private double old_beat;
    private double song_start_time;

    public delegate void OnBeat(double beat);
    public OnBeat onBeat;

    // Use this for initialization
    void Start()
    {
        audio_source = GetComponent<AudioSource>();
        /*
        System.IO.File.WriteAllLines("beats.txt",
            Enumerable.Range(1, 300 * 32)
                .Select(t => beatLerper.TimeFromBeat((double)t / 32).ToString())
                .ToArray());
        */
        song = BeatFile.ReadStepfile("Assets/Resources/music/" + path + "/" + simfileName);
        string audio_path = "music/" + path + "/" + System.IO.Path.GetFileNameWithoutExtension(song.metadata["MUSIC"]);
        Debug.Log(audio_path);
        audio_source.clip = Resources.Load<AudioClip>(audio_path);
        beatLerper = new BeatLerper(song.bpmEvents, song.offset);
        notes_iter = song.notes.OrderBy(note => note.beat);
        //onBeat += SpawnNotes;
        //onBeat += AnimateLines;
        ready = true;

        foreach (NoteController board in boards)
        {
            board.Initialize(this, notes_iter);
        }

        Debug.Log("Ready.");
    }

    private static void Noop(double _) { }

    public void PlaySong()
    {
        Debug.Log(audio_source.clip);
        audio_source.Play(0);
        song_start_time = (double) AudioSettings.dspTime;
        playing = true;
        Debug.Log("Playing!");
    }

    // Update is called once per frame
    void Update()
    {
        if (playing && SongTime() > song.offset)
        {
            double new_beat = SongBeat();
            if (System.Math.Truncate(new_beat) != System.Math.Truncate(old_beat) && onBeat != null)
            {
                onBeat(new_beat);
            }
            old_beat = new_beat;
        }
    }
    /*
    private void SpawnNotes(double current_beat)
    {
        //Debug.Log("hello from SpawnNotes");
        foreach (NoteData note in notes_iter.Where(note => (beatLerper.TimeFromBeat(note.beat) - SongTime()) < (note_time + note_pre_time)))
        {
            //Debug.Log("spawning note for beat " + note.beat);
            Instantiate(note_prefab).Initialize(GetComponent<AudioPlayer>(), note.beat, note.direction);
        }
    }

    private void AnimateLines(double _)
    {
        StartCoroutine(LineBulge());
    }

    private IEnumerator LineBulge()
    {
        topLine.widthMultiplier = .15f;
        bottomLine.widthMultiplier = .15f;
        yield return new WaitForSeconds(0.1f);
        topLine.widthMultiplier = .1f;
        bottomLine.widthMultiplier = .1f;
    }
    */
    public double SongTime ()
    {
        return AudioSettings.dspTime - song_start_time;
    }

    public double SongBeat()
    {
        return beatLerper.BeatFromTime(SongTime());
    }
}

public enum BPMEventType { BPMChange, Stop };

[System.Serializable]
public class BPMEvent
{
    public BPMEventType type;
    public double beat;
    public double newBPM;
    public double stopDuration;

    public BPMEvent(BPMEventType type, double beat_, double param)
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

// DO NOT MESS WITH THIS CLASS FOR THE LOVE OF PALUTENA
public class BeatLerper
{
    [System.Serializable]
    public class BeatLine
    {
        public double start, end, bps, intercept, offset;

        public BeatLine(double start_, double end_, double bps_, double intercept_, double offset_)
        {
            start = start_; end = end_; bps = bps_; intercept = intercept_; offset = offset_;
        }

        public double BeatFromTime(double time)
        {
            return bps * (time + offset) + intercept;
        }

        public double TimeFromBeat(double beat)
        {
            return (beat - intercept) / bps - offset;
        }

        public bool TimeWithinBounds(double time)
        {
            return (time - offset) > start && (time - offset) <= end;
        }
    }

    private BeatLine[] lines;

    public BeatLerper(BPMEvent[] events_arr, double offset)
    {
        IEnumerable<BPMEvent> events = events_arr.OrderBy(ev => ev.beat);
        List<BeatLine> lines_l = new List<BeatLine>();

        BPMEvent ev1 = events.First();
        if (ev1.type == BPMEventType.Stop)
        {
            Debug.LogException(new System.Exception("First BPM event must be a BPM change"));
        }
        lines_l.Add(new BeatLine(0, double.PositiveInfinity, ev1.newBPM / 60, 0, offset));
        
        foreach (BPMEvent ev in events.Skip(1))
        {
            BeatLine left = lines_l.Last();
            left.end = left.TimeFromBeat(ev.beat);
            lines_l[lines_l.Count - 1] = left;

            if (ev.type == BPMEventType.BPMChange)
            {
                double new_bps = ev.newBPM / 60;
                lines_l.Add(new BeatLine(left.end, double.PositiveInfinity, new_bps, ev.beat - new_bps * left.end, offset));
            }
            else // it's a stop
            {
                lines_l.Add(new BeatLine(left.end, left.end + ev.stopDuration, 0, ev.beat, offset));
                lines_l.Add(new BeatLine(left.end + ev.stopDuration, double.PositiveInfinity, left.bps, ev.beat - left.bps * (left.end + ev.stopDuration), offset));
            }
        }

        lines = lines_l.ToArray();
    }

    public double BeatFromTime(double time)
    {
        return lines.AsEnumerable().Where( line => line.TimeWithinBounds( time ) )
                                   .First( )
                                   .BeatFromTime( time );
    }

    public double TimeFromBeat(double beat)
    {
        return lines.AsEnumerable().Where( line => line.TimeWithinBounds( line.TimeFromBeat( beat ) ) )
                                   .First( )
                                   .TimeFromBeat( beat );
    }
}