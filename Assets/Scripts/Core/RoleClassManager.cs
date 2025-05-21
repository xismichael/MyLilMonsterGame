using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine.UI;

public class RoleClassManager : MonoBehaviour
{
    private static RoleClassManager theInstance;
    public GameObject DisplaySprite;
    public Button LeftButton;
    public Button RightButton;
    public Button SelectBtton;
    public int currentRoleIndex;

    public List<string> roleNames;

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

        roleNames = RoleClassDatabase.Instance.GetAllRoleClassNames();
        currentRoleIndex = 0;
    }




}