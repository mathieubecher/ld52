using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Character : NetworkBehaviour
{
    private ReplicateData m_replicateData;
    private Controller m_controller;
    private Rigidbody2D m_rigidBody;
    private FishingRod m_fishingRod;
    [SerializeField] public Animator animator;
    
    [SerializeField] private float m_speed = 3.0f;
    [SerializeField] public Transform cameraTarget;

    private Vector3 m_previousPos;
    
    void Start()
    {
        if (isLocalPlayer)
        {
            m_controller = FindObjectOfType<Controller>();
            m_rigidBody = GetComponent<Rigidbody2D>();

            Controller.OnClick += ReceiveClickInput;
            Controller.OnRelease += ReceiveReleaseInput;
            GameManager.SetPlayerInstance(this);
            GameManager.AttachVMCamToObject(cameraTarget);
            gameObject.tag = "Player";
        }

        m_previousPos = transform.position;
        m_fishingRod = GetComponent<FishingRod>();
        m_replicateData = GetComponent<ReplicateData>();
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            Vector2 velocity = m_controller.moveInput * m_speed;
            if (m_fishingRod.isFishing || m_fishingRod.isCasting || m_fishingRod.isPulling)
            {
                velocity = Vector2.zero;   
                animator.SetFloat("speed", 0.0f);
                animator.SetFloat("sensX", !m_fishingRod.isCasting? 0.0f : m_controller.targetDirection.x*10.0f);
            }
            else
            {
                animator.SetFloat("speed", velocity.magnitude);
                animator.SetFloat("sensX", velocity.x);
            }
            
            m_rigidBody.velocity = velocity;
            
        }
        else
        {
            Vector2 velocity = (transform.position - m_previousPos)/Time.deltaTime;
            animator.SetFloat("speed", velocity.magnitude);
            animator.SetFloat("sensX", velocity.x);
            m_previousPos = transform.position;
        }
    }

    void ReceiveClickInput()
    {
        m_fishingRod.ReceiveInput(m_controller.targetDirection);
    }
    void ReceiveReleaseInput()
    {
        m_fishingRod.ReceiveInput(m_controller.targetDirection, false);
    }
    

}
