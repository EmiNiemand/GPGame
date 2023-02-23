using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

public class GlowWorm : MonoBehaviour
{
    [SerializeField] private List<GameObject> points;
    [SerializeField] private float moveSpeed = 0.1f;
    private List<Vector2> destinations = new List<Vector2>();
    private Vector2 destination;

    private float t;

    private bool bMoveToNextDestination = false;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < points.Count - 2; i += 2)
        {
            t = Vector2.Distance(points[i].transform.position, points[i + 2].transform.position) / 100;
            for (float j = 0; j <= 1; j += t)
            {
                destinations.Add(
                    Mathf.Pow((1 - j), 2) * points[i].transform.position + 2 * (1 - j) * j * points[i + 1].transform.position +
                    Mathf.Pow(j, 2) * points[i + 2].transform.position
                );
            }
        }

        destination = destinations[0];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!bMoveToNextDestination && (transform.position.x < GameObject.FindGameObjectWithTag("Player").transform.position.x ||
                                       transform.position.y < GameObject.FindGameObjectWithTag("Player").transform.position.y)) bMoveToNextDestination = true;

        if (bMoveToNextDestination) transform.position = Vector2.MoveTowards(transform.position, destination, moveSpeed);

        if (Vector2.Distance(transform.position, destination) < 0.001f)
        {
            if (destinations.IndexOf(destination) == destinations.Count - 1)
            {
                Destroy(gameObject);
                return;
            }
            destination = destinations[destinations.IndexOf(destination) + 1];
            bMoveToNextDestination = false;
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider != null && collider.gameObject.CompareTag("Player"))
        {
            bMoveToNextDestination = true;
        }
    }
}
