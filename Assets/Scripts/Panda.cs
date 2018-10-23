using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using UnityEngine.Assertions;

[RequireComponent(typeof(Animator))]
public class Panda : MonoBehaviour, IInputHandler
{
    [Tooltip("The GameObject that the Panda will eat")]
    [SerializeField] private GameObject _bamboo;
    [Tooltip("The Panda Paw to hold the food/bamboo")]
    [SerializeField] private Transform _pawToHoldBamboo;  // 'PANDA/new_Bone009' in the model is the left paw
    [Tooltip("Angular speed in radians per sec.")]
    [SerializeField] private float angularSpeed = 1f;
    [SerializeField] private AudioSource _audioSource;
    
    [SerializeField] private AudioClip _bearBreath;
    [SerializeField] private AudioClip _bearSounds;
    [SerializeField] private AudioClip _chewing;
    
    private Animator _animator;
    // Animator Hashes, for efficiency
    private int _isEatingParameterHash = Animator.StringToHash("IsEating");
    private int _isLayingDownParameterHash = Animator.StringToHash("IsLayingDown");
    private int _isIdlingParameterHas = Animator.StringToHash("IsIdling");

    private bool _isEating = false;
    private bool _isLayingDown = false;
    private bool _isIdling = false;
    
    void Awake()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        Assert.IsNotNull(_animator);
        Assert.IsNotNull(_bamboo);
        Assert.IsNotNull(_pawToHoldBamboo);
        Assert.IsNotNull(_audioSource);
        Assert.IsNotNull(_bearBreath);
        Assert.IsNotNull(_bearSounds);
        Assert.IsNotNull(_chewing);
    }

    void Start()
    {
        SetIdle();
    }    

    void Update()
    {
        RotatePandaTowardsCamera();
    }    

    private void RotatePandaTowardsCamera()
    {
        Vector3 targetDir = this.transform.position - HoloToolkit.Unity.CameraCache.Main.transform.position;

        // The step size is equal to speed times frame time.
        float step = angularSpeed * Time.deltaTime;

        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
        //Debug.DrawRay(transform.position, newDir, Color.red);

        // Move our position a step closer to the target.
        transform.rotation = Quaternion.LookRotation(newDir);
    }

    private void ShowBamboo()
    {
        _bamboo.gameObject.SetActive(true);
    }

    private void HideBamboo()
    {
        _bamboo.gameObject.SetActive(false);
    }

    void IInputHandler.OnInputDown(InputEventData eventData)
    {
        //Debug.Log("OnInputDown");                
    }

    void IInputHandler.OnInputUp(InputEventData eventData)
    {
        //Debug.Log("OnInputUp");
        if (_isIdling)
        {
            SetEat();            
            return;
        }

        if (_isLayingDown)
        {
            SetIdle();
            return;
        }
                
        //if (_isEating)
        //{
            SetLayDown();
            return;
        //}
        
    }

    #region Helper Methods
    public void SetEat()
    {
        _isEating = true;
        _isIdling = false;
        _isLayingDown = false;
        SetAnimatorParameters();
        StartCoroutine(ShowAndHideBamboo());
        StartCoroutine(PlayClip(_chewing, 6f));
        // Setting _isEating = false, to stop repeating this state
        _isEating = false;
        StartCoroutine(SetAnimatorParameters(3f));        
    }

    IEnumerator ShowAndHideBamboo()
    {
        _bamboo.gameObject.SetActive(true);
        float animLength = 9.5f;
        yield return new WaitForSeconds(animLength);
        _bamboo.gameObject.SetActive(false);
    }

    public void SetIdle()
    {
        HideBamboo();
        _isEating = false;
        _isIdling = true;
        _isLayingDown = false;
        SetAnimatorParameters();
        StartCoroutine(PlayClip(_bearSounds));
    }

    public void SetLayDown()
    {
        HideBamboo();
        _isEating = false;
        _isIdling = false;
        _isLayingDown = true;
        SetAnimatorParameters();
        StartCoroutine(PlayClip(_bearBreath));
    }

    private void SetAnimatorParameters()
    {
        _animator.SetBool(_isEatingParameterHash, _isEating);
        _animator.SetBool(_isIdlingParameterHas, _isIdling);
        _animator.SetBool(_isLayingDownParameterHash, _isLayingDown);
    }

    IEnumerator SetAnimatorParameters(float delayToSet)
    {
        yield return new WaitForSeconds(delayToSet);
        _animator.SetBool(_isEatingParameterHash, _isEating);
        _animator.SetBool(_isIdlingParameterHas, _isIdling);
        _animator.SetBool(_isLayingDownParameterHash, _isLayingDown);
    }

    IEnumerator PlayClip(AudioClip clip, float delayToStartInSeconds = 0f)
    {
        if (clip == null) yield break;

        _audioSource.Stop();
        yield return new WaitForSeconds(delayToStartInSeconds);
        _audioSource.clip = clip;
        _audioSource.Play();
    }
    #endregion
}
