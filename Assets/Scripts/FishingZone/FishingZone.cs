using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingZone : MonoBehaviour
{
    [SerializeField] private List<Fish> m_fishesInZone;
    
    public List<Fish> getFishes(){ return m_fishesInZone; }
}
