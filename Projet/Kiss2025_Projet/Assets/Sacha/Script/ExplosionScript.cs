using System.Collections;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    [SerializeField] private float destroyDelay = 2.0f; // Temps en secondes avant de détruire l'objet

    private void Start()
    {
        // Démarre le timer pour détruire l'objet
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        // Attend la durée définie par destroyDelay
        yield return new WaitForSeconds(destroyDelay);

        // Détruit l'objet
        Destroy(gameObject);
    }

    public void DestroyIt()
    {
        // Méthode publique pour détruire immédiatement
        Destroy(gameObject);
    }
}
