using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DynamicMeshCutter;

public class BRSkinController : MonoBehaviour
{
    [Header("Main Variables")]
    public FightStyle activeSkin;

    public List<GameObject> skins = new List<GameObject>();
    public List<FightStyle> weapons = new List<FightStyle>();
    public List<FightStyle> allSkins = new List<FightStyle>();
    GameObject weapon;
    public BRCharacterCombat myCombatController;

    public List<FightStyle> slicingWeapons = new List<FightStyle>();
    public bool slicingWeapon = false;
    public PlaneBehaviour planeBehaviour;

    public float scale = 0.4f;

    public Transform handPoint;

    bool skinSet = false;

    public void OnEnable()
    {
        if (!skinSet)
        {
            RandomizeSkin();

            for(int i =0; i< slicingWeapons.Count; i++)
            {
                if (slicingWeapons[i] == activeSkin)
                {
                    slicingWeapon = true;
                    myCombatController.slicingWeapon = true;
                }
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////         RANDOMISED SKINS         /////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Call to randomise select body parts
    void RandomiseBodyPart(List<GameObject> partList)
    {
        int Rand = Random.Range(0, partList.Count);
        for (int i = 0; i < partList.Count; i++)
        {
            if (Rand == i)
            {
                partList[Rand].SetActive(true);
                for (int j = 0; j < allSkins.Count; j++)
                {
                    if (partList[Rand].name == allSkins[j].ToString())
                    {
                        activeSkin = allSkins[j];
                    }
                }

            }
            else
            {
                partList[i].SetActive(false);
            }
        }
        SetWeaponOnPart(activeSkin, weapons);
    }

    // Randomise skins
    public void RandomizeSkin()
    {
        skinSet = true;
        RandomiseBodyPart(skins);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////         NONRANDOMISED SKINS         ////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Call to set specific skin part
    public void SetSkinOnBodyPart(FightStyle skin, List<GameObject> partList)
    {
        for (int i = 0; i < partList.Count; i++)
        {
            switch (partList[i].name.Contains(skin.ToString()))
            {
                case true:
                    partList[i].SetActive(true);
                    break;
                case false:
                    partList[i].SetActive(false);
                    break;
            }
        }
    }

    // Call to set specific skin
    public void SetSkin(FightStyle skin)
    {
        SetSkinOnBodyPart(skin, skins);
        activeSkin = skin;

        SetWeaponOnPart(skin, weapons);
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////         WEAPONS         //////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Call to set specific weapon part
    public void SetWeaponOnPart(FightStyle skin, List<FightStyle> partList)
    {
        for (int i = 0; i < partList.Count; i++)
        {
            switch (partList[i].ToString().Contains(skin.ToString()))
            {
                case true:
                    GameObject _weapon;
                    _weapon = (GameObject)Resources.Load("WeaponsEnemy/" + partList[i].ToString(), typeof(GameObject));

                    switch (weapon == null)
                    {
                        case true:
                            weapon = Instantiate(_weapon, handPoint);
                            break;
                        case false:
                            Destroy(weapon.gameObject);
                            weapon = null;
                            weapon = Instantiate(_weapon, handPoint);
                            break;
                    }
                    ResizeWeapon();
                    break;
                case false:
                    //partList[i].SetActive(false);
                    break;
            }
        }
    }

    void ResizeWeapon()
    {
        if (weapon != null)
        {
            weapon.transform.localScale = new Vector3(scale, scale, scale);
            weapon.transform.localEulerAngles = new Vector3(42.704f, 10.784f, 78.196f);
            weapon.transform.localPosition = new Vector3(0.515f, -0.14f, -0.152f);
        }
    }

}
