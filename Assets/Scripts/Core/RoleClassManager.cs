using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RoleClassManager : MonoBehaviour
{
    private static RoleClassManager theInstance;
    public PlayerController player;
    public Image DisplaySprite;
    public TMP_Text RoleNameText;
    public int currentRoleIndex = 0;
    public string roleName;

    public static RoleClassManager Instance
    {
        get
        {
            return theInstance;
        }
    }

    void Awake()
    {
        if (theInstance == null)
        {
            theInstance = this;
        }
    }

    void Update()
    {
        SetDisplaySprite();
    }

    public void LeftButtonPress()
    {
        currentRoleIndex = currentRoleIndex - 1;
        if (currentRoleIndex < 0)
        {
            currentRoleIndex = RoleClassDatabase.Instance.GetRoleClassCount() - 1;
        }
    }

    public void RightButtonPress()
    {
        currentRoleIndex = (currentRoleIndex + 1) % RoleClassDatabase.Instance.GetRoleClassCount();
    }

    public void SelectButtonPress()
    {
        player.role = roleName;
        gameObject.SetActive(false);
        EnemySpawner.Instance.restartScreen();
    }

    public void SetDisplaySprite()
    {
        GameManager.Instance.playerSpriteManager.PlaceSprite(currentRoleIndex, DisplaySprite);
        roleName = RoleClassDatabase.Instance.GetAllRoleClassNames()[currentRoleIndex];
        RoleNameText.text = roleName;
    }

}