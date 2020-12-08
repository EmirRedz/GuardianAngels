using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class LevelManager : MonoBehaviour
{
    Spawner spawner;
    Player player;
    [Header("Pause")]
    public GameObject pauseObject;
    public static bool isPaused;
    [Header("Game Over")]
    public Animator anim, gameOverAnim;
    public float transitionTime;

    [Header("New Wave Banner")]
    public RectTransform newWaveBanner;
    public TextMeshProUGUI title, enemyCount;

    [Header("Wave Stats")]
    public GameObject waveStats;
    public TextMeshProUGUI enemiesReamining, scoreText;

    [Header("Health Bar")]
    public RectTransform healthBar;
    public TextMeshProUGUI playerHealth;

    GameObject[] Enemies;
    public float threshold;
    // Start is called before the first frame update
    private void Awake()
    {
        if (FindObjectOfType<Spawner>() != null)
        {
            spawner = FindObjectOfType<Spawner>();
            spawner.OnNewWave += OnNewWave;
        }
        if (FindObjectOfType<Player>() != null)
        {
            player = FindObjectOfType<Player>();
            player.OnDeath += OnGameOver;
        }
    }

    private void Start()
    {
        //Cursor.visible = false;
        AudioListener.pause = false;
        Time.timeScale = 1;
        if (gameOverAnim != null)
        {
            gameOverAnim.gameObject.SetActive(false);
        }

        if (pauseObject != null)
        {
            pauseObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }

        if (GameObject.FindGameObjectWithTag("EnemySound") != null)
        {
            Enemies = GameObject.FindGameObjectsWithTag("EnemySound");
        }

        if (enemiesReamining != null)
        {
            enemiesReamining.text = "Enemies: " + spawner.enemiesRemainingAlive.ToString();
        }
        if (scoreText != null)
        {
            scoreText.text ="x"+Score.streakCount + "/" + Score.score.ToString("D9");
        }
        float healthPercent = 0;
        if (player != null)
        {
            healthPercent = player.currentHealth / player.health;
        }
        if (healthBar != null || playerHealth != null)
        {
            healthBar.localScale = new Vector3(healthPercent, 1, 1);
            playerHealth.text = "Health: " + Mathf.RoundToInt(player.currentHealth).ToString();
        }

        DeleteAudioSources();
    }

    void OnNewWave(int waveNumber)
    {
        Cursor.visible = false;
        string[] numbers = { "One", "Two","Three","Four","Five"};
        title.text = "- Wave " + numbers[waveNumber - 1] + " -";
        string enemyCountString = ((spawner.waves[waveNumber - 1].infinite)?"Infinite": spawner.waves[waveNumber - 1].enemyCount + "");
        enemyCount.text = "Enemies: " + enemyCountString;

        StopCoroutine("AnimateNewWaveBanner");
        StartCoroutine("AnimateNewWaveBanner");
    }

    private void DeleteAudioSources()
    {
        if (Enemies != null)
        {
            if (Enemies.Length > threshold)
            {
                for (int i = 0; i < Enemies.Length - threshold; i++)
                {
                    Destroy(Enemies[i].GetComponent<AudioSource>());
                }
            }
        }
    }

    public void LoadGame()
    {
        StartCoroutine(StartGame());
    }

    public void Restart()
    {
        StartCoroutine(RestartLevel());
    }

    public void MainMenu()
    {
        StartCoroutine(LoadMainMenu());
    }

    public void QuitGame()
    {
        Application.Quit();
    }


    private void OnGameOver()
    {
        Cursor.visible = true;

        gameOverAnim.gameObject.SetActive(true);
        gameOverAnim.SetTrigger("Fading");

        waveStats.SetActive(false);
    }

    public void PauseGame()
    {
        isPaused = !isPaused;
        pauseObject.SetActive(isPaused);

        if (isPaused)
        {
            Cursor.visible = true;
            Time.timeScale = 0;
            AudioListener.pause = true;
        }

        if (!isPaused)
        {
            Time.timeScale = 1;
            AudioListener.pause = false;
            Cursor.visible = false;
        }
    }


    IEnumerator AnimateNewWaveBanner()
    {
        float speed = 2.5f;
        float delayTime = 1.5f;
        float percent = 0;
        int dir = 1;

        float endDelayTime = Time.time + 1 / speed + delayTime; 
        while(percent >= 0)
        {
            percent += Time.deltaTime * speed * dir;

            if(percent >= 1)
            {
                percent = 1;
                if(Time.time > endDelayTime)
                {
                    dir = -1;
                }
            }

            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-650, -318, percent);
            yield return null;
        }
    }

    private IEnumerator RestartLevel()
    {
        anim.SetTrigger("Fading");
        yield return new WaitForSecondsRealtime(transitionTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator LoadMainMenu()
    {
        anim.SetTrigger("Fading");
        yield return new WaitForSecondsRealtime(transitionTime);
        SceneManager.LoadScene("Menu");
    }

    private IEnumerator StartGame()
    {
        anim.SetTrigger("Fading");
        yield return new WaitForSecondsRealtime(transitionTime);
        SceneManager.LoadScene("Game");
    }




}

  
