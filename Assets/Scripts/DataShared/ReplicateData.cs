using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class ReplicateData : NetworkBehaviour
{
    [Serializable]
    class FishCatch
    {
        public string fishName;
        public int nbCatch;

        public FishCatch(string _fishName, int _nbCatch)
        {
            fishName = _fishName;
            nbCatch = _nbCatch;
        }
    }
    
    [SerializeField] private List<FishCatch> m_fishesCatch;
    [SerializeField] private float m_wallet = 0.0f;

    [Header("FishingRod Stat")]
    [SerializeField] private float m_strength = 1.0f;
    [SerializeField] private float m_resistance = 1.0f;
    [SerializeField] private float m_rodLength = 7.0f;
    
    public float strength => m_strength;
    public float resistance => m_resistance;
    public float rodLength => m_rodLength;
    
    public delegate void UpdateWalletDelegate(float _wallet);
    public static event UpdateWalletDelegate OnUpdateWallet;
    
    public delegate void UpdateResistanceDelegate(float _resistance, float _price);
    public static event UpdateResistanceDelegate OnUpdateResistance;
    
    public delegate void UpdateRodLengthDelegate(float _rodLength, float _price);
    public static event UpdateRodLengthDelegate OnUpdateRodLength;
    
    public delegate void UpdateStrengthDelegate(float _strength, float _price);
    public static event UpdateStrengthDelegate OnUpdateStrength;

    void Start()
    {
        if (isServer)
        {
            UpdateWallet(m_wallet);
            UpdateRodeLength(m_rodLength, m_rodLengthUpgradeList.Count > 0 ? m_rodLengthUpgradeList[0].price : -1.0f);
            UpdateStrength(m_strength, m_strengthUpgradeList.Count > 0 ? m_strengthUpgradeList[0].price : -1.0f);
            UpdateResistance(m_resistance, m_resistanceUpgradeList.Count > 0 ? m_resistanceUpgradeList[0].price : -1.0f);
        }
    }
    
    [Server] public void CatchFish(string _fishName)
    {
        Fish fish = GameManager.instance.FindFish(_fishName);
        if (!fish) return;
        
        FishCatch fishCatch = m_fishesCatch.Find((x) => x.fishName == _fishName);

        if (fishCatch == null)
        {
            fishCatch = new FishCatch(_fishName, 1);
            m_fishesCatch.Add(fishCatch);
        }
        else ++fishCatch.nbCatch;
        UpdateFishCatch(fishCatch.fishName, fishCatch.nbCatch);
        
        m_wallet += fish.price;
        UpdateWallet(m_wallet);
    }

    [ClientRpc]
    void UpdateFishCatch(string _fishName, int _nbCatch)
    {
        
        FishCatch fishCatch = m_fishesCatch.Find((x) => x.fishName == _fishName);

        if (fishCatch == null)
        {
            fishCatch = new FishCatch(_fishName, _nbCatch);
            m_fishesCatch.Add(fishCatch);
        }
        else fishCatch.nbCatch = _nbCatch;
    }
    
    [ClientRpc]
    void UpdateWallet(float _wallet)
    {
        m_wallet = _wallet;
        if (isLocalPlayer)
        {
            OnUpdateWallet?.Invoke(m_wallet);
        }
    }
    
    public int GetNbCatchFish(string _fishName)
    {
        FishCatch fishCatch = m_fishesCatch.Find((x) => x.fishName == _fishName);

        if (fishCatch == null)
            return 0;

        return fishCatch.nbCatch;
    }

    [Serializable]
    struct UpgradePriceAndValue
    {
        public float value;
        public float price;
    }
    [Header("Rod Length")]
    [SerializeField] private List<UpgradePriceAndValue> m_rodLengthUpgradeList;
    [SerializeField] private int m_rodLengthCurrentUpgrade = 0;

    [Command]
    public void UpgradeRodeLength()
    {
        if (m_rodLengthCurrentUpgrade >= m_rodLengthUpgradeList.Count)
            return;
        
        float price = m_rodLengthUpgradeList[m_rodLengthCurrentUpgrade].price;
        float value = m_rodLengthUpgradeList[m_rodLengthCurrentUpgrade].value;
        if (m_wallet < price)
            return;

        m_wallet -= price;
        UpdateWallet(m_wallet);
        
        m_rodLength = value;
        m_rodLengthCurrentUpgrade++;
        UpdateRodeLength(m_rodLength, m_rodLengthCurrentUpgrade >= m_rodLengthUpgradeList.Count? -1 : m_rodLengthUpgradeList[m_rodLengthCurrentUpgrade].price);
    }
    
    [ClientRpc]
    void UpdateRodeLength(float _rodLength, float _price)
    {
        m_rodLength = _rodLength;
        if (isLocalPlayer)
        {
            OnUpdateRodLength?.Invoke(m_rodLength, _price);
        }
    }

    [Header("Resistance")]
    [SerializeField] private List<UpgradePriceAndValue> m_resistanceUpgradeList;
    private int m_resistanceCurrentUpgrade = 0;
    
    [Command]
    public void UpgradeResistance()
    {
        if (m_resistanceCurrentUpgrade >= m_resistanceUpgradeList.Count)
            return;
        
        float price = m_resistanceUpgradeList[m_resistanceCurrentUpgrade].price;
        float value = m_resistanceUpgradeList[m_resistanceCurrentUpgrade].value;
        if (m_wallet < price)
            return;

        m_wallet -= price;
        UpdateWallet(m_wallet);
        
        m_resistance = value;
        m_resistanceCurrentUpgrade++;
        UpdateResistance(m_resistance, m_resistanceCurrentUpgrade >= m_resistanceUpgradeList.Count? -1 : m_resistanceUpgradeList[m_resistanceCurrentUpgrade].price);
    }

    [ClientRpc]
    void UpdateResistance(float _resistance, float _price)
    {
        m_resistance = _resistance;
        if (isLocalPlayer)
        {
            OnUpdateResistance?.Invoke(m_resistance, _price);
        }
    }

    [Header("Strength")]
    [SerializeField] private List<UpgradePriceAndValue> m_strengthUpgradeList;
    private int m_strengthCurrentUpgrade = 0;
    
    [Command]
    public void UpgradeStrength()
    {
        if (m_strengthCurrentUpgrade >= m_strengthUpgradeList.Count)
            return;
        
        float price = m_strengthUpgradeList[m_strengthCurrentUpgrade].price;
        float value = m_strengthUpgradeList[m_strengthCurrentUpgrade].value;
        if (m_wallet < price)
            return;

        m_wallet -= price;
        UpdateWallet(m_wallet);
        
        m_strength = value;
        m_strengthCurrentUpgrade++;
        UpdateStrength(m_strength, m_strengthCurrentUpgrade >= m_strengthUpgradeList.Count? -1 : m_strengthUpgradeList[m_strengthCurrentUpgrade].price);
    }
    [ClientRpc]
    void UpdateStrength(float _strength, float _price)
    {
        m_strength = _strength;
        if (isLocalPlayer)
        {
            OnUpdateStrength?.Invoke(m_strength, _price);
        }
    }
    
}
