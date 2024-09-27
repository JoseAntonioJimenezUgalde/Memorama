using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndShoot : MonoBehaviour
{
    public float launchForce = 500f; // Fuerza de lanzamiento
    public int trajectoryPoints = 30; // N�mero de puntos para la l�nea de trayectoria
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
        rb.isKinematic = true; // Desactivamos la f�sica hasta que se lance el objeto

        // Configurar LineRenderer
        lineRenderer.positionCount = 0; // Sin puntos inicialmente
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.enabled = false; // Desactivado hasta que sea necesario
        lineRenderer.material = lineMaterial; // Aseg�rate de asignar un material visible
        lineRenderer.sortingLayerName = "Default"; // Aseg�rate de que est� en la capa correcta
        lineRenderer.sortingOrder = 1; // Para asegurarte que est� frente a otros objetos 2D
    }

    void OnMouseDown()
    {
        // Inicia el arrastre
        dragStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        isDragging = true;
        rb.isKinematic = true; // Desactivamos la f�sica mientras arrastramos
        lineRenderer.enabled = true; // Habilitamos el LineRenderer para mostrar la trayectoria
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            // Actualiza la posici�n del objeto seg�n la posici�n del mouse
            Vector2 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            rb.MovePosition(currentPosition); // Usamos MovePosition para mover el Rigidbody

            // Dibujar la trayectoria
            Vector2 direction = dragStart - currentPosition; // Direcci�n de la fuerza
            DrawTrajectory(rb.position, direction * launchForce / rb.mass);
        }
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            // Finaliza el arrastre y aplica la fuerza
            Vector2 dragEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = dragStart - dragEnd; // Direcci�n opuesta del arrastre
            rb.isKinematic = false; // Activamos la f�sica
            rb.AddForce(direction * launchForce); // Aplicamos la fuerza

            // Desactivar la l�nea despu�s de lanzar
            lineRenderer.enabled = false;
            isDragging = false;
        }
    }

    void DrawTrajectory(Vector2 startPosition, Vector2 initialVelocity)
    {
        // Calculamos los puntos de la trayectoria con base en la f�rmula de movimiento parab�lico
        lineRenderer.positionCount = trajectoryPoints;
        Vector3[] positions = new Vector3[trajectoryPoints];

        for (int i = 0; i < trajectoryPoints; i++)
        {
            float time = i * trajectoryTimeStep;
            // F�rmulas de movimiento bajo gravedad: posici�n = posici�n_inicial + velocidad * tiempo + 0.5 * gravedad * tiempo^2
            Vector2 position = startPosition + initialVelocity * time + 0.5f * Physics2D.gravity * time * time;
            positions[i] = new Vector3(position.x, position.y, 0f); // Aseg�rate que Z sea 0 para ser visible
        }

        lineRenderer.SetPositions(positions); // Establece los puntos calculados en el LineRenderer
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Si quieres resetear la posici�n despu�s de un tiempo tras la colisi�n
        Invoke("ResetPosition", 2f); // Por ejemplo, 2 segundos tras la colisi�n
    }

    void ResetPosition()
    {
        transform.rotation = rotacionInicial;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.position = startPos;
        rb.isKinematic = true; // Volvemos a desactivar la f�sica para arrastrarlo de nuevo
    }


}
