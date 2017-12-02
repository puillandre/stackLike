using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TheStack : MonoBehaviour
{
    public Text scoreText;
    public GameObject endPanel;
    public GameObject prefab;
    public ParticleSystem particle;
    public int nbBoxes;
    private const float BOUND_SIZE = 3.5f;
    private const float STACK_MOVING_SPEED = 1.0f;
    private const float ERROR_MARGIN = 0.1f;
    private const float STACK_BOUNDS_GAIN = 0.25f;
    private const int COMBO_BOUNDS_GAIN = 3;

    private GameObject[] theStack;
    private Vector2 stackBounds = new Vector2(BOUND_SIZE, BOUND_SIZE);

    private int scoreCount = 0;
    private int stackIndex = 0;
    private int combo = 0;

    private float tileTransition = 0.0f;
    private float tileSpeed = 2.5f;
    private float secondaryPosition;

    private Vector3 desiredPosition;
    private Vector3 lastTilePosition;

    private bool isMovingOnX = true;
    private bool gameOver = false;

    public int StackIndex
    {
        get { return stackIndex; }
        set { stackIndex = value; }
    }

    // Use this for initialization
    private void Start()
    {
        SpawnBoxes();
    }

    private void SpawnBoxes()
    {
        theStack = new GameObject[nbBoxes];
        for (int i = 0; i < nbBoxes; ++i)
        {
            theStack[i] = Instantiate(prefab, new Vector3(0, -i - 1, 0), Quaternion.identity, transform);
        }
        stackIndex = nbBoxes - 1;
    }

    private void CreateRubble(Vector3 pos, Vector3 scale)
    {
        GameObject go = Instantiate(prefab, pos, Quaternion.identity, transform); // GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.localScale = scale;
        go.AddComponent<Rigidbody>();
		go.GetComponentInChildren<RandomMaterial>().SetMaterialID(theStack[stackIndex].GetComponentInChildren<RandomMaterial>().getMaterialID());
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
            return;
        if (Input.GetButtonUp("Fire1"))
        {
            if (PlaceTile())
            {
                SpawnTile();
                ++scoreCount;
                scoreText.text = scoreCount.ToString();
            }
            else
            {
                EndGame();
            }
        }
        MoveTile();

        //Move the stack
        transform.position = Vector3.Lerp(transform.position, desiredPosition, STACK_MOVING_SPEED * Time.deltaTime);
    }

    private void MoveTile()
    {
        tileTransition += Time.deltaTime * tileSpeed;
        if (isMovingOnX)
            theStack[stackIndex].transform.localPosition = new Vector3(Mathf.Sin(tileTransition) * BOUND_SIZE, scoreCount, secondaryPosition);
        else
            theStack[stackIndex].transform.localPosition = new Vector3(secondaryPosition, scoreCount, Mathf.Sin(tileTransition) * BOUND_SIZE);
    }

    private void SpawnTile()
    {
        lastTilePosition = theStack[stackIndex].transform.localPosition;
        stackIndex = stackIndex - 1;
        if (stackIndex < 0)
            stackIndex = transform.childCount - 1;

        desiredPosition = (Vector3.down) * scoreCount;
        theStack[stackIndex].transform.localPosition = new Vector3(0, scoreCount, 0);
        theStack[stackIndex].transform.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
    }

	private void PlaceCombo(Transform t)
	{
		++combo;
		if (combo > COMBO_BOUNDS_GAIN)
		{
			if (stackBounds.x < BOUND_SIZE)
				stackBounds.x += STACK_BOUNDS_GAIN;
			if (stackBounds.y < BOUND_SIZE)
				stackBounds.y += STACK_BOUNDS_GAIN;
		}
		t.localPosition = new Vector3(lastTilePosition.x, scoreCount, lastTilePosition.z);
	}

	private bool PlaceOnX(Transform t)
	{
		float deltaX = lastTilePosition.x - t.transform.position.x;
		if (Mathf.Abs(deltaX) > ERROR_MARGIN)
		{
			//CUT TILE
			combo = 0;
			stackBounds.x -= Mathf.Abs(deltaX);
			if (stackBounds.x <= 0)
				return false;

			float middle = (lastTilePosition.x + t.localPosition.x) / 2;
			t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
			CreateRubble(
				new Vector3(t.position.x > 0 ? t.position.x + t.localScale.x / 2 : t.position.x - t.localScale.x / 2, t.position.y, t.position.z),
				new Vector3(Mathf.Abs(deltaX), 1, t.localScale.z)
			);
			t.localPosition = new Vector3(middle, scoreCount, lastTilePosition.z);
		}
		else
		{
			PlaceCombo (t);
		}
		return (true);
	}

	private bool PlaceOnY(Transform t)
	{
		float deltaZ = lastTilePosition.z - t.transform.position.z;
		if (Mathf.Abs(deltaZ) > ERROR_MARGIN)
		{
			//CUT TILE
			combo = 0;
			stackBounds.y -= Mathf.Abs(deltaZ);
			if (stackBounds.y <= 0)
				return false;

			float middle = (lastTilePosition.z + t.localPosition.z) / 2;
			t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
			CreateRubble(
				new Vector3(t.position.x, t.position.y, t.position.z > 0 ? t.position.z + t.localScale.z / 2 : t.position.z - t.localScale.z / 2),
				new Vector3(t.localScale.x, 1, Mathf.Abs(deltaZ))
			);
			t.localPosition = new Vector3(lastTilePosition.x, scoreCount, middle);
		}
		else
		{
			PlaceCombo (t);
		}
		return (true);
	}

    private bool PlaceTile()
    {
        Transform t = theStack[stackIndex].transform;

		if (isMovingOnX)
        {
			if (PlaceOnX (t) == false)
			{
				return (false);
			}
			secondaryPosition = t.localPosition.x;
			isMovingOnX = false;
        }
        else
        {
			if (PlaceOnY (t) == false)
				return (false);
			secondaryPosition = t.localPosition.z;
			isMovingOnX = true;
        }
        return (true);
    }

    private void EndGame()
    {
        if (scoreCount > PlayerPrefs.GetInt("score"))
            PlayerPrefs.SetInt("score", scoreCount);
        if (!gameOver)
            theStack[stackIndex].AddComponent<Rigidbody>();
        endPanel.SetActive(true);
        gameOver = true;
    }

    public void OnButtonClick(string sceneName)
    {
        particle.Stop();
        SceneManager.LoadScene(sceneName);
    }
}
