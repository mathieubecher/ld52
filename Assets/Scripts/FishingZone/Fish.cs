using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Fish/New Fish", order = 1)]
public class Fish : ScriptableObject
{
    [SerializeField] private string m_name;
    [SerializeField] private Sprite m_sprite;
    [SerializeField] private float m_biteDuration = 1.0f;
    [SerializeField] private float m_dropChance = 1.0f;
    [SerializeField] private float m_sthrength = 1.0f;
    [SerializeField] private float m_price = 1.0f;
    [SerializeField] private bool m_masked = false;
    public float biteDuration {get => m_biteDuration;}
    public string fishName {get => m_name;}
    public Sprite fishSprite {get => m_sprite;}
    public float dropChance {get => m_dropChance;}
    public float sthrength {get => m_sthrength;}
    public float price {get => m_price;}
    public bool masked {get => m_masked;}
}
