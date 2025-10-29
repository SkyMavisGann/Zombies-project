using UnityEngine;

public class Effect : MonoBehaviour
{
    AudioSource effectAudio;

    GameObject particleEffect;
    private float volume;

    private void Start()
    {
        effectAudio = gameObject.GetComponent<AudioSource>();
        volume = 0.7f;
    }

    public void Play(RaycastHit hit, AudioClip hitSound, GameObject hitEffect, float effectDuration)
    {
        if (hitEffect != null)
        {
            particleEffect = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(particleEffect, effectDuration);
        }

        effectAudio.PlayOneShot(hitSound, volume);
    }
}
