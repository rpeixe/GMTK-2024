using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMusic : MonoBehaviour
{
    [SerializeField] private AudioClip _music;

    void Start()
    {
        AudioManager.Instance.PlayMusic(_music);
    }
}
