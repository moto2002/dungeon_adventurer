using UnityEngine;

public class CheatController : MonoBehaviour {

    [SerializeField] bool active = false;

    [SerializeField] int goldIncrease = 1;
    [SerializeField] int silverIncrease = 1;
    [SerializeField] int copperIncrease = 1;
    [SerializeField] int diamondsIncrease = 1;

    void Update() {
        if (!active) return;

        if (Input.GetKeyDown(KeyCode.G)) {
            if (goldIncrease >= 0)
                ServiceRegistry.Currency.CreditCurrency(Currency.Coins, goldIncrease*10000);
            else
                ServiceRegistry.Currency.TryPurchase(Currency.Coins, -goldIncrease*10000);
        }

        if (Input.GetKeyDown(KeyCode.S)) {

            if (silverIncrease >= 0)
                ServiceRegistry.Currency.CreditCurrency(Currency.Coins, silverIncrease*100);
            else
                ServiceRegistry.Currency.TryPurchase(Currency.Coins, -silverIncrease*100);
        }

        if (Input.GetKeyDown(KeyCode.C)) {

            if (copperIncrease >= 0)
                ServiceRegistry.Currency.CreditCurrency(Currency.Coins, copperIncrease);
            else
                ServiceRegistry.Currency.TryPurchase(Currency.Coins, -copperIncrease);
        }

        if (Input.GetKeyDown(KeyCode.D)) {

            if (diamondsIncrease >= 0)
                ServiceRegistry.Currency.CreditCurrency(Currency.Diamonds, diamondsIncrease);
            else
                ServiceRegistry.Currency.TryPurchase(Currency.Diamonds, -diamondsIncrease);
        }


    }
}
