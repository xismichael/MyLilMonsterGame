using UnityEngine;
using UnityEngine.UI;             // For Image
using TMPro;

public class RelicRewardUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public RelicRewardUIManager relicRewardUIManager;
    public TMP_Text relicNameText;
    public TMP_Text relicDescriptionText;
    public bool selected;
    public Image relicImage;
    public TMP_Text buttonText;

    private string relicName;

    void Start()
    {
        selected = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (relicRewardUIManager.RelicSelected == relicName) buttonText.text = "UNSELECT";
        else buttonText.text = "SELECT";
    }

    public void SetName(string name)
    {
        relicName = name;
        relicNameText.text = name;
    }

    public void SetDescription(string description)
    {
        relicDescriptionText.text = description;
    }

    public void SetDisplayImage(int sprite)
    {
        GameManager.Instance.relicIconManager.PlaceSprite(sprite, relicImage);
    }

    public void OnButtonClick()
    {
        relicRewardUIManager.RelicSelected = relicName;
    }
}
