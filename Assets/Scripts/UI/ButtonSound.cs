using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    [SerializeField] AudioClip soundClip;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(HandleClick);
    }

    private void HandleClick()
    {
        AudioManager.Instance.PlayEffect(soundClip);
    }
}
