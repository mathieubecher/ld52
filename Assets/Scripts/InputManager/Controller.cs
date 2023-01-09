using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    private PlayerInput m_playerInput;
    private Camera m_mainCameraRef;
    
    [HideInInspector]
    public Vector2 moveInput;
    
    private Vector2 m_targetDirection;
    public Vector2 targetDirection
    {
        get
        {
            if (m_playerInput.currentControlScheme == "Mouse and Keyboard")
            {
                Vector3 mousePos = m_mainCameraRef.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                Vector3 direction = (mousePos - GameManager.playerPosition);
                direction.z = 0f;
                direction.Normalize();
                m_targetDirection = direction;
            }
            return m_targetDirection;
        }
    }
    
    public delegate void Click();
    public static event Click OnClick;
    
    public delegate void Release();
    public static event Release OnRelease;
    
    public delegate void Interact();
    public static event Interact OnInteract;
    
    public delegate void Escape();
    public static event Escape OnEscape;
    
    private void Awake()
    {
        m_playerInput = GetComponent<PlayerInput>();
        m_mainCameraRef = Camera.main;
    }

    private static bool openUI = false;
    public static void OpenUI()
    {
        openUI = true;
    }

    public static void CloseUI()
    {
        openUI = false;
    }
    
    public void ReadMoveInput(InputAction.CallbackContext _context)
    {
        if (openUI)
        {
            moveInput = Vector2.zero;
            return;
        }
        
        moveInput = _context.ReadValue<Vector2>();
    }

    public void ReadClickInput(InputAction.CallbackContext _context)
    {
        if (openUI) return;
        
        if (_context.performed)
            OnClick?.Invoke();
        else if(_context.canceled)
            OnRelease?.Invoke();
    }

    public void ReadTargetDirection(InputAction.CallbackContext _context)
    {
        if (openUI)
        {
            m_targetDirection = Vector2.zero;
            return;
        }

        Vector2 targetDirection = _context.ReadValue<Vector2>();
        if (targetDirection.magnitude < 0.3f)
            return;
        
        m_targetDirection = targetDirection;
    }
    
    public void ReadInteractInput(InputAction.CallbackContext _context)
    {
        if (_context.performed)
            OnInteract?.Invoke();
    }
    
    public void ReadEscapeInput(InputAction.CallbackContext _context)
    {
        if (FindObjectOfType<MerchantInterface>().open)
        {
            if (_context.performed)
                OnInteract?.Invoke();
            return;
        }

        if (_context.performed)
            OnEscape?.Invoke();
    }
}
