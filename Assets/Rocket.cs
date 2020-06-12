using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource audioSource;
	[SerializeField] float rcsThrust = 100f; //Reaction Control System
	[SerializeField] float mainThrust = 100f;

	enum State 
	{ 
		ALIVE,
		DYING,
		TRANSCENDING
	};

	State state = State.ALIVE;

	// Start is called before the first frame update
	void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource= GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
		if(state == State.ALIVE)
		{
			Thrust();
			Rotation();
		}

    }

	void OnCollisionEnter(Collision collision)
	{
		if(state != State.ALIVE) //ignore collision when dead
		{
			return;
		}

		switch (collision.gameObject.tag)
		{
			case "Friendly":
				//Do Nothing
				break;
			
			case "Finish":
				state = State.TRANSCENDING;
				Invoke("LoadNextScene", 1f); //parameterize time
				break;

			default:
				state = State.DYING;
				Invoke("LoadStartScene", 1f); //parameterize time
				break;
		}
	}

	private void LoadStartScene()
	{
		SceneManager.LoadScene(0);
	}

	private void LoadNextScene()
	{	
		SceneManager.LoadScene(1);
	}

	private void Thrust()
	{
		if (Input.GetKey(KeyCode.W))
		{
			rigidBody.AddRelativeForce(Vector3.up * mainThrust);
			if (!audioSource.isPlaying)
			{
				audioSource.Play();
			}
		}
		else
		{
			audioSource.Stop();
		}
	}
	
	private void Rotation()
	{
		rigidBody.freezeRotation = true;	//take manual control over rotation
		float rotationThatFrame = Time.deltaTime * rcsThrust;

		if (Input.GetKey(KeyCode.A))
		{
			transform.Rotate(Vector3.forward * rotationThatFrame);
		}
		else if (Input.GetKey(KeyCode.D))
		{
			transform.Rotate(-Vector3.forward * rotationThatFrame);
		}
		rigidBody.freezeRotation = false;
	}
}
