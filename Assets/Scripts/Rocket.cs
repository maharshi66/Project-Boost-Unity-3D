using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource audioSource;
	[SerializeField] float rcsThrust = 100f; //Reaction Control System
	[SerializeField] float mainThrust = 100f;

	[SerializeField] AudioClip mainEngine;
	[SerializeField] AudioClip rocketDeath;
	[SerializeField] AudioClip levelFinished;

	[SerializeField] ParticleSystem mainEngineParticle;
	[SerializeField] ParticleSystem successParticle;
	[SerializeField] ParticleSystem deathParticle;

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
			RespondToThrustInput();
			RespondToRotationInput();
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
				StartSuccessSequence();
				break;

			default:
				StartDeathSequence();
				break;
		}
	}

	private void StartDeathSequence()
	{
		state = State.DYING;
		audioSource.Stop();
		deathParticle.Play();
		audioSource.PlayOneShot(rocketDeath);
		Invoke("LoadStartScene", 1f); //parameterize time
	}

	private void StartSuccessSequence()
	{
		state = State.TRANSCENDING;
		audioSource.Stop();
		audioSource.PlayOneShot(levelFinished);
		successParticle.Play();
		Invoke("LoadNextScene", 1f); //parameterize time
	}

	private void LoadStartScene()
	{
		SceneManager.LoadScene(0);
	}

	private void LoadNextScene()
	{	
		SceneManager.LoadScene(1);
	}

	private void RespondToThrustInput()
	{
		if (Input.GetKey(KeyCode.W))
		{
			ApplyThrust();
		}
		else
		{
			audioSource.Stop();
			mainEngineParticle.Stop();
		}
	}
	private void ApplyThrust()
	{
		rigidBody.AddRelativeForce(Vector3.up * mainThrust);
		if (!audioSource.isPlaying)
		{
			audioSource.PlayOneShot(mainEngine);
		}
		mainEngineParticle.Play();
	}
	private void RespondToRotationInput()
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
