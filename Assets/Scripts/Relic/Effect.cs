using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Effect
{
    public string Type;           // e.g., "gain-mana", "gain-spellpower"
    public string AmountExpr;     // RPN expression string
    public string Until;          // e.g., "move", "cast-spell", "health-percentage"
    public string Condition;      // used with health-percentage: "above", "below", "equal"
    public float Percentage;      // used with health-percentage

    public string Description;

    public int evaluatedAmount;
    private int revertAmount = 0;
    private Relic parentRelic;

    // Delegate references for safe unsubscription
    private System.Action<Vector2> onMoveHandler;
    private System.Action<Spell> onCastHandler;
    private System.Action<Hittable> onHealthChangeHandler;

    List<Vector3> teleportLocations = new List<Vector3>
    {
        new Vector3(18f, 18f, 0f),
        new Vector3(40f, 18f, 0f),
        new Vector3(60f, -7f, 0f),
        new Vector3(-10f, 0f, 0f)
    };

    public void Apply(PlayerController player, Relic source)
    {
        evaluatedAmount = Mathf.RoundToInt(RPNEvaluator.Evaluate(AmountExpr, player.GetRPNVariables()));
        parentRelic = source;

        switch (Type)
        {
            case "gain-mana":
                player.spellcaster.mana += evaluatedAmount;
                revertAmount += evaluatedAmount;
                break;
            case "gain-spellpower":
                player.spellcaster.power += evaluatedAmount;
                revertAmount += evaluatedAmount;
                break;
            case "gain-speed":
                player.speed += evaluatedAmount;
                revertAmount += evaluatedAmount;
                break;
            case "gain-mana-regen":
                player.spellcaster.mana_reg += evaluatedAmount;
                revertAmount += evaluatedAmount;
                break;
            case "regenerate-hp":
                Debug.Log($"The evaluated amount::::::::: {evaluatedAmount} --------- The amountExpr::::::: {AmountExpr} ========== The player HP before::::::: {player.hp.hp}.");
                player.hp.Heal(evaluatedAmount);
                revertAmount += evaluatedAmount;
                Debug.Log($"The player HP after regenerated ::::::: {player.hp.hp}.");
                break;                
            case "teleport":
                SetTeleportPosition(player);
                break;
            case "damage-speed-up":
                SetSpeed(player, evaluatedAmount);
                break;
        }

        if (!string.IsNullOrEmpty(Until))
        {
            SubscribeToUntilEvent(player);
        }
    }

    private void SubscribeToUntilEvent(PlayerController player)
    {
        switch (Until)
        {
            case "move":
                onMoveHandler = (_) =>
                {
                    Revert(player);
                    player.OnMoveEvent -= onMoveHandler;
                    parentRelic.ResetTrigger();
                };
                player.OnMoveEvent += onMoveHandler;
                break;

            case "cast-spell":
                onCastHandler = (_) =>
                {
                    Revert(player);
                    player.spellcaster.OnSpellCast -= onCastHandler;
                    parentRelic.ResetTrigger();
                };
                player.spellcaster.OnSpellCast += onCastHandler;
                break;

            case "health-percentage":
                onHealthChangeHandler = (hp) =>
                {
                    float current = hp.hp;
                    float max = hp.max_hp;
                    float currentPercentage = current / max;
                    //Debug.Log("current hp: " + current + " max hp: " + max);
                    //Debug.Log("current health percentage: " + currentPercentage + " condition: " + Condition + " Percentage: " + Percentage);
                    bool conditionMet = Condition switch
                    {
                        "below" => currentPercentage < Percentage,
                        "above" => currentPercentage > Percentage,
                        "equal" => Mathf.Approximately(currentPercentage, Percentage),
                        _ => false
                    };

                    if (conditionMet)
                    {
                        Revert(player);
                        player.OnHealthChange -= onHealthChangeHandler;
                        parentRelic.ResetTrigger();
                    }
                };
                player.OnHealthChange += onHealthChangeHandler;
                break;
        }
    }

    public void NextWaveReset(PlayerController player)
    {
        Revert(player);

        // Unsubscribe all possible handlers
        if (onMoveHandler != null)
            player.OnMoveEvent -= onMoveHandler;

        if (onCastHandler != null)
            player.spellcaster.OnSpellCast -= onCastHandler;

        if (onHealthChangeHandler != null)
            player.OnHealthChange -= onHealthChangeHandler;
    }

    public void Revert(PlayerController player)
    {
        switch (Type)
        {
            case "gain-spellpower":
                player.spellcaster.power -= revertAmount;
                break;
            case "gain-speed":
                player.speed -= revertAmount;
                break;
            case "gain-mana-regen":
                player.spellcaster.mana_reg -= revertAmount;
                break;
        }

        revertAmount = 0;
    }

    public void SetTeleportPosition(PlayerController player){
        // Need to obtain the current player in case if the previous player gets destroyed once the levels is changed.
        // Without this, you will get the error: NullReferenceException - Instance of object is null.
        // I have added the code in line 81 on your implemented effects for gainmana and gaintemporaryspell effects. 
        PlayerController control = player.GetComponent<PlayerController>();
        if(control.hp.hp <= (control.hp.max_hp/2)){
            int i = Random.Range(0, teleportLocations.Count);
            control.transform.position = teleportLocations[i];
            //control.transform.position = teleportLocations[0];
            Debug.Log($"[Relic] Player has teleported.");
        } else {
            Debug.Log($"[Relic] Player has NOT teleported. HP ======= {control.hp.hp} ;; MAX_HP ======== {control.hp.max_hp}.");
        }
    }

    /*
    public void AddHP(PlayerController player, Relic source){
        player.hp.hp += source.Effect.evaluatedAmount;
        Debug.Log($"The evaluatedAmount::::: {source.Effect.evaluatedAmount} ;; The player HP:::::: {player.hp.hp}.");
    }
    */

    private void SetSpeed(PlayerController player, int amount){
        PlayerController control = player.GetComponent<PlayerController>();
        control.StartCoroutine(ChangeSpeed(control, amount));
        Debug.Log("The speed has doubled!!!!!!!!!!!!");        
    }

    private IEnumerator ChangeSpeed(PlayerController control, int amount){
        int originalSpeed = control.speed;
        
        control.speed *= amount;
        Debug.Log($"[Relic] Player's speed has increased {amount} for 10 seconds.");
        yield return new WaitForSeconds(10f);

        control.speed = originalSpeed;
        Debug.Log($"[Relic] Player's speed has resetted to its original.");
    }
}