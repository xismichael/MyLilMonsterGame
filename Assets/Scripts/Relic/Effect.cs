using UnityEngine;

public class Effect
{
    public string Type;           // e.g., "gain-mana", "gain-spellpower"
    public string AmountExpr;     // RPN expression string
    public string Until;          // e.g., "move", "cast-spell", "health-percentage"
    public string Condition;      // used with health-percentage: "above", "below", "equal"
    public float Percentage;      // used with health-percentage

    public string Description;

    private int evaluatedAmount;
    private int revertAmount = 0;
    private Relic parentRelic;

    // Delegate references for safe unsubscription
    private System.Action<Vector2> onMoveHandler;
    private System.Action<Spell> onCastHandler;
    private System.Action<Hittable> onHealthChangeHandler;

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
                player.hp.Heal(evaluatedAmount);
                revertAmount += evaluatedAmount;
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
}
