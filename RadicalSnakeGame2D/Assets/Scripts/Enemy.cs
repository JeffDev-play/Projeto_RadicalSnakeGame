using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public List<GameObject> bodyParts = new List<GameObject>(); // Lista das partes do corpo do inimigo
    public float moveSpeed = 2f; // Velocidade do inimigo
    public delegate void DeathEvent();
    public event DeathEvent OnDeath; // Evento para a morte do inimigo

    [SerializeField] private float bodySpacing = 0.1f; // Espaçamento entre partes do corpo do inimigo

    private Snake targetPlayer; // O alvo do inimigo
    void Start()
    {
        targetPlayer = FindObjectOfType<Snake>();
    }

    void Update()
    {
        MoveHead();
        MoveBody();
    }
    private void MoveHead()
    {
        // Direção de rotação ao jogador
        Vector3 rotationDirection = (targetPlayer.bodyParts[0].transform.position - bodyParts[0].transform.position).normalized;
        bodyParts[0].transform.position = Vector2.MoveTowards(bodyParts[0].transform.position, targetPlayer.bodyParts[0].transform.position, 
            moveSpeed * Time.deltaTime);
        // Ajusta a rotação da cabeça para apontar em direção ao jogador
        bodyParts[0].transform.up = rotationDirection;

    }
    public void SetSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }
    private void MoveBody()
    {
        // Faz cada parte do corpo seguir suavemente a parte da frente
        for (int i = 1; i < bodyParts.Count; i++)
        {
            // Calcula a posição alvo para cada parte do corpo com base na parte anterior
            Vector3 targetPosition = bodyParts[i - 1].transform.position - bodyParts[i - 1].transform.up * bodySpacing;

            // Move a parte do corpo suavemente em direção à posição alvo
            bodyParts[i].transform.position = Vector3.Lerp(bodyParts[i].transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Calcula a direção para alinhar a rotação com a parte anterior
            Vector3 direction = bodyParts[i - 1].transform.position - bodyParts[i].transform.position;

            // Ajusta a rotação da parte do corpo para coincidir com a direção do movimento
            if (direction.sqrMagnitude > 0.01f) // Só aplica a rotação se houver movimento suficiente
            {
                Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction.normalized);

                // Suaviza a rotação para garantir uma transição fluida
                bodyParts[i].transform.rotation = Quaternion.Lerp(bodyParts[i].transform.rotation, targetRotation, 0.3f);
            }
        }
    }
    public void Die()
    {
        OnDeath.Invoke(); // Chama o evento quando o Inimigo morre
        Destroy(gameObject);
    }
}
