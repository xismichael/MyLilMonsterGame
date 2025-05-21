using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

public class RoleClassDatabase : MonoBehaviour
{
    private static RoleClassDatabase theInstance;
    public static RoleClassDatabase Instance
    {
        get
        {
            return theInstance;
        }
    }

    private Dictionary<string, RoleClass> roleClassDict;

    //load database on runtime
    void Awake()
    {
        if (theInstance == null)
        {
            theInstance = this;
            LoadRoleClasses();
            RoleClassManager.Instance.SetDisplaySprite();
        }
    }

    //populate the dictionary
    private void LoadRoleClasses()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("classes");
        roleClassDict = JsonConvert.DeserializeObject<Dictionary<string, RoleClass>>(jsonFile.text);
    }

    //for getting individual enemy data
    public RoleClass GetRoleClass(string name)
    {
        if (roleClassDict.ContainsKey(name)) return roleClassDict[name];
        return null;
    }

    public List<string> GetAllRoleClassNames()
    {
        if (roleClassDict == null) return new List<string>();
        return roleClassDict.Keys.ToList();
    }

    public int GetRoleClassCount()
    {
        if (roleClassDict == null) return 0;
        return roleClassDict.Count;
    }

    public int GetSpriteIcon(string name)
    {
        return GetRoleClass(name).sprite;
    }



}