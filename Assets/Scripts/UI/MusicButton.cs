using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MusicButton : MonoBehaviour
{
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;

    private Button _button;
    private Image _image;

    private void Start()
    {
        _button = GetComponent<Button>();
        _image = GetComponent<Image>();
        _button.onClick.AddListener(HandleClick);
        UpdateSprite();
    }

    private void HandleClick()
    {
        AudioManager.Instance.ToggleMusic();
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (AudioManager.Instance._musicSource.mute)
        {
            _image.sprite = offSprite;
        }
        else
        {
            _image.sprite = onSprite;
        }
    }
}
