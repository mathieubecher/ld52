using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class FishingRod : NetworkBehaviour
{
    private ReplicateData m_replicateData;
    
    [SerializeField] private FishingCap m_fishingCap;
    [SerializeField] private Transform m_fishingCapRef;
    [SerializeField] private Transform m_fishingCapFollowPos;
    [SerializeField] private AnimationCurve m_fishinCapHeightOverTime;
    [SerializeField] private AnimationCurve m_fishinCapHeightOverTimeInWater;
    [SerializeField] private AnimationCurve m_fishinCapPositionOverTime;
    [SerializeField] private AnimationCurve m_fishinCastStrengthOverTime;
    [SerializeField] private ShowFishToOther m_showFishToOther;

    private bool m_isFishing = false;
    private bool m_isCasting = false;
    private bool m_isPulling = false;
    private bool m_isInWater = false;
    
    private float m_launchCapsthrength = 0.0f;
    private Vector2 m_launchCapDir = Vector2.up;
    private Vector2 m_fishingCapOriginalPos = Vector2.zero;
    private Vector2 m_fishingCapDesiredOriginalPos = Vector2.zero;
    private float m_launchCapDuration = 0.0f;
    private float m_castDuration = 0.0f;

    private Character m_character;
    
    public bool isFishing { get => m_isFishing; }
    public bool isCasting { get => m_isCasting; }
    public bool isPulling { get => m_isPulling; }

    void Awake()
    {
        m_replicateData = GetComponent<ReplicateData>();
        m_character = GetComponent<Character>();
    }

    void Start()
    {
        m_fishingCapRef.parent.SetParent(null);
    }

    void OnDestroy()
    {
        Destroy(m_fishingCapRef.parent.gameObject);
    }
    private void Update()
    {
        if (!isClient)
            return;

        if (m_isCasting)
        {
            m_castDuration += Time.deltaTime;
        }

        m_fishingCapRef.parent.rotation = Quaternion.identity;
        if (m_isFishing)
        {
            if (m_launchCapDuration < 2.0f)
            {
                m_launchCapDuration += Time.deltaTime;
                Vector2 originalPos = Vector2.Lerp(m_fishingCapOriginalPos,m_fishingCapDesiredOriginalPos, math.min(1.0f, m_launchCapDuration * 2f));
                m_fishingCapRef.parent.position = new Vector2(originalPos.x, m_fishingCapDesiredOriginalPos.y) + m_launchCapDir * (m_launchCapsthrength * m_fishinCapPositionOverTime.Evaluate(m_launchCapDuration));

                Vector3 camPos = transform.position + Vector3.up * 1.5f;
                float dist = (0.5f * m_launchCapsthrength * m_fishinCapPositionOverTime.Evaluate(m_launchCapDuration));
                if (dist <= 4.0f) camPos += (Vector3) m_launchCapDir * dist;
                else camPos += (Vector3) m_launchCapDir * (m_launchCapsthrength * m_fishinCapPositionOverTime.Evaluate(m_launchCapDuration) - 4.0f);
                m_character.cameraTarget.transform.position = camPos;
            
                AnimationCurve heightCurve = m_isInWater ? m_fishinCapHeightOverTimeInWater : m_fishinCapHeightOverTime;
                float height = heightCurve.Evaluate(m_launchCapDuration) + originalPos.y - m_fishingCapDesiredOriginalPos.y;
                //m_fishingCap.setCapColor(height);
            
                m_fishingCapRef.localPosition = Vector3.up * height / math.abs(m_fishingCapRef.parent.lossyScale.y);   
            }
        }
        else
        {
            m_character.cameraTarget.transform.position = transform.position + Vector3.up * 1.5f;
            m_fishingCapRef.parent.position = Vector3.Lerp(m_fishingCapRef.parent.position, m_fishingCapFollowPos.position, 0.2f) ;
        }
    }
    
    public void ReceiveInput(Vector2 _direction, bool _perform = true)
    {
        if (GameManager.instance.isShowingResult)
            return;
        
        if (!isFishing)
        {
            if (_perform)
            {
                CastingFishingRod();
                
                m_castDuration = 0.0f;
                m_isCasting = true;
                m_isPulling = false;
                GetComponent<Character>().animator.SetTrigger("Cast");
            }
            else if(m_isCasting)
            {
                CastFishingRod(transform.position, _direction, m_replicateData.rodLength * m_fishinCastStrengthOverTime.Evaluate(m_castDuration));
                StartCoroutine(StartFishing());
                
                m_castDuration = 0.0f;
                m_isCasting = false;
                m_isPulling = false;
                GetComponent<Character>().animator.SetTrigger("Fishing");
            }
        }
        else if(m_launchCapDuration > 0.5f)
        {
            PullUpFishingRod();
            GetComponent<Character>().animator.SetTrigger("Stop");
            m_isPulling = true;
            StartCoroutine(StopPulling());
        }
    }

    public IEnumerator StartFishing()
    {
        yield return new WaitForSeconds(1.0f);
        if(m_isFishing) m_fishingCap.StartFishing();
    }
    public IEnumerator StopPulling()
    {
        yield return new WaitForSeconds(1.5f);
        m_isPulling = false;
    }
    [Command] void CastingFishingRod() {CastingFishingRodRPC(); }

    [ClientRpc]
    void CastingFishingRodRPC()
    {
        if (!isLocalPlayer && isClient)
        {
            GetComponent<Character>().animator.SetTrigger("Cast");
        }
    }

    [Command]
    void CastFishingRod(Vector2 _originalPos, Vector2 _direction, float _strength)
    {
        m_fishingCap.ResetHooked();
        CastFishingRodRPC(_originalPos, _direction, _strength);
    }
    [ClientRpc] void CastFishingRodRPC(Vector2 _originalPos, Vector2 _direction, float _strength)
    {
        m_isFishing = true;
        m_launchCapDir = _direction;
        m_launchCapsthrength = _strength;
        m_launchCapDuration = 0.0f;
        m_fishingCapOriginalPos = m_fishingCapRef.parent.position;
        m_fishingCapDesiredOriginalPos = _originalPos;
        
        //m_fishingCapRef.parent.gameObject.SetActive(true);
        m_fishingCapRef.parent.localPosition = Vector3.zero;
        m_fishingCapRef.localPosition = Vector3.zero;

        m_isInWater = GameManager.instance.FindFishZoneAtPosition(_originalPos + _direction * _strength);
        if (!isLocalPlayer && isClient)
        {
            GetComponent<Character>().animator.SetTrigger("Fishing");
        }
    }

    [Command]
    void PullUpFishingRod()
    {
        if (m_fishingCap.currentLog == FishingCap.PullUpLog.SUCCESS)
        {
            string fishName = m_fishingCap.getHookedFishName();
            if (fishName == "")
            {
                PullUpFishingRodRPC("", FishingCap.PullUpLog.BREAK, 0);
                Debug.Log("Invalid Fish");
            }
            else
            {
                m_replicateData.CatchFish(fishName);
                int nbCatches = m_replicateData.GetNbCatchFish(fishName);
            
                PullUpFishingRodRPC(m_fishingCap.getHookedFishName(), m_fishingCap.currentLog, nbCatches);
            }
        }
        else PullUpFishingRodRPC("", m_fishingCap.currentLog, 0);
        
        m_fishingCap.ResetHooked();
    }

    [ClientRpc] void PullUpFishingRodRPC(string _hookedFishName, FishingCap.PullUpLog _log, int _nbCatch)
    {
        m_fishingCap.SetBite(false);
        m_isFishing = false;
        //m_fishingCapRef.parent.gameObject.SetActive(false);
        //m_fishingCapRef.parent.localPosition = Vector3.zero;
        m_fishingCapRef.localPosition = Vector3.zero;

        if (isLocalPlayer)
        {
            if(_log == FishingCap.PullUpLog.SUCCESS)
                    GameManager.instance.DrawSuccessFish(_hookedFishName, _nbCatch);
            else
                GameManager.instance.DrawFailFish(_log);
        }
        if (!isLocalPlayer && isClient)
        {
            GetComponent<Character>().animator.SetTrigger("Stop");
            if(_log == FishingCap.PullUpLog.SUCCESS)
                m_showFishToOther.ShowFish(_hookedFishName);
        }
    }

}
