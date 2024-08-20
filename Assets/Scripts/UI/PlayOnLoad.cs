using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayOnLoad : MonoBehaviour
{
    [SerializeField] AudioClip soundClip;

    private void Start()
    {
        AudioManager.Instance.PlayEffect(soundClip);
    }
}
