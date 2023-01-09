using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;

    public static GameManager instance
    {
        get
        {
            if (!m_instance)
            {
                m_instance = FindObjectOfType<GameManager>();
            }
            return m_instance;
        }
    }

    private Character playerInstance;
    private SuccessFish successFish;
    private FailFish failFish;


    [SerializeField] private List<Fish> m_fishes;
    public List<Fish> fishes => m_fishes;
    
    [SerializeField] private CinemachineVirtualCamera VMCam;
    public static void SetPlayerInstance(Character _playerInstance) { instance.playerInstance = _playerInstance; }
    public static void AttachVMCamToObject(Transform _object) { instance.VMCam.Follow = _object; }
    public static Vector3 playerPosition { get { return instance.playerInstance.transform.position; } }
    public static ReplicateData playerReplicateData {get {return instance.playerInstance.GetComponent<ReplicateData>();}}

    public delegate void CatchFishDelegate(Fish _fish, int _nbCatch);
    public static event CatchFishDelegate OnCatchFish;
    void Awake()
    {
        successFish = FindObjectOfType<SuccessFish>();
        failFish = FindObjectOfType<FailFish>();
    }
    
    public bool FindFishZoneAtPosition(Vector2 _position)
    {
        List<Fish> fishes = new List<Fish>();
        RaycastHit2D[] hits = Physics2D.RaycastAll(_position, Vector2.zero, 0.0f, layerMask: LayerMask.GetMask("FishingZone"));
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.TryGetComponent(out FishingZone _zone))
            {
                return true;
            }
        }
        return false;
    }
    
    public List<Fish> FindFishAtPosition(Vector2 _position)
    {
        List<Fish> fishes = new List<Fish>();
        RaycastHit2D[] hits = Physics2D.RaycastAll(_position, Vector2.zero, 0.0f, layerMask: LayerMask.GetMask("FishingZone"));
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.TryGetComponent(out FishingZone _zone))
            {
                foreach (Fish fish in _zone.getFishes())
                {
                    if (!fishes.Contains(fish))
                    {
                        //Debug.Log("Found fish : "+fish.fishName);
                        fishes.Add(fish);
                    }
                }
            }
        }
        return fishes;
    }

    public bool isShowingResult = false;

    public void DrawSuccessFish(string _fishName, int _nbCatches)
    {
        Fish fish = m_fishes.Find((x) => x.fishName == _fishName);
        
        if (fish)
        {
            isShowingResult = true;
            successFish.DrawSuccessFish(fish, _nbCatches);
            OnCatchFish?.Invoke(fish, _nbCatches);
        }
    }

    public void DrawFailFish(FishingCap.PullUpLog _log)
    {
        failFish.DrawFailFish(_log);
    }

    public Fish FindFish(string _fishName)
    {
        return m_fishes.Find((x) => x.fishName == _fishName);
    }
}
