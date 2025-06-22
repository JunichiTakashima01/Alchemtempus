using System.Collections;
using UnityEngine;

public class PlayerMana : MonoBehaviour
{
    public float maxMana = 1000;
    private float currMana;
    public ManaBarUI manaBarUI;
    public Shield shield;

    public float perfectParryTime = 0.2f;
    private bool isInPerfectParryTimeInterval = false;
    public float perfectParryManaReturnFactor = 1.4f;

    public float ManaUsedForEachDamageBlocked = 50;
    public float ManaUsedForShielding;
    public float whenShieldingManaDrainedEachSecond = 50f;
    public float whenShieldingManaDrainFrequency = 2f; //2 times per s
    private Coroutine whenShieldingManaDrain = null;

    private bool isShielding = false;


    void Awake()
    {
        currMana = maxMana;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    // void Update()
    // {

    // }

    public void UseMana(float mana)
    {
        currMana -= mana;

        if (currMana > maxMana)
        {
            currMana = maxMana;
        }
        else if (currMana <= 0)
        {
            currMana = 0;
            if (isShielding)
            {
                StopShield();
            }
        }

        manaBarUI.SetManaFiller(currMana, maxMana);
    }

    public void StartShield()
    {
        if (currMana > ManaUsedForShielding && shield.StartShield()) //if the shield uses successfully
        {
            UseMana(ManaUsedForShielding);
            this.GetComponent<PlayerHealth>().OnShielding(true);
            StartCoroutine(PerfectParryCountDown(perfectParryTime));
            isShielding = true;
            whenShieldingManaDrain = StartCoroutine(ShieldingManaDrain(whenShieldingManaDrainedEachSecond, whenShieldingManaDrainFrequency));
        }
    }

    public void StopShield()
    {
        this.GetComponent<PlayerHealth>().OnShielding(false);
        shield.StopShield();
        isShielding = false;
        StopCoroutine(whenShieldingManaDrain);
    }

    public void ShieldDmg(float damageBlocked)
    {
        UseMana(damageBlocked * ManaUsedForEachDamageBlocked);
        if (isInPerfectParryTimeInterval)
        {
            UseMana(-perfectParryManaReturnFactor * damageBlocked * ManaUsedForEachDamageBlocked);
        }
    }

    private IEnumerator PerfectParryCountDown(float perfectParryTime)
    {
        isInPerfectParryTimeInterval = true;
        yield return new WaitForSeconds(perfectParryTime);
        isInPerfectParryTimeInterval = false;
    }

    private IEnumerator ShieldingManaDrain(float whenShieldingManaDrainedEachSecond, float whenShieldingManaDrainFrequency)
    {
        while (true)
        {
            yield return new WaitForSeconds(1 / whenShieldingManaDrainFrequency);
            UseMana(whenShieldingManaDrainedEachSecond / whenShieldingManaDrainFrequency);
        }
    }
}
