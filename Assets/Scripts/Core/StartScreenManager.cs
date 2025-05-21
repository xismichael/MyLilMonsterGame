using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

public class StartScreenManager
{
    private static StartScreenManager theInstance;
    public static StartScreenManager Instance
    {
        get
        {
            if (theInstance == null)
                theInstance = new StartScreenManager();
            return theInstance;
        }
    }

    private StartScreenManager()
    { }

    public void StartScreen()
    {

    }

}
