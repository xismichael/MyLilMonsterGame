using System.Collections.Generic;

[System.Serializable]
public class Level
{
    public string name;
    public int waves = -1;
    public List<Spawn> spawns;
}