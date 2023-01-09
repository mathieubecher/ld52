using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_strength;
    [SerializeField] private TextMeshProUGUI m_rodLength;
    [SerializeField] private TextMeshProUGUI m_resistance;
    [SerializeField] private TextMeshProUGUI m_wallet;

    void Awake()
    {
        ReplicateData.OnUpdateResistance += UpdateResistance;
        ReplicateData.OnUpdateRodLength += UpdateRodLength;
        ReplicateData.OnUpdateStrength += UpdateStrength;
        ReplicateData.OnUpdateWallet += UpdateWallet;
    }

    private void OnDestroy()
    {
        ReplicateData.OnUpdateResistance -= UpdateResistance;
        ReplicateData.OnUpdateRodLength -= UpdateRodLength;
        ReplicateData.OnUpdateStrength -= UpdateStrength;
        ReplicateData.OnUpdateWallet -= UpdateWallet;
    }
    
    private void UpdateResistance(float _resistance, float _price)
    {
        m_resistance.text = "Resistance : " + _resistance;
    }
    
    private void UpdateRodLength(float _rodLength, float _price)
    {
        m_rodLength.text = "Rod Length : " + _rodLength;
    }
    
    private void UpdateStrength(float _strength, float _price)
    {
        m_strength.text = "Strength : " + _strength;
    }
    
    private void UpdateWallet(float _wallet)
    {
        m_wallet.text = _wallet + " $";
    }
}
