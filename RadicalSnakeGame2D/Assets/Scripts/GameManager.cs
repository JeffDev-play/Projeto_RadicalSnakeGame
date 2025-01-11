using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject enemyPrefab;                // Prefab do inimigo
    public float timeBetweenSpawns = 2f;          // Intervalo entre o spawn dos inimigos
    public int maxEnemiesPerWave = 5;             // M�ximo de inimigos por wave
    public int currentWave = 1;                   // Wave atual
    public TextMeshProUGUI waveText;              // Refer�ncia para o texto que exibe a wave
    public GameObject victoryScreen;              // Refer�ncia para o objeto de tela de vit�ria
    public float spawnDistanceFromCamera = 10f;   // Dist�ncia da c�mera para spawnar inimigos fora da vis�o
    public float baseEnemySpeed = 5f;       // Velocidade inicial do inimigo
    public float speedIncreasePerWave = 0.5f; // Quanto a velocidade aumenta por wave

    private int remainingEnemiesToSpawn; // Total de inimigos que ainda precisam spawnar
    private int remainingEnemiesToKill; // Total de inimigos que ainda precisam ser mortos
    private List<GameObject> enemiesInWave = new List<GameObject>(); // Lista para monitorar inimigos da wave
    void Start()
    {
        Time.timeScale = 1;
        // Exibe a primeira wave
        StartCoroutine(UpdateWaveText());
        StartWave();
    }
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            if (Input.GetKeyDown(KeyCode.Return)) // Apertando na tecla Enter, come�a a cena de jogo
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
    void StartWave()
    {
        // Inicia o spawn de inimigos com base na quantidade por wave e multiplicado pela wave atual
        remainingEnemiesToSpawn = maxEnemiesPerWave * currentWave; // Quantidade total de inimigos a spawnar
        remainingEnemiesToKill = remainingEnemiesToSpawn;          // Inicialmente, todos precisam ser mortos
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        // Spawn de inimigos de acordo com a quantidade determinada pela wave
        for (int i = 0; i < maxEnemiesPerWave * currentWave; i++)
        {
            // Pega a posi��o aleat�ria fora da vis�o da c�mera
            Vector2 spawnPosition = GetRandomSpawnPosition();

            // Instancia o inimigo
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            enemiesInWave.Add(enemy);

            // Adiciona um callback para quando o inimigo morrer
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            // A velocidade aumenta cada vez mais que as waves avan�am
            float enemySpeed = baseEnemySpeed + (speedIncreasePerWave * (currentWave - 1));
            enemyScript.SetSpeed(enemySpeed);
            //Atribui o m�todo OnEnemyDeath para o evento OnDeath da classe Enemy
            enemyScript.OnDeath += () => OnEnemyDeath(enemy);

            remainingEnemiesToSpawn--; // Reduz o contador de inimigos a spawnar

            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    // Fun��o para obter uma posi��o aleat�ria fora da c�mera
    Vector2 GetRandomSpawnPosition()
    {
        // Obt�m a posi��o da c�mera na tela
        Camera cam = Camera.main;
        float cameraHeight = cam.orthographicSize;
        float cameraWidth = cameraHeight * cam.aspect;

        // Posi��o aleat�ria fora da vis�o da c�mera
        float randomX = Random.Range(-cameraWidth - spawnDistanceFromCamera, cameraWidth + spawnDistanceFromCamera);
        float randomY = Random.Range(-cameraHeight - spawnDistanceFromCamera, cameraHeight + spawnDistanceFromCamera);

        // Garante que a posi��o de spawn n�o esteja no meio da tela
        while (Mathf.Abs(randomX) < cameraWidth || Mathf.Abs(randomY) < cameraHeight)
        {
            randomX = Random.Range(-cameraWidth - spawnDistanceFromCamera, cameraWidth + spawnDistanceFromCamera);
            randomY = Random.Range(-cameraHeight - spawnDistanceFromCamera, cameraHeight + spawnDistanceFromCamera);
        }

        // Retorna a posi��o de spawn
        return new Vector2(randomX, randomY);

    }

    void OnEnemyDeath(GameObject enemy)
    {
        // Remove o inimigo da lista quando ele morrer
        enemiesInWave.Remove(enemy);

        remainingEnemiesToKill--; // Reduz o contador de inimigos restantes para matar

        // Se n�o houver mais inimigos para spawnar e todos foram mortos
        if (remainingEnemiesToKill == 0 && remainingEnemiesToSpawn == 0)
        {
            EndWave();
        }
    }

    void EndWave()
    {
        // Exibe "Wave Cleaned!" e espera 4 segundos antes de come�ar a pr�xima wave
        waveText.text = "Wave Cleaned!";
        Invoke("StartNextWave", 4f);
    }

    void StartNextWave()
    {
        // Verifica se a wave atual � a 10, e se for, ativa a tela de vit�ria
        if (currentWave == 10)
        {
            victoryScreen.SetActive(true); // Ativa o objeto de vit�ria
            Time.timeScale = 0;
            waveText.text = " "; 
            
        }
        else
        {
            // Caso contr�rio, prepara a pr�xima wave
            currentWave++;
            StartCoroutine(UpdateWaveText());
            StartWave();
        }
    }
    IEnumerator UpdateWaveText()
    {
        // Atualiza o texto com o n�mero da wave atual
        waveText.text = "  Wave " + currentWave;

        // Aguarda 3 segundos
        yield return new WaitForSeconds(3f);

        // Apaga o texto
        waveText.text = "";
    }
    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
