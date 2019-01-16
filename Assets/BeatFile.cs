﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

[System.Serializable]
public class Song
{
    public string name;
    public string artist;
    public float offset;
    public Dictionary<string, string> metadata;
    public BPMEvent[] bpmEvents;
    public NoteData[] notes;
}

public class BeatFile {
    private static readonly Dictionary<char, NoteType> directionMap = new Dictionary<char, NoteType>()
    {
        {'<', NoteType.left},
        {'v', NoteType.down},
        {'^', NoteType.up},
        {'>', NoteType.right}
    };

    public static Song ParseStepfile(string smtext)
    {
        Song song = new Song();
        // Remove comments
        smtext = Remove(smtext, @"//[^\n]*$", RegexOptions.Multiline);

        Dictionary<string, string> tags = new Dictionary<string, string>();

        foreach (Match tag in Regex.Matches(smtext, @"#([^;:]+):([^;]*);"))
        {
            string name = tag.Groups[1].Captures[0].Value;
            string value = tag.Groups[2].Captures[0].Value;
            tags[name] = value;
        }

        song.name   = tags.ContainsKey("TITLE")  ? tags["TITLE"]               : "[no title]";
        song.artist = tags.ContainsKey("ARTIST") ? tags["ARTIST"]              : "[no artist]";
        song.offset = tags.ContainsKey("OFFSET") ? float.Parse(tags["OFFSET"]) : 0;
        song.metadata = new Dictionary<string, string>(tags);

        song.bpmEvents = ParseBPMsAndStops(tags["BPMS"], tags["STOPS"]).OrderBy(ev => ev.beat).ToArray();

        return song;
    }

    public static Song ReadStepfile(string path)
    {
        return ParseStepfile(File.ReadAllText(path));
    }

    public static IEnumerable<BPMEvent> ParseBPMsAndStops(string bpmTag, string stopTag)
    {
        IEnumerable<BPMEvent> events;

        bpmTag = Remove(bpmTag, @"\s+");
        events = Regex.Matches(bpmTag, @"([0-9.]+)=([0-9.]+)").Cast<Match>().Select(match => ParseBPMEvent(match, BPMEventType.BPMChange));

        stopTag = Remove(stopTag, @"\s+");
        events = events.Concat(Regex.Matches(stopTag, @"([0-9.]+)=([0-9.]+)").Cast<Match>().Select(match => ParseBPMEvent(match, BPMEventType.BPMChange));

        return events;
    }

    private static BPMEvent ParseBPMEvent(Match ev, BPMEventType type)
    {
        return new BPMEvent(type, float.Parse(ev.Groups[1].Value),
                                  float.Parse(ev.Groups[2].Value));
    }

    public static string Remove(string input, string regex)
    {
        return Regex.Replace(input, regex, "");
    }

    public static string Remove(string input, string regex, RegexOptions options)
    {
        return Regex.Replace(input, regex, "", options);
    }

    /*
    public static Song ParseBeatfile(string bftext)
    {
        Song song = new Song();
        IEnumerable<string> lines = bftext.Split(new[] { System.Environment.NewLine }, System.StringSplitOptions.None).AsEnumerable();

        IEnumerator<string> metadata = lines.Take(4).GetEnumerator();
        metadata.MoveNext();
        song.name = metadata.Current;
        metadata.MoveNext();
        song.artist = metadata.Current;
        metadata.MoveNext();
        song.quarterBPM = float.Parse(metadata.Current);
        metadata.MoveNext();
        song.offset = float.Parse(metadata.Current);

        IEnumerable<NoteData> notes_enum = new NoteData[]{}.AsEnumerable();
        foreach (string[] line in lines.Skip(4).Select(l => l.Split(new char[] { ' ' }))) {
            if (line[0][0] != '#')
            {
                int beat = int.Parse(line[0]);
                notes_enum = notes_enum.Concat(line[1].AsEnumerable().Select(arrow => new NoteData(beat, directionMap[arrow])));
            }
        }
        song.notes = notes_enum.ToArray();

        return song;
    }

    public static Song ReadBeatfile(string path)
    {
        return ParseBeatfile(File.ReadAllText(path));
    }
    */
}
