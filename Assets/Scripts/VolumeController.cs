using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VolumeController : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider = null;

    [SerializeField] private TextMeshProUGUI volumeTextUI = null;
    
    // Start is called before the first frame update
    void Start()
    {
        if (volumeSlider != null) {
            volumeSlider.value = 100;
            Debug.Log(volumeSlider);
            AudioListener.volume = volumeSlider.value/100;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (volumeSlider != null) {
            AudioListener.volume = volumeSlider.value/100;
            Debug.Log(volumeSlider);
        }
    }

    public void VolumeSlider(float volume)
    {
        volumeTextUI.text = volume.ToString("0");
    }
}
