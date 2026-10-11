using System.Collections;
using UnityEngine;

public class MusicManagerScript : MonoBehaviour
{
    public static MusicManagerScript instance;
    [SerializeField] MusicTrackScript musicTrack;
    public AudioSource musicSource;
    [SerializeField] float musicVolume;
    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
    }
    public void PlayMusic(string trackName, float fadeDuration)
    {
        StartCoroutine(AnimateMusicCrossFade(musicTrack.GetClipFromName(trackName), fadeDuration));
    }
    IEnumerator AnimateMusicCrossFade(AudioClip nextTrack, float fadeDuration)
    {
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            musicSource.volume = Mathf.Lerp(musicVolume, 0, percent);
            yield return null;
        }
        musicSource.clip = nextTrack;
        musicSource.Play();
        percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            musicSource.volume = Mathf.Lerp(0, musicVolume, percent);
            yield return null;
        }
    }
    public void StopMusic()
    {
        StopAllCoroutines();
        StartCoroutine(StopMusicCrossfade(0.5f));
    }
    IEnumerator StopMusicCrossfade(float fadeDuration)
    {
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            musicSource.volume = Mathf.Lerp(musicVolume, 0, percent);
            yield return null;
        }
        musicSource.Stop();
    }
}
