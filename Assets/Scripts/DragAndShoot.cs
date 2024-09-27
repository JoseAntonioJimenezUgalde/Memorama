using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndShoot : MonoBehaviour
{
    public float launchForce = 500f; // Fuerza de lanzamiento
    public int trajectoryPoints = 30; // Número de puntos para la línea de trayectoria
    public float trajectoryTimeStep = 0.1f; // Tiempo entre los puntos de la trayectoria
    public Material lineMaterial; // Material para el LineRenderer

    private Rigidbody2D rb;
    private Vector2 startPos;
    private Vector2 dragStart;
    private bool isDragging = false;

    private LineRenderer lineRenderer;
    private Quaternion rotacionInicial;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rotacionInicial = transform.rotation;
        lineRenderer = GetComponent<LineRenderer>();
        startPos = rb.position;
        rb.isKinematic = true; // Desactivamos la física hasta que se lance el objeto

        // Configurar LineRenderer
        lineRenderer.positionCount = 0; // Sin puntos inicialmente
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.enabled = false; // Desactivado hasta que sea necesario
        lineRenderer.material = lineMaterial; // Asegúrate de asignar un material visible
        lineRenderer.sortingLayerName = "Default"; // Asegúrate de que esté en la capa correcta
        lineRenderer.sortingOrder = 1; // Para asegurarte que está frente a otros objetos 2D
    }

    void OnMouseDown()
    {
        // Inicia el arrastre
        dragStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        isDragging = true;
        rb.isKinematic = true; // Desactivamos la física mientras arrastramos
        lineRenderer.enabled = true; // Habilitamos el LineRenderer para mostrar la trayectoria
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            // Actualiza la posición del objeto según la posición del mouse
            Vector2 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            rb.MovePosition(currentPosition); // Usamos MovePosition para mover el Rigidbody

            // Dibujar la trayectoria
            Vector2 direction = dragStart - currentPosition; // Dirección de la fuerza
            DrawTrajectory(rb.position, direction * launchForce / rb.mass);
        }
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            // Finaliza el arrastre y aplica la fuerza
            Vector2 dragEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = dragStart - dragEnd; // Dirección opuesta del arrastre
            rb.isKinematic = false; // Activamos la física
            rb.AddForce(direction * launchForce); // Aplicamos la fuerza

            // Desactivar la línea después de lanzar
            lineRenderer.enabled = false;
            isDragging = false;
        }
    }

    void DrawTrajectory(Vector2 startPosition, Vector2 initialVelocity)
    {
        // Calculamos los puntos de la trayectoria con base en la fórmula de movimiento parabólico
        lineRenderer.positionCount = trajectoryPoints;
        Vector3[] positions = new Vector3[trajectoryPoints];

        for (int i = 0; i < trajectoryPoints; i++)
        {
            float time = i * trajectoryTimeStep;
            // Fórmulas de movimiento bajo gravedad: posición = posición_inicial + velocidad * tiempo + 0.5 * gravedad * tiempo^2
            Vector2 position = startPosition + initialVelocity * time + 0.5f * Physics2D.gravity * time * time;
            positions[i] = new Vector3(position.x, position.y, 0f); // Asegúrate que Z sea 0 para ser visible
        }

        lineRenderer.SetPositions(positions); // Establece los puntos calculados en el LineRenderer
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Si quieres resetear la posición después de un tiempo tras la colisión
        Invoke("ResetPosition", 2f); // Por ejemplo, 2 segundos tras la colisión
    }

    void ResetPosition()
    {
        transform.rotation = rotacionInicial;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.position = startPos;
        rb.isKinematic = true; // Volvemos a desactivar la física para arrastrarlo de nuevo
    }


}
