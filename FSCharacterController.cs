using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DynamicMeshCutter;

public enum FightStyle
{
    hands,
    StoneAgeV1, StoneAgeV2, StoneAgeV3, StoneAgeV4, StoneAgeV5,
    MetalAgeV1, MetalAgeV2, MetalAgeV3, MetalAgeV4, MetalAgeV5, MetalAgeV6,
    NinjaAgeV1, NinjaAgeV2, NinjaAgeV3, NinjaAgeV4,
    VikingAgeV1, VikingAgeV2, VikingAgeV3, VikingAgeV4 
}

public class FSCharacterController : MonoBehaviour
{
    [Header("Main Variables")]
    public FightSequenceController fightSequenceController;
    public FightStyle activeStyle;
    public FSSkinController skinController;
    public Animator myAnim;
    public string defaultIdleAnimation = "", equipAnimation = "", winAnimation = "", loseAnimation = "", defeatedAnim = "", laughAnim = "";
    public Transform handPosition, fakeHandPos;
    public FSFXController fxController;
    public FSRagdollController ragdollController;
    public IKControl myIk;
    public AudioSource mySource;
    public bool isAttacking = false, isHit = false;

    float transitionTime = 0.1f, lookSpeed =1;

    [Header("Health Variables")]
    public float health;
    public float maxHealth = 100, baseDamage = 10, damage;
    public bool hasWeapon;
    //public bool dieOnOneHit = false;

    [Header("Weapon Variables")]
    public FSWeapon Weapon;
    public Transform weaponJumpTo;
    public float newWeaponScale =.25f;

    [Header(" Control Variables")]
    public float arcHeight = 1f;
    public float  equipSpeed = 5f;
    public bool smoothen = true, isFinalHit = false;
    public PlaneBehaviour planeBehaviour;
    public DynamicRagdoll myRagdoll;
    float reactDelay = 1f;
    bool alreadyCalled = false;

    [Header(" Reversal Variables")]
    public bool startReversalCountdown = false;
    public float reversalCountdownTimer = 3f, timerReductionRate = 1;
    float originalTimerValue;
    Vector3 originalRot;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////         INITIALIZE VARIABLES         ///////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region Initialise Variables
    // Start is called before the first frame update
    void Start()
    {
        //ResetHealth();
        ResetDamage();
        originalTimerValue = reversalCountdownTimer;
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////         ATTACK VARIABLES         /////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region Attack Variables
    //private void Update()
    //{
    //    if (startReversalCountdown)
    //    {
    //        reversalCountdownTimer -= (timerReductionRate * Time.deltaTime);
    //        if(reversalCountdownTimer < 0)
    //        {
    //            startReversalCountdown = false;
    //            reversalCountdownTimer = originalTimerValue;
    //            fightSequenceController.Reversal();
    //        }
    //    }
    //}
    
    public void ResetDamage()
    {
        damage = baseDamage;
    }

    public void SetWeaponDamage(FSWeapon weapon)
    {
        hasWeapon = true;
        damage = weapon.WeaponDamage;
    }

    public float GetDamage()
    {
        if (hasWeapon)
        {
            return Weapon.WeaponDamage;
        }
        else
        {
            return damage;
        }
    }

    [ContextMenu("Swipe Attack")]
    public void SwipeAttack()
    {
        //fightSequenceController.FightSequenceUi.ToggleInstructions(false);
        //fightSequenceController.NormalSpeed();
        //fightSequenceController.StopEnemyReversalCountdown();
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////         SEQUENCE VARIABLES         ////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region Sequence Variables
    public void EndTurn()
    {
        // used to swap turns in sequence controller
        fightSequenceController.EndTurn();// might need checking
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////         ANIMATION VARIABLES         ///////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region Animation Variables
    public void PlayWinAnim()
    {
        // used to swap turns in sequence controller
        myAnim.CrossFadeInFixedTime(winAnimation, transitionTime);
    }

    public void PlayDefeatedAnim()
    {
        // used to swap turns in sequence controller
        myAnim.CrossFadeInFixedTime(defeatedAnim, transitionTime);
    }
    
    public void PlayLaughingAnim()
    {
        // used to swap turns in sequence controller
        myAnim.CrossFadeInFixedTime(laughAnim, transitionTime);
        // Play laugh sound
        CommandCentre.Instance.audioLib_.PlayGameplaySound(GamePlaySounds.EnemyLaugh);
    }

    public void PlayDeathAnim()
    {
        // used to swap turns in sequence controller
        myAnim.CrossFadeInFixedTime(loseAnimation, transitionTime);
    }
    
    public void PlayDeathAnim(bool usingAnim, bool ragdoll, BodyParts partHit)
    {
        switch (usingAnim)
        {
            case true:
                // used to swap turns in sequence controller
                myAnim.CrossFadeInFixedTime(loseAnimation, transitionTime);
                break;
            case false:
                switch (ragdoll)
                {
                    case true:
                        ragdollController.RagdollWithForce(partHit);
                        break;
                    case false:
                        ragdollController.enabled = false;
                        CommandCentre.Instance.audioLib_.PlayGameplaySound(GamePlaySounds.Slice);
                        fightSequenceController.player.planeBehaviour.transform.parent = fightSequenceController.theLevel.transform;
                        fightSequenceController.player.planeBehaviour.CutSpecific(myRagdoll.transform);
                        myAnim.CrossFadeInFixedTime(loseAnimation, transitionTime);
                        break;
                }
                break;
        }
    }

    public void PlayDeathAnim(string deathAnimation)
    {
        // used to swap turns in sequence controller
        for(int i = 0; i < fightSequenceController.AnimLib_.deathAnimations.Count; i++)
        {
            if(fightSequenceController.AnimLib_.deathAnimations[i] == deathAnimation)
            {
                myAnim.CrossFadeInFixedTime(deathAnimation, transitionTime);
            }
            else
            {
                myAnim.CrossFadeInFixedTime(fightSequenceController.AnimLib_.defaultDeathAnimation, transitionTime);
            }
        }
    }

    public void EquipAnimation()
    {
        // Play equip sound
        //CommandCentre.Instance.audioLib_.PlayGameplaySound(GamePlaySounds.WeaponEquip);

        fakeHandPos.position = handPosition.position;
        fakeHandPos.rotation = handPosition.rotation;
        fakeHandPos.transform.parent = handPosition;

        //myAnim.CrossFadeInFixedTime(equipAnimation, transitionTime);
        
        //ResetHealth();
    }

    public void OnDeathEventFunction()
    {
        fightSequenceController.StartDancing();
    }

    public void ReturnToIdle()
    {
        myAnim.Play(defaultIdleAnimation);
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////         WEAPON VARIABLES         /////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region Weapon Variables

    public void RescaleWeaponSize()
    {
        fakeHandPos.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }

    public void SetWeapon(FSWeapon weapon)
    {
        // player looks at camera
        var cameraDir = fightSequenceController.MainCamera.transform.position;
        cameraDir.y = 0;
        //fightSequenceController.player.transform.DOLookAt(cameraDir, lookSpeed);

        Weapon = weapon;
        CommandCentre.Instance.FightSequenceController_.CombinedBrain.PlayerBrain.weapon = GetComponentInParent<TheLevel>().SpawnedItem.GetComponent<FSWeapon>();
        activeStyle = Weapon.fightStyle;
        CommandCentre.Instance.FightSequenceController_.CombinedBrain.PlayerBrain.myStyle = Weapon.fightStyle;
        SetWeaponDamage(Weapon);

        // Control
        fakeHandPos.position = weapon.transform.position;

        // IKs
        myIk.LH = weapon.LH;

        // Weapon size rescaled before moving
        fightSequenceController.theLevel.SpawnedItem.GetComponent<Weld_Obj>().SetStandLoc(fakeHandPos);
        RescaleWeaponSize();
        fakeHandPos.parent = null;
        fakeHandPos.transform.eulerAngles =  new Vector3(-90,90,0);
        Weapon.transform.localScale = new Vector3(1f, 1f, 1f);
        EquipAnimation();
        OnCompleteWeaponEquip();
        planeBehaviour = weapon.GetComponentInChildren<PlaneBehaviour>();
        // Stop FS Camera
        fightSequenceController.fightCameraController.CanFollowCamPos = false;
        fightSequenceController.fightCameraController.MoveCameraAndDoAction(fightSequenceController.fightCameraController.weaponEquipCamPosition, DelayBeforeMove);
    }

    void DelayBeforeMove()
    {
        // stop current animations
        //fightSequenceController.player.ReturnToIdle();
        //fightSequenceController.enemy.ReturnToIdle();
        //EquipAnimation();
        //Invoke(nameof(MoveWeaponToHand), 0.1f);
    }

    void MoveWeaponToHand()
    {
        // Move weapon to hand
        CommandCentre.Instance.MainMenuController_.TriggerFS_StartUI_text();
        fakeHandPos.transform.DOJump(weaponJumpTo.position, arcHeight, 1, equipSpeed, smoothen).OnComplete(EquipAnimation);
        //fakeHandPos.transform.DOMove(weaponJumpTo.position, equipSpeed).OnComplete(EquipAnimation);
    }


    public void OnCompleteWeaponEquip()
    {
        //Debug.Log(GetCurrentClipName());
        fightSequenceController.ReadyToStart();
    }

    //public string GetCurrentClipName()
    //{
    //    int layerIndex = 0;
    //    AnimatorClipInfo[] clipInfo;
    //    clipInfo = myAnim.GetCurrentAnimatorClipInfo(layerIndex);
    //    return clipInfo[0].clip.name;
    //}

    public void ReduceWeaponDurability()
    {
        Weapon.ReduceHealth();
    }

    public bool CheckIfWeaponIsBroken()
    {
        if (Weapon!= null)
        {
            if (Weapon.Health <= 0)
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////         HEALTH VARIABLES         /////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////// <Description> Anything to do with the character taking damage </Description> ////////////////////

    #region Health Variables
    public void ResetHealth()
    {
        switch (gameObject.name == "Enemy" && PlayerPrefs.HasKey("EnemyLife"))
        {


            case true:
                //switch(dieOnOneHit)
                //{
                //    case true:
                //        health = 1;
                //        break;
                //    case false:
                //        health = PlayerPrefs.GetFloat("EnemyLife");
                //        break; 
                //}
                health = PlayerPrefs.GetFloat("EnemyLife");
                break;
            case false:
                //switch (dieOnOneHit)
                //{
                //    case true:
                //        health = 1;
                //        break;
                //    case false:
                //        health = maxHealth;
                //        break;
                //}
                health = maxHealth;
                break;
        }
        
    }

    public void ResetMaxHealth()
    {
        maxHealth = CommandCentre.Instance.EnemyHealth_.GetMaxHealth(this);
    }

    public void TakeDamage(float value)
    {
        health -= value;
        //if (dieOnOneHit) { health = 0; }

        if (health <= 0)
        {
            health = 0;
            EndFight();
        }
    }

    public void Hit(FightStyle style, string response, FightFX fX)
    {
        //Use Default Hit Animation
        myAnim.CrossFadeInFixedTime(response, transitionTime);
        fxController.PlaySpecificFX(fX);
    }

    // Needs to be on character as is called by the Event Handler system to send events from each character to the Sequence Manager, can be better optimised.
    public void HitCharacter(Attack storedAttack, bool playerIsTheOneWhoAttacked)
    {
        // Check who's hit and call hit fn on the non active chatacter
        switch (playerIsTheOneWhoAttacked)
        {
            case true:

                fightSequenceController.ChangeTurn();
                fightSequenceController.enemy.Hit(storedAttack.style, storedAttack.attackResponse, storedAttack.fightFX);
                if (!fightSequenceController.isAutoFighting && fightSequenceController.IsFightStarted)
                {

                    fightSequenceController.enemy.isHit = true;
                    fightSequenceController.player.isAttacking = false;

                    fightSequenceController.enemy.TakeDamage(damage);
                    fightSequenceController.EnemyHasBeenHit(damage);
                    // Destroy Random Building
                    fightSequenceController.environmentController.DestroyRandomBuilding();
                    // Camera Shake
                    fightSequenceController.fightCameraController.fightCameraShake.enabled = true;
                    fightSequenceController.fightCameraController.fightCameraShake.ShakeNow(.5f);

                    // For now play random fx
                    fightSequenceController.enemy.fxController.PlayRandomFX();

                    // Weapon Control values
                    if (fightSequenceController.player.hasWeapon) {
                        ReduceWeaponDurability();
                        if (fightSequenceController.player.CheckIfWeaponIsBroken() && fightSequenceController.enemy.health > 0)
                        {
                            WeaponBroken();
                        }
                    }

                    // Play hurt sounds
                    int i = Random.Range(0, 3);
                    switch (i)
                    {
                        case 0:
                            CommandCentre.Instance.audioLib_.PlayGameplaySound(GamePlaySounds.Hurt1);
                            break;
                        case 1:
                            CommandCentre.Instance.audioLib_.PlayGameplaySound(GamePlaySounds.Hurt2);
                            break;
                        case 2:
                            CommandCentre.Instance.audioLib_.PlayGameplaySound(GamePlaySounds.Hurt3);
                            break;
                        case 3:
                            CommandCentre.Instance.audioLib_.PlayGameplaySound(GamePlaySounds.Hurt4);
                            break;
                    }


                    // play Hit sounds
                    switch (storedAttack.style)
                    {
                        case FightStyle.hands:
                            int j = Random.Range(0, 3);
                            switch (j)
                            {
                                case 0:
                                    CommandCentre.Instance.audioLib_.PlayGameplaySoundAtSource(GamePlaySounds.Punch1, mySource);
                                    break;
                                case 1:
                                    CommandCentre.Instance.audioLib_.PlayGameplaySoundAtSource(GamePlaySounds.Punch2, mySource);
                                    break;
                                case 2:
                                    CommandCentre.Instance.audioLib_.PlayGameplaySoundAtSource(GamePlaySounds.BackSlap, mySource);
                                    break;
                                case 3:
                                    CommandCentre.Instance.audioLib_.PlayGameplaySoundAtSource(GamePlaySounds.Kick, mySource);
                                    break;
                            }
                            break;
                        case FightStyle.StoneAgeV1:
                        case FightStyle.StoneAgeV2:
                        case FightStyle.MetalAgeV3:
                        case FightStyle.MetalAgeV4:
                        case FightStyle.MetalAgeV5:
                        case FightStyle.NinjaAgeV1:
                        case FightStyle.NinjaAgeV2:
                        case FightStyle.NinjaAgeV3:
                        case FightStyle.NinjaAgeV4:
                        case FightStyle.VikingAgeV1:
                        case FightStyle.VikingAgeV2:
                            CommandCentre.Instance.audioLib_.PlayGameplaySound(GamePlaySounds.Thud);
                            break;
                        case FightStyle.StoneAgeV3:
                        case FightStyle.StoneAgeV4:
                        case FightStyle.MetalAgeV1:
                        case FightStyle.MetalAgeV2:
                        case FightStyle.VikingAgeV3:
                        case FightStyle.VikingAgeV4:
                            CommandCentre.Instance.audioLib_.PlayGameplaySound(GamePlaySounds.Thud);
                            break;
                    }
                }

                break;
            case false:

                fightSequenceController.ChangeTurn();
                fightSequenceController.player.Hit(storedAttack.style, storedAttack.attackResponse, storedAttack.fightFX);
                if (!fightSequenceController.isAutoFighting && fightSequenceController.IsFightStarted) {


                    fightSequenceController.player.isHit = true;
                    fightSequenceController.enemy.isAttacking = false;

                    // if player is hit when attacking do sth


                    fightSequenceController.player.TakeDamage(damage);
                    fightSequenceController.PlayerHasBeenHit(damage);

                    // Feedback on hit
                    CommandCentre.Instance.FlashRedBars.Flash();
                    fightSequenceController.fightCameraController.fightCameraShake.ShakeNow(0.5f);

                    // For now play random fx
                    fightSequenceController.player.fxController.PlayRandomFX();


                    // Play hurt sounds
                    int i = Random.Range(0, 3);
                    switch (i)
                    {
                        case 0:
                            CommandCentre.Instance.audioLib_.PlayGameplaySound(GamePlaySounds.Hurt1);
                            break;
                        case 1:
                            CommandCentre.Instance.audioLib_.PlayGameplaySound(GamePlaySounds.Hurt2);
                            break;
                        case 2:
                            CommandCentre.Instance.audioLib_.PlayGameplaySound(GamePlaySounds.Hurt3);
                            break;
                        case 3:
                            CommandCentre.Instance.audioLib_.PlayGameplaySound(GamePlaySounds.Hurt4);
                            break;
                    }

                    // play Hit sounds
                    switch (storedAttack.style)
                    {
                        case FightStyle.hands:
                            int j = Random.Range(0, 3);
                            switch (j)
                            {
                                case 0:
                                    CommandCentre.Instance.audioLib_.PlayGameplaySoundAtSource(GamePlaySounds.Punch1, mySource);
                                    break;
                                case 1:
                                    CommandCentre.Instance.audioLib_.PlayGameplaySoundAtSource(GamePlaySounds.Punch2, mySource);
                                    break;
                                case 2:
                                    CommandCentre.Instance.audioLib_.PlayGameplaySoundAtSource(GamePlaySounds.BackSlap, mySource);
                                    break;
                                case 3:
                                    CommandCentre.Instance.audioLib_.PlayGameplaySoundAtSource(GamePlaySounds.Kick, mySource);
                                    break;
                            }
                            break;
                        case FightStyle.StoneAgeV1:
                        case FightStyle.StoneAgeV2:
                        case FightStyle.MetalAgeV3:
                        case FightStyle.MetalAgeV4:
                        case FightStyle.MetalAgeV5:
                        case FightStyle.NinjaAgeV1:
                        case FightStyle.NinjaAgeV2:
                        case FightStyle.NinjaAgeV3:
                        case FightStyle.NinjaAgeV4:
                        case FightStyle.VikingAgeV1:
                        case FightStyle.VikingAgeV2:
                            CommandCentre.Instance.audioLib_.PlayGameplaySound(GamePlaySounds.Thud);
                            break;
                        case FightStyle.StoneAgeV3:
                        case FightStyle.StoneAgeV4:
                        case FightStyle.MetalAgeV1:
                        case FightStyle.MetalAgeV2:
                        case FightStyle.VikingAgeV3:
                        case FightStyle.VikingAgeV4:
                            CommandCentre.Instance.audioLib_.PlayGameplaySound(GamePlaySounds.Thud);
                            break;
                    }
                } ;
               
                break;
        }
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////         END SEQUENCE VARIABLES         //////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region End Sequence Variables
    void EndFight()
    {
        fightSequenceController.EndFight();
    }

    public void WeaponBroken()
    {
        if (alreadyCalled) { return; }

        alreadyCalled = true;
        // stop fight on tap controller
        //fightSequenceController.tapController.StopFight();

        //KO Attacks
        CommandCentre.Instance.FightSequenceController_.CombinedBrain.DeactivateCharacters();

        // Play weapon break sound
        CommandCentre.Instance.audioLib_.PlayGameplaySound(GamePlaySounds.WeaponBreak);

        //PlayerPrefs.SetFloat("EnemyLife", fightSequenceController.enemy.health);
        if (fightSequenceController.enemy.startReversalCountdown)
        {
            fightSequenceController.enemy.startReversalCountdown = false;
        }
        //fightSequenceController.NormalSpeed();
        fightSequenceController.BrokenWeapon();
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////         CONTROL VARIABLES         ////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region Control Variables
    public void ToggleFinalHit(bool toggle)
    {
        isFinalHit = toggle;
    }


    public void ResetReversalCounter()
    {
        reversalCountdownTimer = originalTimerValue;
    }
    #endregion
}

// Used on player side to set attacks, slow mo time for each attack and camera angle of each attack
[System.Serializable]
public class Attack
{
    // Death anim can be null for default anim to play
    public string attack, attackResponse;
    public FightStyle style;
    public float slowMoTime;
    public Transform cameraAngle;
    public FightFX fightFX;
    public BodyParts partHit;
    public bool hasRagdoll = false, usingAnimation = false;
    public string deathAnimation;
}

[System.Serializable]
public class AudioSourceHolder
{
    public AudioSource mySource;
}
