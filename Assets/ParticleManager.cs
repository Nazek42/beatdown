using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    private AudioPlayer audio;
    private List<GameObject> activeEffects;

    [System.Serializable]
    public class EffectMapping
    {
        public Effect effect;
        public GameObject prefab;
    }
    [SerializeField] private EffectMapping[] effectPrefabs;

    private Dictionary<Effect, GameObject> prefabs;

    private readonly Dictionary<string, CannedEffect> vfxLibrary = new Dictionary<string, CannedEffect>()
    {
        {"high_clash", new CannedEffect(Effect.ClashSpark, new Vector2(1.3f,2f), 2.5f) },
        {"low_clash", new CannedEffect(Effect.ClashSpark, new Vector2(1.3f,-2f), 2.5f) },
        {"mid_clash", new CannedEffect(Effect.ClashSpark, new Vector2(1.3f,0f), 2.5f) },
        {"light_high_hit", new CannedEffect(Effect.LightImpact, new Vector2(0.75f, 2f), 3f) },
        {"light_mid_hit", new CannedEffect(Effect.LightImpact, new Vector2(0.75f, 0f), 3f) },
        {"light_low_hit", new CannedEffect(Effect.LightImpact, new Vector2(0.75f, -2f), 3f) },
        {"heavy_high_hit", new CannedEffect(Effect.HeavyImpact, new Vector2(0.75f, 2f), 3f) },
        {"heavy_mid_hit", new CannedEffect(Effect.HeavyImpact, new Vector2(0.75f, 0f), 3f) },
        {"heavy_low_hit", new CannedEffect(Effect.HeavyImpact, new Vector2(0.75f, -2f), 3f) },
    };

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioPlayer>();
        activeEffects = new List<GameObject>();
        prefabs = new Dictionary<Effect, GameObject>();
        foreach (EffectMapping map in effectPrefabs)
        {
            prefabs.Add(map.effect, map.prefab);
        }
        audio.onBeat += ClearActiveEffects;
    }

    void ClearActiveEffects(double _)
    {
        foreach (GameObject effect in activeEffects)
        {
            Destroy(effect);
        }
        activeEffects.Clear();
    }

    public GameObject SpawnEffectOnPlayer(string effect_name, Player player, Player other)
    {
        if (effect_name == "beam")
        {
            float start = player.right_facing ? 1f : -1f;
            float end = (other.transform.position - player.transform.position).x;
            float center = (start + end) / 2;
            float xscale = Mathf.Abs(end - start) * 20;
            CannedEffect can = new CannedEffect(Effect.Fireball, new Vector2(center, 0), new Vector2(xscale, 3) / player.transform.localScale);
            return SpawnCannedOnPlayer(can, player, other);
        }
        return SpawnCannedOnPlayer(vfxLibrary[effect_name], player, other);
    }

    public GameObject SpawnCannedOnPlayer(CannedEffect can, Player player, Player other)
    {
        GameObject obj = Instantiate(prefabs[can.effect]);
        obj.transform.parent = player.transform;
        obj.transform.localPosition = can.pos;
        obj.transform.localPosition *= new Vector2(1, player.right_facing ? 1 : -1);
        obj.transform.localRotation = player.right_facing ? Quaternion.identity : Quaternion.Euler(0f, 0f, 180f);
        obj.transform.localScale = can.scale;
        activeEffects.Add(obj);
        return obj;
    }

    // Update is called once per frame
    void Update()
    {

    }
}

public enum Effect
{
    DustCloud,
    ClashSpark,
    EmitSpark,
    Fireball,
    LightImpact,
    HeavyImpact,
    Explosion
}

[System.Serializable]
public class CannedEffect
{
    public Effect effect;
    public Vector2 pos;
    public Vector2 scale;

    public CannedEffect(Effect effect_, Vector2 pos_, Vector2 const_scale)
    {
        effect = effect_;
        pos = pos_;
        scale = const_scale;
    }

    public CannedEffect(Effect effect_, Vector2 pos_, float const_scale)
    {
        effect = effect_;
        pos = pos_;
        scale = new Vector2(const_scale, const_scale);
    }
}