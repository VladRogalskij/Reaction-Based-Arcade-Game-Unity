using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI gameOverText;
    public GameObject titleScreen;
    public Button restartButton;

    public List<GameObject> targetPrefabs;

    public float spawnRate = 1.5f;
    public bool isGameActive;

    private int score;
    private float time;

    private float spaceBetweenSquares = 2.5f;
    private float minValueX = -3.75f;
    private float minValueY = -3.75f;

    private bool[,] gridOccupied = new bool[4, 4];

    public void StartGame(int difficulty)
    {
        isGameActive = true;
        titleScreen.SetActive(false);

        time = 0;
        spawnRate /= difficulty;

        StartCoroutine(SpawnTarget());
        StartCoroutine(IncreaseSpawnRate());

        score = 0;
        UpdateScore(score);
    }

    private void Update()
    {
        if (isGameActive)
        {
            time += Time.deltaTime;
            timerText.SetText("Time: " + Mathf.Round(time));
        }
    }

    IEnumerator SpawnTarget()
    {
        while (isGameActive)
        {
            yield return new WaitForSeconds(spawnRate);

            int index = Random.Range(0, targetPrefabs.Count);
            Vector3 spawnPos = RandomSpawnPosition(out int x, out int y);

            if (x == -1 && y == -1)
            {
                Debug.LogWarning("Усі клітинки зайняті — спавн пропущено");
                continue;
            }

            GameObject newTarget = Instantiate(targetPrefabs[index], spawnPos, targetPrefabs[index].transform.rotation);

            Target targetScript = newTarget.GetComponent<Target>();
            if (targetScript != null)
            {
                targetScript.cellX = x;
                targetScript.cellY = y;
            }
        }
    }

    IEnumerator IncreaseSpawnRate()
    {
        while (isGameActive)
        {
            yield return new WaitForSeconds(5);
            UpdateSpawnRate();
        }
    }

    public Vector3 RandomSpawnPosition(out int x, out int y)
    {
        int attempts = 0;

        do
        {
            x = Random.Range(0, 4);
            y = Random.Range(0, 4);
            attempts++;

            if (attempts > 50)
            {
                x = y = -1;
                return new Vector3(float.MinValue, float.MinValue, 0);
            }

        } while (gridOccupied[x, y]);

        gridOccupied[x, y] = true;

        float spawnPosX = minValueX + (x * spaceBetweenSquares);
        float spawnPosY = minValueY + (y * spaceBetweenSquares);

        return new Vector3(spawnPosX, spawnPosY, 0);
    }

    public void ClearGridCell(int x, int y)
    {
        if (x >= 0 && x < 4 && y >= 0 && y < 4)
        {
            gridOccupied[x, y] = false;
        }
    }

    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = "Score: " + score;
    }

    public void UpdateSpawnRate()
    {
        spawnRate -= spawnRate / 20f;
    }

    public void GameOver()
    {
        gameOverText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        isGameActive = false;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
