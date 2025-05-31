using UnityEngine;
using TMPro;

public class SoundToggle : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    private bool sound = true;

    void Start()
    {
        sound = !AudioListener.pause;
        UpdateSoundButton();
        sound = PlayerPrefs.GetInt("SoundMuted", 0) == 0;
        AudioListener.pause = !sound;
        UpdateSoundButton();
    }

    public void ToggleSound()
    {
        sound = !sound;
        AudioListener.pause = !sound;
        UpdateSoundButton();
        PlayerPrefs.SetInt("SoundMuted", sound ? 0 : 1);
    }

    void UpdateSoundButton()
    {
        if (sound)
        {
            buttonText.text = "SOUND: ON";
            Debug.Log("sound enabled");
        } else
        {
            buttonText.text = "SOUND: OFF";
            Debug.Log("sound disabled");
        }
    }
}
