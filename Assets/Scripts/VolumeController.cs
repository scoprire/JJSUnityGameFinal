using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VolumeController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        volumeSlider.value = 50;
        AudioListener.volume = volumeSlider.value;
    }

    // Update is called once per frame
    void Update()
    {
        AudioListener.volume = volumeSlider.value;
    }

    [SerializeField] private Slider volumeSlider = null;

    [SerializeField] private TextMeshProUGUI volumeTextUI = null;

    public void VolumeSlider(float volume)
    {

        volumeTextUI.text = volume.ToString("0");
    }
}
