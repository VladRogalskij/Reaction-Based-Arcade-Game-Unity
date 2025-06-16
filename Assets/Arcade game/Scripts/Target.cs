using System.Collections;
using UnityEngine;

public class Target : MonoBehaviour
{
    private Rigidbody rb;
    private GameManager gameManager;
    public int pointValue;
    public GameObject explosionFx;

    public float timeOnScreen = 1.0f;

    [HideInInspector] public int cellX;
    [HideInInspector] public int cellY;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        StartCoroutine(RemoveObjectRoutine());
    }

    private void OnMouseDown()
    {
        if (gameManager.isGameActive)
        {
            gameManager.ClearGridCell(cellX, cellY);
            Destroy(gameObject);
            Explode();

            if (gameObject.CompareTag("Bad"))
            {
                gameManager.GameOver();
                return;
            }

            gameManager.UpdateScore(pointValue);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        gameManager.ClearGridCell(cellX, cellY);
        Destroy(gameObject);

        if (other.gameObject.CompareTag("Sensor") && !gameObject.CompareTag("Bad"))
        {
            gameManager.GameOver();
        }
    }

    void Explode()
    {
        Instantiate(explosionFx, transform.position, explosionFx.transform.rotation);
    }

    IEnumerator RemoveObjectRoutine()
    {
        yield return new WaitForSeconds(timeOnScreen);
        if (gameManager.isGameActive)
        {
            gameManager.ClearGridCell(cellX, cellY);
            transform.Translate(Vector3.forward * 5, Space.World);
        }
    }
}
