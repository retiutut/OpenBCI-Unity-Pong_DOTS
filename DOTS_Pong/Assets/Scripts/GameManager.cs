using System.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager main;
	public Canvas renderCanvas;

	public GameObject ballPrefab;
	public GameObject brainflowPrefab;
	public Text mainTextPrefab;
	private Text mainText;

	public float xBound = 3f;
	public float yBound = 3f;
	public float ballSpeed = 3f;
	public float respawnDelay = 2f;
	public int[] playerScores;

	public Text[] playerTexts;
	public Text[] playerData;
	public Text playerAccelData;

	Entity ballEntityPrefab;
	EntityManager manager;

	WaitForSeconds oneSecond;
	WaitForSeconds delay;
	WaitForSeconds halfSecond;
    private Vector3 pos;
    private Quaternion rot;

    private void Awake()
	{
		if (main != null && main != this)
		{
			Destroy(gameObject);
			return;
		}

		main = this;
        playerScores = new int[2];

		Debug.Log("Game Settings: " + MainMenu.playerSelect + " , " + MainMenu.boardSelect + " , " + MainMenu.controlMethod);

		manager = World.DefaultGameObjectInjectionWorld.EntityManager;

		GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
		ballEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(ballPrefab, settings);

		oneSecond = new WaitForSeconds(1f);
		halfSecond = new WaitForSeconds(.5f);
		delay = new WaitForSeconds(respawnDelay);

        Instantiate(brainflowPrefab);


		mainText = Instantiate(mainTextPrefab, Vector3.zero, Quaternion.identity) as Text;
		mainText.transform.SetParent(renderCanvas.transform, false);
		mainText.text = "";
		mainText.gameObject.SetActive(false);

		StartCoroutine(CountdownAndSpawnBall(mainText));
	}

	public void PlayerScored(int playerID)
	{
		playerScores[playerID]++;
		for (int i = 0; i < playerScores.Length && i < playerTexts.Length; i++)
			playerTexts[i].text = playerScores[i].ToString();

		StartCoroutine(CountdownAndSpawnBall(mainText));
	}

	public void updatePlayerData(int channel, string text)
    {
		playerData[channel].text = text;
	}

	public void updatePlayerAccelData(string text)
    {
		playerAccelData.text = text;
    }

	IEnumerator CountdownAndSpawnBall(Text t)
	{
		t.gameObject.SetActive(true);
		t.text = "Get Ready";
		yield return delay;

		t.text = "3";
		yield return oneSecond;

		t.text = "2";
		yield return oneSecond;

		t.text = "1";
		yield return oneSecond;

		t.text = "Meow!";
		yield return halfSecond;

		t.gameObject.SetActive(false);

		SpawnBall();
	}

	void SpawnBall()
	{
		Entity ball = manager.Instantiate(ballEntityPrefab);

		Vector3 dir = new Vector3(UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1, UnityEngine.Random.Range(-.5f, .5f), 0f).normalized;
		Vector3 speed = dir * ballSpeed;

		PhysicsVelocity velocity = new PhysicsVelocity()
		{
			Linear = speed,
			Angular = float3.zero
		};

		manager.AddComponentData(ball, velocity);
	}

	void Destroy()
	{
		//boardSelect.onValueChanged.RemoveAllListeners();
	}

    public void Start()
    {
		/*
		boardSelect.onValueChanged.AddListener(delegate {
			myDropdownValueChangedHandler(boardSelect);
		});
		*/
	}

	void Update()
	{
		if (Input.GetKey("escape"))
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
		}
	}
}

