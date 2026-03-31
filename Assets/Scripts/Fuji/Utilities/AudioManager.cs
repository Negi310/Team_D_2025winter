
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I { get; private set; }

    [Header("Mixer")]
    [SerializeField] private AudioMixer _audioMixer;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _seSource;
    [SerializeField] private AudioSource _bgmSource;
    [Header("Data")]
    [SerializeField] private List<SE> _seList;
    [SerializeField] private List<BGM> _bgmList;

    private Dictionary<SE.Name, AudioClip> _seDict;
    private Dictionary<BGM.Name, AudioClip> _bgmDict;

    private void Awake()
    {
        // singleton pattern
        if (I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // dictionaries 初期化
        _seDict = new Dictionary<SE.Name, AudioClip>();
        foreach (var se in _seList)
        {
            if (se.SeClip != null && !_seDict.ContainsKey(se.SeName))
                _seDict.Add(se.SeName, se.SeClip);
        }

        _bgmDict = new Dictionary<BGM.Name, AudioClip>();
        foreach (var bgm in _bgmList)
        {
            if (bgm.BgmClip != null && !_bgmDict.ContainsKey(bgm.BgmName))
                _bgmDict.Add(bgm.BgmName, bgm.BgmClip);
        }
    }
    
    // SE 再生
    public void PlaySE(SE.Name name, AudioSource overrideSource = null)
    {
        if (!_seDict.TryGetValue(name, out AudioClip clip) || clip == null)
        {
            Debug.LogWarning($"[AudioManager] SE '{name}' not found.");
            return;
        }

        var src = overrideSource ?? _seSource;
        if (src == null) return;

        src.PlayOneShot(clip);
    }

    // ★変更: 引数を BGM.Name に変更
    public void PlayBGM(BGM.Name name, float fadeTime = 0.2f)
    {
        if (!_bgmDict.TryGetValue(name, out AudioClip clip) || clip == null)
        {
            Debug.LogWarning($"[AudioManager] BGM '{name}' not found.");
            return;
        }
        Debug.Log($"[AudioManager] BGM '{name}' is playing.");
        StopAllCoroutines();
        StartCoroutine(FadeBGM(clip, fadeTime));
    }

    private IEnumerator FadeBGM(AudioClip newClip, float fadeTime)
    {
        float startVolume = _bgmSource.volume;

        // フェードアウト
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            _bgmSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeTime);
            yield return null;
        }

        _bgmSource.clip = newClip;
        _bgmSource.Play();

        // フェードイン
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            _bgmSource.volume = Mathf.Lerp(0f, 1f, t / fadeTime);
            yield return null;
        }

        _bgmSource.volume = 0.25f;
    }

    public void StopBGM()
    {
        _bgmSource.Stop();
    }
}

[System.Serializable]
public class SE
{
    public enum Name
    {
        Hit,
        Explosion,
        Click,
        ScoreAdd,
        Rolling,
        Jump,
        BadReaction,
        NiceReaction,
        GreatReaction,
        ExcellentReaction,
        Start,
        Slide,
        Finish,
        
    }
    
    public Name SeName;
    public AudioClip SeClip;
}

[System.Serializable]
public class BGM
{
    public enum Name
    {
        Stage_1,
        Stage_2,
        Stage_3,
        Stage_4,
        Stage_5,
        Title,
        StageSelect,
        Result,
        GameOver,
        Sllow,
    }
    
    public Name BgmName;
    public AudioClip BgmClip;
}
