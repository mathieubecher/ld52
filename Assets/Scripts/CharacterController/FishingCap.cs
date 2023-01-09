using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishingCap : NetworkBehaviour
{
    public enum PullUpLog
    {
        SUCCESS,
        BREAK,
        FISHLEAVE,
        NOFISH
    }
    private ReplicateData m_replicateData;
    
    [SerializeField] private Transform m_fishingCapRef;
    public SpriteRenderer fishingCapSprite;
    public SpriteRenderer fishingCapShadow;
    [SerializeField] private GameObject m_ringPrefab;
    
    [SerializeField] private float m_minWaitingDuration = 5.0f;
    [SerializeField] private float m_maxWaitingDuration = 5.0f;
    
    [SerializeField] private Fish m_hookedFish;
    private float m_fishHookedDuration = 0.0f;
    private float m_waitBeforeBite = 0.0f;
    private PullUpLog m_currentLog = PullUpLog.NOFISH;
    private bool m_bite = false;

    public bool hasFishHooked {get => m_fishHookedDuration > 0.0f;}
    public PullUpLog currentLog {get => m_currentLog;}
    
    [Server] public Fish getHookedFish() { return m_hookedFish; }
    [Server] public string getHookedFishName() { return m_hookedFish ? m_hookedFish.fishName : ""; }

    void Awake()
    {
        m_replicateData = GetComponent<ReplicateData>();
    }
    
    void Update()
    {
        if (isServer)
        {
            if (m_waitBeforeBite > 0.0f)
            {
                m_waitBeforeBite -= Time.deltaTime;
                if (m_waitBeforeBite <= 0.0f)
                {
                    Debug.Log("Fish :  "+ m_hookedFish.fishName+ " : " +m_replicateData.strength + " < " + m_hookedFish.sthrength);
                    if (m_replicateData.strength < m_hookedFish.sthrength) m_currentLog = PullUpLog.BREAK;
                    else m_currentLog = PullUpLog.SUCCESS;
                    
                    StartBit(m_hookedFish.biteDuration, false);
                }
            }
        }
        if(isClient)
        {
            setCapColor(m_fishingCapRef.transform.localPosition.y);
        }
        if (isLocalPlayer)
        {
            if (m_fishHookedDuration > 0.0f)
            {
                m_fishHookedDuration -= m_replicateData.resistance > 0.0f ? Time.deltaTime / m_replicateData.resistance : 1.0f;
                if (m_fishHookedDuration <= 0.0f)
                {
                    m_bite = false;
                    LooseFish();
                }
            }
        }
        
    }

    [Command] private void LooseFish()
    {
        //Debug.Log("Loose Fish");
        m_currentLog = PullUpLog.FISHLEAVE;
        LeaveBite();
    }

    [ClientRpc]
    private void LeaveBite()
    {
        m_bite = false;
    }
    
    private void FixedUpdate()
    {
        if (isClient)
        {
            if (m_bite)
            {
                m_fishingCapRef.localPosition = Vector3.Lerp(m_fishingCapRef.localPosition, Vector2.down * 0.5f, 0.3f);
            }
            else
            {
                m_fishingCapRef.localPosition = Vector3.Lerp(m_fishingCapRef.localPosition, Vector2.zero, 0.1f);
            }
        }
    }

    private float m_previousHeight = 0.0f;
    public void setCapColor(float _height)
    {
        Color shadowColor = fishingCapShadow.color;
        shadowColor.a = math.clamp(_height < 0.0f ? 0.0f : 0.3f * (1.0f - _height / 2.0f) + 0.1f, 0.0f, 1.0f);
        fishingCapShadow.color = shadowColor;
        Color capColor = fishingCapSprite.color;
        capColor.a = math.clamp(_height > 0.0f ? 1.0f : 1.0f + _height, 0.0f, 1.0f);
        
        fishingCapSprite.color = capColor;
        if(m_previousHeight >= 0.0f && _height < 0.0f)
            Instantiate(m_ringPrefab, m_fishingCapRef.parent.transform.position, Quaternion.identity);
        
        m_previousHeight = _height;
    }

    public void StartFishing()
    {
        StartFishingCommand(m_fishingCapRef.parent.transform.position);
    }

    [Command]
    void StartFishingCommand(Vector2 _position)
    {
        m_hookedFish = null;
        var fishesInPosition = GameManager.instance.FindFishAtPosition(_position);
        if (fishesInPosition.Count > 0)
        {
            float maxDropValue = 0.0f;
            foreach (Fish fish in fishesInPosition)
            {
                maxDropValue += fish.dropChance;
            }

            float randomValue = Random.Range(0.0f, maxDropValue);
            foreach (Fish fish in fishesInPosition)
            {
                randomValue -= fish.dropChance;
                if (randomValue < 0.0f)
                {
                    //Debug.Log("Select fish : "+fish.fishName + " "+ (m_replicateData.strength < fish.sthrength? "BREAK": ""));
                    m_hookedFish = fish;
                    break;
                }
            }
            
            if(m_hookedFish) m_waitBeforeBite = Random.Range(m_minWaitingDuration, m_maxWaitingDuration); 
        }
    }
    [Server] public void ResetHooked()
    {
        m_hookedFish = null;
        m_waitBeforeBite = 0.0f;
        //Debug.Log("Reset Fish");
        m_currentLog = PullUpLog.NOFISH;
    }

    [ClientRpc]
    void StartBit(float _duration, bool _alreadyHave)
    {
        Instantiate(m_ringPrefab, m_fishingCapRef.parent.transform.position, Quaternion.identity);
        m_bite = true;
        
        if (isLocalPlayer)
        {
            m_fishHookedDuration = _duration;
        }
    }
    public void SetBite(bool _bite)
    {
        m_bite = _bite;
    }


}
