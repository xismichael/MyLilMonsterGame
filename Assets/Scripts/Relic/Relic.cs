using System.Collections.Generic;
using UnityEngine;

public class Relic
{
    public string Name;
    public int SpriteIndex;
    public Trigger Trigger;
    public Effect Effect;
    public string Description;

    private GameManager.GameState prevGameState = GameManager.GameState.PREGAME;

    private bool hasTriggered = false;

    public void TryActivate(PlayerController player, Dictionary<string, object> parameters)
    {
        if(prevGameState != GameManager.GameState.WAVEEND && GameManager.Instance.state == GameManager.GameState.WAVEEND){
            Effect.Apply(player, this);
        }
        prevGameState = GameManager.Instance.state;

        if (GameManager.Instance.state == GameManager.GameState.PREGAME ||
            GameManager.Instance.state == GameManager.GameState.WAVEEND ||
            GameManager.Instance.state == GameManager.GameState.GAMEOVER)
        {
            Effect.NextWaveReset(player);
            return;
        }
        if (!hasTriggered && Trigger.ShouldActivate(parameters))
        {
            hasTriggered = false;
            Effect.Apply(player, this);
            Debug.Log(Name + " is triggered");
        }
    }

    public void ResetTrigger()
    {
        hasTriggered = false;
    }

    public void SetDescription()
    {
        Description = Trigger.Description + " " + Effect.Description;
    }
}