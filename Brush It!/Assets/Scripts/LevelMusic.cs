using UnityEngine;

public class LevelMusic : MonoBehaviour
{
    [SerializeField] private AudioClip song;

    void Start()
    {
        MusicManager.Instance.Play(song, 2);
    }
}
