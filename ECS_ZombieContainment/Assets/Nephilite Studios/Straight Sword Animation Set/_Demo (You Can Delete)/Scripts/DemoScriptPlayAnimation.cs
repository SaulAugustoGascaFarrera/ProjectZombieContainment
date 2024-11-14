using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoScriptPlayAnimation : MonoBehaviour
{
    private Animator animator;
    private Vector3 startingPosition;
    private Quaternion startingRotation;

    [Header("Character")]
    [SerializeField] GameObject offHandWeapon;
    [SerializeField] GameObject criticalStrikeDummy;
    private Vector3 criticalStrikeDummyPosition;

    [Header("Current Animation")]
    public bool isPerformingAnimation = false;
    [SerializeField] string currentAnimation;

    [Header("Animations")]
    [SerializeField] List<string> animations = new List<string>();
    [SerializeField] Text currentAnimationName;
    private int animationListIndex = -1;

    [Header("Autoplay")]
    private bool autoPlay = false;
    [SerializeField] Text autoPlayText;

    [Header("Camera")]
    [SerializeField] Transform[] cameraTransforms;
    private int cameraPositionIndex = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        startingPosition = transform.position;
        startingRotation = transform.rotation;
        criticalStrikeDummyPosition = criticalStrikeDummy.transform.position;

        foreach (AnimationClip ac in animator.runtimeAnimatorController.animationClips)
        {
            animations.Add(ac.name);
        }
    }

    private void Start()
    {
        PlayNextAnimation();
        ToggleAutoPlay();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PlayPeviousAimation();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            PlayNextAnimation();
        }

        if (autoPlay && !IsAnimationPlaying())
        {
            PlayNextAnimation();
        }
    }

    public void PlayNextAnimation()
    {
        criticalStrikeDummy.SetActive(false);

        transform.rotation = startingRotation;
        transform.position = startingPosition;

        animationListIndex++;

        if (animationListIndex >= animations.Count)
        {
            animationListIndex = 0;
        }

        isPerformingAnimation = true;
        animator.Play(animations[animationListIndex]);
        currentAnimationName.text = animations[animationListIndex];

        if (animations[animationListIndex].Contains("dw") || animations[animationListIndex].Contains("off"))
        {
            offHandWeapon.SetActive(true);
        }
        else
        {
            offHandWeapon.SetActive(false);
        }
    }

    public void PlayPeviousAimation()
    {
        criticalStrikeDummy.SetActive(false);

        transform.rotation = startingRotation;
        transform.position = startingPosition;

        animationListIndex--;

        if (animationListIndex < 0)
        {
            animationListIndex = animations.Count -1;
        }

        isPerformingAnimation = true;
        animator.Play(animations[animationListIndex]);
        currentAnimationName.text = animations[animationListIndex];
    }

    public void ToggleAutoPlay()
    {
        autoPlay = !autoPlay;

        if (autoPlay)
        {
            autoPlayText.text = "Auto Play: ON"; 
        }
        else
        {
            autoPlayText.text = "Auto Play: OFF";
        }
    }

    private bool IsAnimationPlaying()
    {
        return animator.GetCurrentAnimatorStateInfo(0).length >
       animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    public void ToggleCameraPosition()
    {
        cameraPositionIndex += 1;

        if (cameraPositionIndex >= cameraTransforms.Length)
            cameraPositionIndex = 0;

        Camera.main.transform.position = cameraTransforms[cameraPositionIndex].position;
        Camera.main.transform.rotation = cameraTransforms[cameraPositionIndex].rotation;
    }

    public void PlayBackstab()
    {
        transform.rotation = startingRotation;
        transform.position = startingPosition;
        animator.Play("core_main_backstab_01");
        currentAnimationName.text = "core_main_backstab_01";

        criticalStrikeDummy.SetActive(true);
        criticalStrikeDummy.transform.position = criticalStrikeDummyPosition;
        criticalStrikeDummy.transform.forward = gameObject.transform.forward;
        criticalStrikeDummy.GetComponent<Animator>().Play("core_main_backstab_victim_01");
    }

    public void PlayRiposte()
    {
        transform.rotation = startingRotation;
        transform.position = startingPosition;
        animator.Play("core_main_riposte_01");
        currentAnimationName.text = "core_main_riposte_01";

        criticalStrikeDummy.SetActive(true);
        criticalStrikeDummy.transform.position = criticalStrikeDummyPosition;
        criticalStrikeDummy.transform.forward = -gameObject.transform.forward;
        criticalStrikeDummy.GetComponent<Animator>().Play("core_main_riposte_victim_01");
    }
}
