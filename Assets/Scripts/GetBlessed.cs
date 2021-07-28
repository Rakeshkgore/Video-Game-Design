using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetBlessed : MonoBehaviour
{
    public int accessToTrials;
    public bool PoseidonPassed;
    public bool DaedalusPassed;
    public bool TycheFortuneCollected;
    // Start is called before the first frame update
    void Awake()
    {
        accessToTrials = 0;
        PoseidonPassed = false;
        DaedalusPassed = false;
        TycheFortuneCollected = false;
    }

    public void GainAccess()
    {
        accessToTrials += 1;
    }

    public void WaterFree()
    {
        PoseidonPassed = true;
    }

    public void Invisible()
    {
        DaedalusPassed = true;
    }

    public void UnveilHealthFortune()
    {
        if (TycheFortuneCollected)
        {
            return;
        }
        TycheFortuneCollected = true;

        float odd = Random.Range(0f, 1f);
        if (odd <= 0.5f)
        {
            GetHealth gh = GetComponent<GetHealth>();
            gh.mhp = 125f;
            gh.defaultAdd = 7.5f;
            ContextualText.ShowFor("Your fortune has been revealed:\n+25% max health, -25% health restored from food", 5f);
        }
        else
        {
            GetHealth gh = GetComponent<GetHealth>();
            gh.mhp = 75f;
            gh.defaultAdd = 15f;
            ContextualText.ShowFor("Your fortune has been revealed:\n-25% max health, +50% health restored from food", 5f);
        }
    }

    public void UnveilMeleeFortune()
    {
        if (TycheFortuneCollected)
        {
            return;
        }
        TycheFortuneCollected = true;

        float odd = Random.Range(0f, 1f);
        if (odd <= 0.5f)
        {
            GameObject bc = GameObject.Find("BatCollider");
            Weapon wp = bc.GetComponent<Weapon>();
            wp.Damage = 15f;
            RootMotionControlScript rmcs = GetComponent<RootMotionControlScript>();
            rmcs.animationSpeed *= 0.6f;
            ContextualText.ShowFor("Your fortune has been revealed:\n+50% melee damage, -40% attack speed", 5f);
        }
        else
        {
            GameObject bc = GameObject.Find("BatCollider");
            Weapon wp = bc.GetComponent<Weapon>();
            wp.Damage = 7.5f;
            RootMotionControlScript rmcs = GetComponent<RootMotionControlScript>();
            rmcs.animationSpeed *= 1.25f;
            ContextualText.ShowFor("Your fortune has been revealed:\n-25% melee damage, +25% attack speed", 5f);
        }
    }

    public void UnveilFireFortune()
    {
        if (TycheFortuneCollected)
        {
            return;
        }
        TycheFortuneCollected = true;

        float odd = Random.Range(0f, 1f);
        if (odd <= 0.5f)
        {
            RockThrower rt = this.gameObject.GetComponent<RockThrower>();
            rt.launchVelocity.z = rt.launchVelocity.z / 2;
            rt.rockPrefab.GetComponent<Weapon>().Damage = 10f;
            ContextualText.ShowFor("Your fortune has been revealed: +100% throw damage, -50% throw range", 5f);
        }
        else
        {
            RockThrower rt = this.gameObject.GetComponent<RockThrower>();
            rt.rockPrefab.GetComponent<Weapon>().Damage = 4f;
            rt.launchVelocity.z = rt.launchVelocity.z * 2;
            ContextualText.ShowFor("Your fortune has been revealed: -20% throw damage, +100% throw range", 5f);
        }
    }
}