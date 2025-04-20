using System.Collections.Generic;

[System.Serializable]
public class Level
{
    public string name;
    public int waves; // will default to 0 if not present
    public List<Spawn> spawns;
}