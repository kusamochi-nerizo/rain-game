using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BgmDefine
{
    public float loopTime;
    public float endTime;
}

[Serializable]
public class SoundVolume
{
    public float bgm = 1.0f;
    public float se = 1.0f;

    public bool mute = false;

    public void Reset()
    {
        bgm = 1.0f;
        se = 1.0f;
        mute = false;
    }
}

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    public class Handle
    {
        public float volume = 1.0f;
        public float fadeDuration = 1.0f;
        public long frame = 0;
        public int index = -1;
        public BgmDefine bgmDefine;

        private bool isValid = false;

        private Coroutine fadeInCoroutineHandle = null;
        private Coroutine fadeOutCoroutineHandle = null;

        public bool IsValid
        {
            get { return isValid; }
            set { isValid = value; }
        }

        private Action completeCallback = null;
        private Action fadeOutCallback = null;

        public void FadeIn()
        {
            fadeInCoroutineHandle = Instance.StartCoroutine(fadeIn());
        }

        public void FadeOut()
        {
            FadeOut(null);
        }

        public void FadeOut(Action fadeout_callback)
        {
            fadeOutCallback = fadeout_callback;
            fadeOutCoroutineHandle = Instance.StartCoroutine(fadeOut());
        }

        public void ResetParams()
        {
            volume = 1.0f;
            fadeDuration = 1.0f;
            frame = 0;
            bgmDefine = null;
            if (fadeInCoroutineHandle != null)
            {
                Instance.StopCoroutine(fadeInCoroutineHandle);
                fadeInCoroutineHandle = null;
            }

            if (fadeOutCoroutineHandle != null)
            {
                Instance.StopCoroutine(fadeOutCoroutineHandle);
                fadeOutCoroutineHandle = null;
            }
        }

        public void SetBgmLoop(float loop_time, float end_time)
        {
            var s = Instance.GetBgmAudioSource();
            if (bgmDefine == null)
            {
                bgmDefine = new BgmDefine();
            }

            bgmDefine.loopTime = loop_time;
            bgmDefine.endTime = end_time;
        }

        public void OnComplete(Action action)
        {
            completeCallback = action;
        }


        private IEnumerator fadeIn()
        {
            while (volume < 1.0f)
            {
                volume += (1.0f / fadeDuration) * Time.fixedDeltaTime;
                yield return null;
            }

            volume = 1.0f;
            fadeInCoroutineHandle = null;
        }

        private IEnumerator fadeOut()
        {
            while (volume > 0.0f)
            {
                volume -= (1.0f / fadeDuration) * Time.fixedDeltaTime;
                yield return null;
            }

            volume = 0.0f;
            fadeOutCoroutineHandle = null;
            if (fadeOutCallback != null)
            {
                fadeOutCallback();
                fadeOutCallback = null;
            }
        }

        //for SoundManager
        public void callCompleteCallback()
        {
            if (completeCallback != null)
            {
                completeCallback();
            }
        }
    }

    [SerializeField] private int numChannel = 16;

    [SerializeField] private string[] targetSeFolders = { "SE" };

    [SerializeField] private string[] targetBgmFolders = { "BGM" };

    [SerializeField] private SoundVolume volume = new SoundVolume();

    public SoundVolume Volume
    {
        get { return volume; }
        set { volume = value; }
    }

    [SerializeField] protected bool isDebugLog = false;

#if UNITY_EDITOR
    [SerializeField] protected float debugBgmTime = 0.0f;
#endif

    private AudioClip[] seClips;

    private AudioClip[] bgmClips;
    private AudioSource bgmSource;

    private Dictionary<string, int> seIndexes = new Dictionary<string, int>();
    private Dictionary<string, int> bgmIndexes = new Dictionary<string, int>();

    private Handle bgmHandle = new Handle();

    private AudioSource[] seSources;
    private Dictionary<Handle, AudioSource> seHandles = new Dictionary<Handle, AudioSource>();

    private const string cSaveMuteKey = "SoundManagerDataMuteKey";
    private const string cSaveSeVolumeKey = "SoundManagerDataSeVolumeKey";
    private const string cSaveBgmVolumeKey = "SoundManagerDataBgmVolumeKey";

    //------------------------------------------------------------------------------
    protected override void doAwake()
    {
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;

        seSources = new AudioSource[numChannel];
        for (int i = 0; i < seSources.Length; i++)
        {
            seSources[i] = gameObject.AddComponent<AudioSource>();
            seHandles[new Handle()] = seSources[i];
        }

        seClips = new AudioClip[0]; // <= Required initialize to use Concat(Linq).

        foreach (var folder in targetSeFolders)
        {
            var path = "Audio/" + folder;
            seClips = seClips.Concat(Resources.LoadAll<AudioClip>(path)).ToArray();
        }

        Debug.Assert(!checkDuplications(seClips), "Detected duplicated file name(s)!");


        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmClips = new AudioClip[0];
        foreach (var folder in targetBgmFolders)
        {
            var path = "Audio/" + folder;
            bgmClips = bgmClips.Concat(Resources.LoadAll<AudioClip>(path)).ToArray();
        }

        Debug.Assert(!checkDuplications(bgmClips), "Detected duplicated file name(s)!");

        for (int i = 0; i < seClips.Length; ++i)
        {
            seIndexes[seClips[i].name] = i;
        }

        for (int i = 0; i < bgmClips.Length; ++i)
        {
            bgmIndexes[bgmClips[i].name] = i;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    //------------------------------------------------------------------------------
    void Update()
    {
        bgmSource.mute = volume.mute;
        foreach (var source in seSources)
        {
            source.mute = volume.mute;
        }

        bgmSource.volume = volume.bgm * bgmHandle.volume;
        if (bgmHandle.bgmDefine != null && bgmSource.isPlaying)
        {
            if (bgmSource.time >= bgmHandle.bgmDefine.endTime)
            {
                bgmSource.time = bgmHandle.bgmDefine.loopTime;
            }
        }

        if (bgmSource.IsFinished())
        {
            bgmHandle.callCompleteCallback();
        }

        bgmHandle.IsValid = !bgmSource.IsFinished();

        foreach (var pair in seHandles)
        {
            if (pair.Key.IsValid)
            {
                pair.Value.volume = volume.se * pair.Key.volume;
                if (pair.Value.IsFinished())
                {
                    pair.Key.callCompleteCallback();
                }
            }

            pair.Key.IsValid = !pair.Value.IsFinished();
        }
    }

    public void DefaultSaveSettings()
    {
        PlayerPrefs.SetInt(cSaveMuteKey, volume.mute ? 1 : 0);
        PlayerPrefs.SetFloat(cSaveSeVolumeKey, volume.se);
        PlayerPrefs.SetFloat(cSaveBgmVolumeKey, volume.bgm);
    }

    public void DefaultLoadSettings()
    {
        volume.mute = PlayerPrefs.GetInt(cSaveMuteKey, 0) == 1;
        volume.se = PlayerPrefs.GetFloat(cSaveSeVolumeKey, 1.0f);
        volume.bgm = PlayerPrefs.GetFloat(cSaveBgmVolumeKey, 1.0f);
    }

    //------------------------------------------------------------------------------
    public int GetSeIndex(string name)
    {
        return seIndexes[name];
    }

    //------------------------------------------------------------------------------
    public int GetBgmIndex(string name)
    {
        return bgmIndexes[name];
    }

    //------------------------------------------------------------------------------
    public Handle PlayBgm(string name)
    {
        int index = bgmIndexes[name];
        return PlayBgm(index);
    }

    //------------------------------------------------------------------------------
    public Handle PlayBgm(int index)
    {
        if (0 > index || bgmClips.Length <= index)
        {
            return null;
        }

        if (bgmSource.clip == bgmClips[index])
        {
            return bgmHandle;
        }

        bgmSource.Stop();
        bgmSource.clip = bgmClips[index];
        bgmSource.loop = true;
        bgmSource.Play();

        bgmHandle.ResetParams();
        bgmHandle.frame = Time.frameCount;
        bgmHandle.IsValid = true;
        bgmHandle.index = index;

        return bgmHandle;
    }

    //------------------------------------------------------------------------------
    public Handle GetBgmHandle()
    {
        return bgmHandle;
    }

    //------------------------------------------------------------------------------
    public void StopBgm()
    {
        bgmSource.Stop();
        bgmSource.clip = null;
    }


    //------------------------------------------------------------------------------
    public bool IsBgmPlaying
    {
        get { return bgmSource.isPlaying; }
    }

    //------------------------------------------------------------------------------
    public Handle PlaySe(string name, float delay)
    {
        DebugLog("PlaySe : " + name);
        int index = GetSeIndex(name);
        Handle h = seHandles.FirstOrDefault(x => (x.Value.clip == seClips[index]) && (x.Key.frame == Time.frameCount))
            .Key;
        if (h == null)
        {
            return null;
        }

        StartCoroutine(DelayMethod(delay, () => PlaySe(index)));
        return h;
    }

    //------------------------------------------------------------------------------
    public Handle PlaySe(string name)
    {
        DebugLog("PlaySe : " + name);
        return PlaySe(GetSeIndex(name));
    }

    //------------------------------------------------------------------------------
    public Handle PlaySe(int index)
    {
        if (0 > index || seClips.Length <= index)
        {
            return null;
        }

        //同一フレームでの重複再生回避
        //二回ループは一回ループにまとめられるが、
        //可読性重視で二回ループにしておく
        foreach (var k in seHandles)
        {
            AudioSource source = k.Value;
            Handle handle = k.Key;
            if (source.clip == seClips[index] &&
                handle.frame == Time.frameCount)
            {
                return handle;
            }
        }

        foreach (var k in seHandles)
        {
            AudioSource source = k.Value;
            Handle handle = k.Key;
            if (source.IsFinished())
            {
                handle.ResetParams();
                source.clip = seClips[index];
                source.Play();
                source.loop = false;
                handle.frame = Time.frameCount;
                handle.IsValid = true;
                handle.index = index;
                return handle;
            }
        }

        return null;
    }

    //------------------------------------------------------------------------------
    //たまにSEをループさせたいことがある
    public void SetEnableSeLoop(Handle handle, bool enable)
    {
        AudioSource source = seHandles[handle];
        source.loop = enable;
    }

    //------------------------------------------------------------------------------
    public void StopSe(Handle handle)
    {
        AudioSource source = seHandles[handle];
        source.Stop();
        source.clip = null;
    }

    //------------------------------------------------------------------------------
    public void StopSe()
    {
        foreach (AudioSource source in seSources)
        {
            source.Stop();
            source.clip = null;
        }
    }

    //------------------------------------------------------------------------------
    public AudioSource GetBgmAudioSource()
    {
        return bgmSource;
    }

    //------------------------------------------------------------------------------
    public void DebugLog(string str)
    {
        if (isDebugLog && Debug.isDebugBuild)
        {
            Debug.Log(str);
        }
    }

    //------------------------------------------------------------------------------
    public AudioSource GetSeAudioSource(Handle handle)
    {
        return seHandles[handle];
    }

    //------------------------------------------------------------------------------
    private bool checkDuplications(AudioClip[] clips)
    {
        var duplicates = clips.GroupBy(audioClip => audioClip.name)
            .Where(name => name.Count() > 1)
            .Select(group => group.Key).ToList();
        return duplicates.Count != 0;
    }

    //------------------------------------------------------------------------------
    private IEnumerator DelayMethod(float wait_time, Action action)
    {
        yield return new WaitForSeconds(wait_time);
        action();
    }
}