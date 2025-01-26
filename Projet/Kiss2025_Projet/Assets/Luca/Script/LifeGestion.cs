using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LifeGestion : MonoBehaviour
{
    [SerializeField] public float LifeCurrent = 20.0f;
    [SerializeField] public float LifeMax = 20.0f;
    [SerializeField] private bool isPlayer = false;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip healSound;
    [SerializeField] private SpriteRenderer charachterSprite;
    [SerializeField] private float hitFlashDuration = 0.5f; // Temps pendant lequel le sprite reste rouge
    private AudioSource audioSource;
    private Coroutine hitCoroutine; // Stocke la coroutine en cours

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing.");
        }
    }

    public void TakeDamage(float damage)
    {
        LifeCurrent -= damage;
        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        Debug.Log("DEGATS : Life: " + LifeCurrent);
        hit();

        if (LifeCurrent <= 0)
        {
            Destroy(gameObject);
            if (isPlayer)
            {
                SceneManager.LoadScene("S_GameOver");
            }
            else if (gameObject.CompareTag("Ennemy"))
            {
                VagueEnnemy vagueEnnemy = GameObject.Find("GameManager").GetComponent<VagueEnnemy>();
                vagueEnnemy.DecrementEnemiesRemaining();
            }
        }
    }

    private void hit()
    {
        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
        }
        
        hitCoroutine = StartCoroutine(FlashRed());
    }

    private IEnumerator FlashRed()
    {
        charachterSprite.color = Color.red;
        
        yield return new WaitForSeconds(hitFlashDuration);
        
        charachterSprite.color = Color.white;
        
        hitCoroutine = null;
    }

    public void TakeHealth(float health)
    {
        LifeCurrent += health;
        if (audioSource != null && healSound != null)
        {
            audioSource.PlayOneShot(healSound);
        }
        if (LifeCurrent > LifeMax)
        {
            LifeCurrent = LifeMax;
        }
    }
}
