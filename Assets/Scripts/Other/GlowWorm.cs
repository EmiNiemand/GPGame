using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class GlowWorm : MonoBehaviour
{
    [SerializeField] private List<GameObject> points;
    [SerializeField] private float minMoveSpeed = 0.1f;
    [SerializeField] private float minDistanceToMove;
    
    private List<Vector2> destinations = new List<Vector2>();
    private Vector2 destination;

    private float t;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < points.Count - 2; i += 2)
        {
            t = Vector2.Distance(points[i].transform.position, points[i + 2].transform.position) / 100;
            for (float j = 0; j <= 1; j += t)
            {
                destinations.Add(
                    Mathf.Pow((1 - j), 2) * points[i].transform.position +
                    2 * (1 - j) * j * points[i + 1].transform.position +
                    Mathf.Pow(j, 2) * points[i + 2].transform.position
                );
            }
        }

        destination = destinations[0];
        
        // TODO: comment before the build
        for(int i = 0; i < destinations.Count - 1; i++)
        {
            Debug.DrawLine(destinations[i], destinations[i + 1], Color.red, 2000f);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var distance = Vector2.Distance(transform.position, GameObject.FindWithTag("Player").transform.position);
        Debug.Log(distance);
        
        if (distance > minDistanceToMove) return;

        float moveSpeed = minMoveSpeed / distance;
        
        transform.position = Vector2.MoveTowards(transform.position, destination, moveSpeed);

        if (Vector2.Distance(transform.position, destination) < 0.001f)
        {
            if (destinations.IndexOf(destination) == destinations.Count - 1)
            {
                Destroy(gameObject);
                return;
            }
            destination = destinations[destinations.IndexOf(destination) + 1];
        }
    }
}
