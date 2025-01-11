using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Snake : MonoBehaviour
{
    [SerializeField] private GameObject head; // Cabe�a da cobra
    [SerializeField] private GameObject bodyPrefab; // Prefab das partes do corpo
    [SerializeField] private float moveSpeed = 5f; // Velocidade da cobra
    [SerializeField] private float rotationSpeed = 200f; // Velocidade de rota��o
    [SerializeField] private float bodySpacing = 0.1f; // Espa�amento entre partes do corpo
    [SerializeField] private GameObject fruitPrefab; // Prefab das frutas
    [SerializeField] private GameObject alvo;  // Ponteiro indicando onde o disparo vai
    [SerializeField] private GameObject bulletPrefab; // Prefab das balas
    [SerializeField] private GameObject healthBar; // Objeto da healthBar que vai servir para a healthBar seguir o player
    [SerializeField] private GameObject gameOver; // Tela de GameOver
    [SerializeField] private TextMeshProUGUI waveText; // Texto dos waves
    [SerializeField] private GameObject explosionPrefab; // Prefab das mortes dos inimigos
    [SerializeField] private Vector3 offset; // Esse Vector3 serve para ajustar a posi��o da healthBar do player
    [SerializeField] private UnityEngine.UI.Slider healthBarSlider; // Vai servir para modificar o valor da healthBar
    [SerializeField] private float bulletSpeed = 0.5f; // Velocidade da bala
    [SerializeField] private float timerShoot; // Tempo para o pr�ximo disparo
    [SerializeField] private float maxHealth; // Vida m�xima do Player

    private Collider2D[] childColliders; // Colisores das partes do corpo para verificar se o inimigo colidiu com algum deles
    private float currentHealth; // Vida atual do Player
    private float timer; // Cron�metro
    private float minX, maxX, minY, maxY; // Limites para a posi��o da fruta

    public float margin = 4f; // Margem que impede a fruta de spawnar muito pr�xima das bordas
    public List<GameObject> bodyParts = new List<GameObject>(); // Lista das partes do corpo

    private void Start()
    {
        SetCameraBounds();
        // Chama o m�todo SpawnFruit imediatamente e depois a cada 30 segundos
        InvokeRepeating("SpawnFruit", 0f, 20f);
        // Esconde o cursor do mouse
        Cursor.visible = false;
        // Vida atual vai receber a vida m�xima
        currentHealth = maxHealth;
        
        childColliders = GetComponentsInChildren<Collider2D>();
        // Chama o m�todo para atualizar logo de in�cio o valor da barra de vida
         UpdateHealthBar();

    }
    private void Update()
    {
        MoveHead(); // Movimenta a cabe�a
        MoveBody(); // Movimenta as partes do corpo
        Vector3 mousePosition = Input.mousePosition; // Pega a posi��o do mouse
        mousePosition.z = 10f;  // Colocando o alvo em uma profundidade vis�vel
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        alvo.transform.position = targetPosition; // Faz com que a mira fique sempre na mesma posi��o do mouse

        // Detecta o clique do mouse
        timer += Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && timer > timerShoot) // Bot�o esquerdo do mouse
        {
            Shoot();
            timer = 0;
        }
        if (Input.GetKeyDown(KeyCode.Space)) // Pressionando a tecla Space o cursor do mouse torna-se visivel novamente
        {
            Cursor.visible = !Cursor.visible;

        }

        healthBar.transform.position = bodyParts[0].transform.position + offset; // Barra de vida fica na mesma posi��o do player

        // Cria um filtro para verificar apenas colis�es com a camada "Enemy"
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(LayerMask.GetMask("Enemy")); // Define o filtro para a camada Enemy

        foreach (var childCollider in childColliders)
        {
            // Array para armazenar os resultados das colis�es
            Collider2D[] results = new Collider2D[2]; // Array para armazenar at� 2 colis�es

            // Verifica se o filho est� colidindo com um objeto da camada "Enemy"
            int hitCount = Physics2D.OverlapCollider(childCollider, contactFilter, results);

            // Se houver colis�es
            for (int i = 0; i < hitCount; i++)
            {
                Collider2D collision = results[i];
                if (collision != null)
                {
                    //Debug.Log($"Colis�o detectada com {collision.gameObject.name}");
                      Enemy pos = collision.transform.parent.GetComponent<Enemy>();
                      GameObject explosion = Instantiate(explosionPrefab, pos.bodyParts[0].transform.position, Quaternion.identity);
                      pos.Die();
                      currentHealth--;
                      UpdateHealthBar();
                   
                    if (currentHealth == 0)
                    {
                        gameOver.SetActive(true);
                        waveText.text = " ";
                        Time.timeScale = 0;
                    }
                    
                }
            }
        }
    }
    public void SpawnFruit()
    {
        // Gera uma posi��o aleat�ria dentro dos limites da c�mera
        float randomX = Random.Range(minX + margin, maxX - margin);
        float randomY = Random.Range(minY + margin, maxY - margin);

        // Instancia a fruta na posi��o aleat�ria
        Vector3 spawnPosition = new Vector3(randomX, randomY, 0f);
        Instantiate(fruitPrefab, spawnPosition, Quaternion.identity);
    }

    public void SetCameraBounds()
    {
        // Pega os limites da c�mera
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            // Pega as posi��es do canto inferior esquerdo e superior direito da c�mera
            Vector3 lowerLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
            Vector3 upperRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

            // Define os limites de gera��o da fruta
            minX = lowerLeft.x;
            maxX = upperRight.x;
            minY = lowerLeft.y;
            maxY = upperRight.y;
        }
    }
    private void MoveHead()
    {
        // Move a cabe�a para frente constantemente
        head.transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        // Rotaciona a cabe�a com as teclas horizontais (esquerda/direita)
        float rotationInput = Input.GetAxis("Horizontal");
        head.transform.Rotate(Vector3.forward, -rotationInput * rotationSpeed * Time.deltaTime);
    }

    private void MoveBody()
    {
        // Faz cada parte do corpo seguir suavemente a parte da frente
        for (int i = 1; i < bodyParts.Count; i++)
        {
            // Calcula a posi��o alvo para cada parte do corpo com base na parte anterior
            Vector3 targetPosition = bodyParts[i - 1].transform.position - bodyParts[i - 1].transform.up * bodySpacing;

            // Move a parte do corpo suavemente em dire��o � posi��o alvo
            bodyParts[i].transform.position = Vector3.Lerp(bodyParts[i].transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Calcula a dire��o para alinhar a rota��o com a parte anterior
            Vector3 direction = bodyParts[i - 1].transform.position - bodyParts[i].transform.position;

            // Ajusta a rota��o da parte do corpo para coincidir com a dire��o do movimento
            if (direction.sqrMagnitude > 0.01f) // S� aplica a rota��o se houver movimento suficiente
            {
                Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction.normalized);

                // Suaviza a rota��o para garantir uma transi��o fluida
                bodyParts[i].transform.rotation = Quaternion.Lerp(bodyParts[i].transform.rotation, targetRotation, 0.3f);
            }
        }
    }
    void Shoot()
    {
        // Obt�m a posi��o do mouse em coordenadas do mundo
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        // Calcula a dire��o da bala (da cabe�a do player para o mouse)
        Vector3 direction = (mousePosition - bodyParts[0].transform.position).normalized;

        // Instancia a bala na posi��o da cabe�a do player
        GameObject bullet = Instantiate(bulletPrefab, bodyParts[0].transform.position, Quaternion.identity);

        // Configura a rota��o da bala para apontar na dire��o do mouse
        bullet.transform.up = direction;

        // Adiciona velocidade � bala na dire��o do mouse
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * bulletSpeed;
        }
    }
    public void RestorativeFruit()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth++; // Recupera uma vida
        }
        UpdateHealthBar(); // Atualiza a barra de vida
    }
    void UpdateHealthBar()
    {
        healthBarSlider.value = (float)currentHealth; // Atualiza a barra de vida
    }
}