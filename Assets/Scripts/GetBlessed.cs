using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetBlessed : MonoBehaviour
{
    public int accessToTrials;
    public bool PoseidonPassed;
    public bool DaedalusPassed;
    // Start is called before the first frame update
    void Awake()
    {
        accessToTrials = 0;
        PoseidonPassed = false;
        DaedalusPassed = false;
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
        // [TO DO] Grab the reference of this boolean value in the code for Rhino AI, player, piles, or whatever required for - when the value is set true - making the rhino blind
        // to spot the player (could be represented by transparency) for the coming 15 sedonds after the player approaches any pile of rocks (whether intact or not) in the main arena.
    }

    public void UnveilHealthFortune()
    {
        float odd = Random.Range(0f, 1f);
        if (odd <= 0.5f)
        {
            GetHealth gh = GetComponent<GetHealth>();
            gh.mhp = 125f;
            gh.defaultAdd = 7.5f;
        }
        else
        {
            GetHealth gh = GetComponent<GetHealth>();
            gh.mhp = 75f;
            gh.defaultAdd = 12.5f;
        }
    }

    public void UnveilMeleeFortune()
    {
        float odd = Random.Range(0f, 1f);
        if (odd <= 0.5f)
        {
            GameObject bc = GameObject.Find("BatCollider");
            Weapon wp = bc.GetComponent<Weapon>();
            wp.Damage = 15f;
            Animator anim = this.gameObject.GetComponent<Animator>();
            anim.SetFloat("animSpeed", 0.8f);
        }
        else
        {

            GameObject bc = GameObject.Find("BatCollider");
            Weapon wp = bc.GetComponent<Weapon>();
            wp.Damage = 7.5f;
            Animator anim = this.gameObject.GetComponent<Animator>();
            anim.SetFloat("animSpeed", 1.6f);
        }
    }

    public void UnveilFireFortune()
    {
        float odd = Random.Range(0f, 1f);
        if (odd <= 0.5f)
        {
            RockThrower rt = this.gameObject.GetComponent<RockThrower>();
            rt.launchVelocity.z = rt.launchVelocity.z / 2;
            rt.rockPrefab.GetComponent<Weapon>().Damage = 10f;
        }
        else
        {
            RockThrower rt = this.gameObject.GetComponent<RockThrower>();
            rt.rockPrefab.GetComponent<Weapon>().Damage = 4f;
            rt.launchVelocity.z = rt.launchVelocity.z * 2;
        }
    }
}