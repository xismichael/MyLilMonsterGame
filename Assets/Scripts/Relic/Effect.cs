using UnityEngine;

public class Effect
{
    public string Type;           // e.g., "gain-mana", "gain-spellpower"
    public string AmountExpr;     // RPN expression string
    public string Until;          // e.g., "move", "cast-spell"
    private int evaluatedAmount;

    private Relic parentRelic;

    public void Apply(PlayerController player, Relic source)
    {
        evaluatedAmount = Mathf.RoundToInt(RPNEvaluator.Evaluate(AmountExpr, player.GetRPNVariables()));
        parentRelic = source;

        switch (Type)
        {
            case "gain-mana":
                player.spellcaster.mana += evaluatedAmount;
                break;
            case "gain-spellpower":
                player.spellcaster.power += evaluatedAmount;
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
                void OnMoveHandler(Vector2 _)
                {
                    Revert(player);
                    player.OnMoveEvent -= OnMoveHandler;
                    parentRelic.ResetTrigger();
                }
                player.OnMoveEvent += OnMoveHandler;
                break;

            case "cast-spell":
                void OnCastHandler(Spell _)
                {
                    Revert(player);
                    player.spellcaster.OnSpellCast -= OnCastHandler;
                    parentRelic.ResetTrigger();
                }
                player.spellcaster.OnSpellCast += OnCastHandler;
                break;
        }
    }

    public void Revert(PlayerController player)
    {
        switch (Type)
        {
            case "gain-spellpower":
                player.spellcaster.power -= evaluatedAmount;
                break;
        }
    }
}
