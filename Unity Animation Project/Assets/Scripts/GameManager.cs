using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _me;
    public static GameManager Me
    {
        get
        {
            //if (_me == null)
            //{
            //    return Instantiate(new GameObject().AddComponent<GameManager>());
            //} This was causing objects to not be cleaned up when quitting the application
            return _me;
        }

        private set
        {
            _me = value;
        }
    }

    [SerializeField] private ShooterController _player;
    public ShooterController Player
    {
        get
        {
            if (_player == null)
            {
                _player = new ShooterController();
            }
            return _player;
        }

        set
        {
            if (_player == null)
            {
                _player = new ShooterController();
            }
            _player = value;
        }
    }

    [SerializeField] private List<EnemyShooterController> _enemies;
    public List<EnemyShooterController> Enemies
    {
        get
        {
            if (_enemies == null)
            {
                _enemies = new List<EnemyShooterController>();
            }
            return _enemies;
        }

        set
        {
            if (_enemies == null)
            {
                _enemies = new List<EnemyShooterController>();
            }
            _enemies = value;
        }
    }

    [SerializeField] private int _playerLives;
    public int PlayerLives
    {
        get
        {
            return _playerLives;
        }

        set
        {
            _playerLives = value;
            if (playerCanvasManager)
            {
                playerCanvasManager.LivesText = _playerLives;
            }
        }
    }

    [SerializeField] private int _enemiesKilled;
    public int EnemiesKilled
    {
        get
        {
            return _enemiesKilled;
        }

        set
        {
            _enemiesKilled = value;
        }
    }

    [SerializeField] private bool _paused = false;
    public bool Paused
    {
        get
        {
            return _paused;
        }

        set
        {
            _paused = value;
            Time.timeScale = Paused ? 0.0f : 1.0f;
            UnityEvent pauseEvent = Paused ? Me.onPause : Me.onUnpause;
            pauseEvent.Invoke();
        }
    }

    [Header("Settings")]

    public GameObject playerPrefab;
    public Transform playerSpawnLocation;
    public CanvasManager playerCanvasManager;
    public float playerRespawnDelay = 3;

    [SerializeField] private int _playerMaxLives = 10;
    public int PlayerMaxLives
    {
        get
        {
            return _playerMaxLives;
        }

        private set
        {
            _playerMaxLives = value;
        }
    }

    [SerializeField] private int _enemiesToWin = 15;
    public int EnemiesToWin
    {
        get
        {
            return _enemiesToWin;
        }

        set
        {
            _enemiesToWin = value;
        }
    }

    public string pauseButton = "Pause";
    private Canvas playerCanvas;
    public Canvas pauseCanvas;
    public Canvas endCanvas;
    private Text endCanvasText;

    [TextArea] public string winText;
    [TextArea] public string loseText;

    [SerializeField] private UnityEvent onPause;
    [SerializeField] private UnityEvent onUnpause;

    // Start is called before the first frame update
    void Awake()
    {
        if (_me == null)
        {
            Me = this;
            //DontDestroyOnLoad(this.gameObject); Disabled as it is currently not necessary to preserve the game manager on the main menu
        }
        else
        {
            Destroy(this.gameObject);
        }

        if (playerCanvasManager)
        {
            playerCanvas = playerCanvasManager.GetComponent<Canvas>();
        }

        if (endCanvas)
        {
            endCanvasText = endCanvas.GetComponentInChildren<Text>();
        }
    }

    private void Start()
    {
        SpawnPlayer(true);
        PlayerLives = PlayerMaxLives;
    }

    // Update is called once per frame
    void Update()
    {
        if (endCanvas.enabled)
        {
            return;
        }

        if (Input.GetButtonDown("Pause"))
        {
            Paused = !Paused;
        }

        if (EnemiesKilled >= EnemiesToWin)
        {
            EndGame(winText);
        }
        if (PlayerLives < 0)
        {
            EndGame(loseText);
        }
    }

    public void OnPlayerDeath()
    {
        if (PlayerLives >= 0)
        {
            Invoke("SpawnPlayerInvoke", playerRespawnDelay);
        }
    }

    private void SpawnPlayerInvoke()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer(bool ignoreLives = false)
    {
        if (Player == null)
        {
            if (ignoreLives == false)
            {
                PlayerLives--;
                if (PlayerLives < 0)
                {
                    return;
                }
            }

            ShooterController player = Instantiate(playerPrefab, playerSpawnLocation.position, Quaternion.identity)
                                        .GetComponent<ShooterController>();
            player.AssociatedCanvas = playerCanvasManager;
            Player = player;
            player.AssociatedCanvas.LivesText = PlayerLives;
        }
    }

    public void FlipUIDisplay()
    {
        playerCanvas.enabled = !playerCanvas.enabled;
        pauseCanvas.enabled = !pauseCanvas.enabled;
    }

    public void EndGame(string text)
    {
        EnableOnlyEndCanvas();
        endCanvasText.text = text;
    }

    private void EnableOnlyEndCanvas()
    {
        if (playerCanvas)
        {
            playerCanvas.enabled = false;
        }
        if (pauseCanvas)
        {
            pauseCanvas.enabled = false;
        }
        if (endCanvas)
        {
            endCanvas.enabled = true;
        }
    }

    private void OnApplicationQuit()
    {
        this.enabled = false;
    }
}
