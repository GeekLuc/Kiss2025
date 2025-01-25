using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public AudioClip explosionSound; // Assignez le fichier audio de l'explosion dans l'inspecteur
    private AudioSource audioSource;

    [SerializeField] public float damage, TimeBeforeBOOM, speed = 10f, height = 5f;
    public void Boom(){
        StartCoroutine(ExplodeAfterDelay());
    }

    private IEnumerator ExplodeAfterDelay(){
        yield return new WaitForSeconds(TimeBeforeBOOM);

        Collider[] hitColliders = Physics.OverlapBox(transform.position, GetComponent<BoxCollider>().size / 2);
        foreach (var hitCollider in hitColliders){
            if (hitCollider.CompareTag("Ennemy")){
                LifeGestion lifeGestion = hitCollider.GetComponent<LifeGestion>();
                if (lifeGestion != null){
                    lifeGestion.TakeDamage(damage);
                }
            }
        }
        print("BOOM");
        PlayExplosionSound();
        Destroy(gameObject);
    }
    private void PlayExplosionSound()
    {
        if (explosionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }
    }
}
