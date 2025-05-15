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



}