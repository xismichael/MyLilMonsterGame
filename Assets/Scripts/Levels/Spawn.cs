using System.Collections.Generic;

[System.Serializable]
public class Spawn
{
    public string enemy;
    public string count;
    public string hp = "base";
    public string delay = "2";
    public string damage = "base";
    public string speed = "base";
    public List<int> sequence = new List<int> { 1 };
    public string location;
}