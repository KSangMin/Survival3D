using UnityEngine;

public class FootSteps : MonoBehaviour
{
    public AudioClip[] footStepClip;
    private AudioSource audioSource;
    private Rigidbody rb;
    public float footStemThreshold;
    public float footStepRate;
    private float footStepTime;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(Mathf.Abs(rb.velocity.y) < 0.1f)
        {
            if(rb.velocity.magnitude > footStemThreshold)
            {
                if(Time.time - footStepTime > footStepRate)
                {
                    footStepTime = Time.time;
                    audioSource.PlayOneShot(footStepClip[Random.Range(0, footStepClip.Length)]);
                }
            }
        }
    }
}
