using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// The game manager object
    /// </summary>
    private static GameManager _bowBeforeMe;
    public static GameManager BowBeforeMe
    {
        get
        {
            if (_bowBeforeMe == null)
            {
                return Instantiate(new GameObject().AddComponent<GameManager>());
            }
            return _bowBeforeMe;
        }

        private set
        {
            _bowBeforeMe = value;
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
            if (playerCanvas)
            {
                playerCanvas.LivesText = _playerLives;
            }
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
        }
    }

    [Header("Settings")]

    public GameObject playerPrefab;
    public Transform playerSpawnLocation;
    public CanvasManager playerCanvas;
    public float playerRespawnDelay = 1;
    public KeyCode pauseKey;

    [SerializeField] private int _playerMaxLives = 10;
    public int PlayerMaxLives
    {
        get
        {
            return _playerMaxLives;
        }

        /*private set
        {
            _playerMaxLives = value;
        }*/
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (_bowBeforeMe == null)
        {
            BowBeforeMe = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        SpawnPlayer();
        PlayerLives = PlayerMaxLives;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            Paused = !Paused;
        }
    }

    public void OnPlayerDeath()
    {
        if (PlayerLives > 0)
        {
            Invoke("SpawnPlayer", playerRespawnDelay);
        }
    }

    private void SpawnPlayer()
    {
        if (Player == null)
        {
            PlayerLives--;
            ShooterController player = Instantiate(playerPrefab, playerSpawnLocation.position, Quaternion.identity)
                                        .GetComponent<ShooterController>();
            player.AssociatedCanvas = playerCanvas;
            Player = player;
            player.AssociatedCanvas.LivesText = PlayerLives;
        }
    }
}
