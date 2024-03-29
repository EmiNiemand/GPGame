using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using UnityEngine.U2D;

public class GlowWorm : MonoBehaviour
{
    [SerializeField] private List<GameObject> points;
    [SerializeField] private float minMoveSpeed = 0.1f;
    [SerializeField] private float minDistanceToMove;
    
    [SerializeField] private float transitionTime = 2.5f;
    private float transitionTimer = 0;
    private bool transitionFinished = false;
    private float maxIntensity = 1000;
    private float maxRadius = 50;

    private Light2D light;

    private GameObject player;
    private List<Vector2> destinations = new List<Vector2>();
    private Vector2 destination;
    private bool stopped;
    private float t;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        light = GetComponent<Light2D>();
        // Calculate path
        // --------------
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
        
#if (UNITY_EDITOR) 
        for(int i = 0; i < destinations.Count - 1; i++)
        {
            Debug.DrawLine(destinations[i], destinations[i + 1], Color.red, 2000f);
        }
#endif
    }

    public void SetStopped(bool stop = true) { stopped = stop; }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        //TODO: figure out what to do with this thing
        // if (stopped) return;

        var distance = Vector2.Distance(transform.position, player.transform.position);
        // Debug.Log(distance);
        
        if (distance > minDistanceToMove) return;

        float moveSpeed = minMoveSpeed / distance;
        
        transform.position = Vector2.MoveTowards(transform.position, destination, moveSpeed);
        
        if (Vector2.Distance(transform.position, destination) < 0.001f)
        {
            if (destinations.IndexOf(destination) == destinations.Count - 1)
            {
                StartCoroutine(IETransition());
                return;
            }
            destination = destinations[destinations.IndexOf(destination) + 1];
        }
    }


    private IEnumerator IETransition()
    {
        player.GetComponent<PlayerMovement>().bBlockMovement = true;
        yield return new WaitUntil(() => Transition());
        player.GetComponent<PlayerMovement>().bBlockMovement = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private bool Transition()
    {
        transitionTimer += Time.deltaTime;
        if (transitionTimer >= transitionTime) return true;

        light.pointLightOuterRadius += maxRadius / transitionTime * Time.deltaTime;
        light.intensity += maxIntensity / transitionTime * Time.deltaTime;
        light.color = Color.Lerp(light.color, Color.white, transitionTime * Time.deltaTime);
        return false;
    }
    
}
