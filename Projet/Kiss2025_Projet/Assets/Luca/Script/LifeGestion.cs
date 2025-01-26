using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LifeGestion : MonoBehaviour
{
    [SerializeField] public float LifeCurrent = 20.0f;
    [SerializeField] public float LifeMax = 20.0f;
    [SerializeField] private bool isPlayer = false;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip healSound;
    private AudioSource audioSource;

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
        print(" DEGATS : Life: " + LifeCurrent);
        if (LifeCurrent <= 0)
        {
            Destroy(gameObject);
            if (isPlayer)
            {
                SceneManager.LoadScene("S_GameOver");
            }
            else if (gameObject.tag == "Ennemy")
            {
                VagueEnnemy vagueEnnemy = GameObject.Find("GameManager").GetComponent<VagueEnnemy>();
                vagueEnnemy.DecrementEnemiesRemaining();
            }
        }
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
