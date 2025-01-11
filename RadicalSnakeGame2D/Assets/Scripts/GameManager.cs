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
    public int maxEnemiesPerWave = 5;             // Máximo de inimigos por wave
    public int currentWave = 1;                   // Wave atual
    public TextMeshProUGUI waveText;              // Referência para o texto que exibe a wave
    public GameObject victoryScreen;              // Referência para o objeto de tela de vitória
    public float spawnDistanceFromCamera = 10f;   // Distância da câmera para spawnar inimigos fora da visão
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
            if (Input.GetKeyDown(KeyCode.Return)) // Apertando na tecla Enter, começa a cena de jogo
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
            // Pega a posição aleatória fora da visão da câmera
            Vector2 spawnPosition = GetRandomSpawnPosition();

            // Instancia o inimigo
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            enemiesInWave.Add(enemy);

            // Adiciona um callback para quando o inimigo morrer
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            // A velocidade aumenta cada vez mais que as waves avançam
            float enemySpeed = baseEnemySpeed + (speedIncreasePerWave * (currentWave - 1));
            enemyScript.SetSpeed(enemySpeed);
            //Atribui o método OnEnemyDeath para o evento OnDeath da classe Enemy
            enemyScript.OnDeath += () => OnEnemyDeath(enemy);

            remainingEnemiesToSpawn--; // Reduz o contador de inimigos a spawnar

            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    // Função para obter uma posição aleatória fora da câmera
    Vector2 GetRandomSpawnPosition()
    {
        // Obtém a posição da câmera na tela
        Camera cam = Camera.main;
        float cameraHeight = cam.orthographicSize;
        float cameraWidth = cameraHeight * cam.aspect;

        // Posição aleatória fora da visão da câmera
        float randomX = Random.Range(-cameraWidth - spawnDistanceFromCamera, cameraWidth + spawnDistanceFromCamera);
        float randomY = Random.Range(-cameraHeight - spawnDistanceFromCamera, cameraHeight + spawnDistanceFromCamera);

        // Garante que a posição de spawn não esteja no meio da tela
        while (Mathf.Abs(randomX) < cameraWidth || Mathf.Abs(randomY) < cameraHeight)
        {
            randomX = Random.Range(-cameraWidth - spawnDistanceFromCamera, cameraWidth + spawnDistanceFromCamera);
            randomY = Random.Range(-cameraHeight - spawnDistanceFromCamera, cameraHeight + spawnDistanceFromCamera);
        }

        // Retorna a posição de spawn
        return new Vector2(randomX, randomY);

    }

    void OnEnemyDeath(GameObject enemy)
    {
        // Remove o inimigo da lista quando ele morrer
        enemiesInWave.Remove(enemy);

        remainingEnemiesToKill--; // Reduz o contador de inimigos restantes para matar

        // Se não houver mais inimigos para spawnar e todos foram mortos
        if (remainingEnemiesToKill == 0 && remainingEnemiesToSpawn == 0)
        {
            EndWave();
        }
    }

    void EndWave()
    {
        // Exibe "Wave Cleaned!" e espera 4 segundos antes de começar a próxima wave
        waveText.text = "Wave Cleaned!";
        Invoke("StartNextWave", 4f);
    }

    void StartNextWave()
    {
        // Verifica se a wave atual é a 10, e se for, ativa a tela de vitória
        if (currentWave == 10)
        {
            victoryScreen.SetActive(true); // Ativa o objeto de vitória
            Time.timeScale = 0;
            waveText.text = " "; 
            
        }
        else
        {
            // Caso contrário, prepara a próxima wave
            currentWave++;
            StartCoroutine(UpdateWaveText());
            StartWave();
        }
    }
    IEnumerator UpdateWaveText()
    {
        // Atualiza o texto com o número da wave atual
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
