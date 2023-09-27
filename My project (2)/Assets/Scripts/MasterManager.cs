using UnityEngine;

public class MasterManager : MonoBehaviour
{
    public GameEventManager GameEventManager { get; private set; }

    //public UIManager UIIManager;

    public PlayerInputManager PlayerInputManager { get; private set; }

    public PlayerInputs playerInputActions { get; private set; }

    public static MasterManager Instance { get; private set; }

    private void Awake()
    {
        Instance = GetComponent<MasterManager>();

        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        GameEventManager = GetComponentInChildren<GameEventManager>();
        PlayerInputManager = GetComponentInChildren<PlayerInputManager>();
        playerInputActions = new PlayerInputs();
    }
}
