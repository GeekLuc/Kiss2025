using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class AttackPlayer : MonoBehaviour
{
    public GameObject[] weaponPrefabs;
    private Move moveScript;
    private HUD_Player hudPlayer;
    private MunitionPlayer munitionPlayer;
    private int currentAttackIndex = 0;
    public Animator animator;
    [SerializeField] private AudioClip swordAttackSound;
    [SerializeField] private AudioClip grenadeAttackSound;
    [SerializeField] private AudioClip sardinneAttackSound;
    [SerializeField] private AudioClip paralysedAttackSound;
    [SerializeField] private AudioClip tourelleAttackSound;
    private AudioSource audioSource;
    // Ajoutez cette variable pour le délai d'attaque
    [SerializeField] private float swordAttackDelay = 1.0f;
    private bool canAttack = true;
    void Start()
    {
        moveScript = GetComponent<Move>();
        hudPlayer = FindObjectOfType<HUD_Player>();
        munitionPlayer = GetComponent<MunitionPlayer>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing.");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            PerformAttack();
        }
        SwitchWeapon();
    }

    void SwitchWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeWeapon(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) ChangeWeapon(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) ChangeWeapon(4);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            if (scroll > 0)
            {
                currentAttackIndex = (currentAttackIndex + 1) % weaponPrefabs.Length;
            }
            else
            {
                currentAttackIndex = (currentAttackIndex - 1 + weaponPrefabs.Length) % weaponPrefabs.Length;
            }
            hudPlayer.UpdateWeapon(currentAttackIndex);
            UpdateMunitionHUD();
        }
    }

    void ChangeWeapon(int index)
    {
        if (index >= 0 && index < weaponPrefabs.Length)
        {
            currentAttackIndex = index;
            hudPlayer.UpdateWeapon(currentAttackIndex);
            UpdateMunitionHUD();
        }
    }

    void UpdateMunitionHUD()
    {
        int currentMunition = munitionPlayer.GetCurrentMunition(currentAttackIndex);
        hudPlayer.UpdateMunitionText(currentMunition);
    }


    void PerformAttack()
    {
        if (weaponPrefabs[currentAttackIndex].name == "Sword" || munitionPlayer.UseMunition(currentAttackIndex))
        {
            Debug.Log("Attaque de " + weaponPrefabs[currentAttackIndex].name);
            if (weaponPrefabs[currentAttackIndex].name == "Sardinnne")
            {
                ThrowSardinne();
                animator.Play("ThrowPecheur", 0, 0f);
                PlaySound(sardinneAttackSound);
            }
            if (weaponPrefabs[currentAttackIndex].name == "Grenade")
            {
                ThrowGrenade();
                animator.Play("ThrowPecheur", 0, 0f);
                PlaySound(grenadeAttackSound);
            }
            if (weaponPrefabs[currentAttackIndex].name == "Sword")
            {
                StartCoroutine(SwordAttackWithDelay());
            }
            if (weaponPrefabs[currentAttackIndex].name == "Paralysed")
            {
                ThrowParalysed();
                animator.Play("ThrowPecheur", 0, 0f);
                PlaySound(paralysedAttackSound);
            }
            if (weaponPrefabs[currentAttackIndex].name == "Tourelle")
            {
                DropTourelle();
                animator.Play("SwordPecheur", 0, 0f);
                PlaySound(tourelleAttackSound);
            }
            UpdateMunitionHUD();
        }
        else
        {
            Debug.Log("Pas de munitions pour " + weaponPrefabs[currentAttackIndex].name);
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private IEnumerator SwordAttackWithDelay()
    {
        canAttack = false;
        SwordAttack();
        animator.Play("SwordPecheur", 0, 0f);
        PlaySound(swordAttackSound);
        yield return new WaitForSeconds(swordAttackDelay);
        canAttack = true;
    }

    //ATTAQUE DU LANCER DE POISSON, Dague//
    public void ThrowSardinne()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject sardinne = Instantiate(weaponPrefabs[currentAttackIndex], transform.position, Quaternion.identity);
            Vector3 direction = (hit.point - transform.position).normalized;
            sardinne.transform.LookAt(hit.point); // Ajuster l'orientation
            sardinne.transform.Rotate(0, 90, 0); // Rotation suppl�mentaire de 90 degr�s
            StartCoroutine(MoveSardinne(sardinne, direction));
        }
    }

    private IEnumerator MoveSardinne(GameObject sardinne, Vector3 direction)
    {
        Sardinnne sardinneScript = sardinne.GetComponent<Sardinnne>();
        float speed = sardinneScript != null ? sardinneScript.speed : 5f;
        float fixedY = sardinne.transform.position.y;

        while (sardinne != null)
        {
            Vector3 newPosition = sardinne.transform.position + direction * speed * Time.deltaTime;
            newPosition.y = fixedY;
            sardinne.transform.position = newPosition;
            yield return null;
        }
    }

    //ATTAQUE DU LANCER DE GRENADE//
    public void ThrowGrenade()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = new Vector3(hit.point.x, 0.2f, hit.point.z);
            GameObject grenade = Instantiate(weaponPrefabs[currentAttackIndex], transform.position, Quaternion.identity);
            grenade.transform.LookAt(targetPosition); // Ajuster l'orientation
            grenade.transform.Rotate(0, 90, 0); // Rotation suppl�mentaire de 90 degr�s
            StartCoroutine(MoveGrenade(grenade, targetPosition));
        }
    }

    private IEnumerator MoveGrenade(GameObject grenade, Vector3 targetPosition)
    {
        Grenade grenadeScript = grenade.GetComponent<Grenade>();
        float speed = grenadeScript != null ? grenadeScript.speed : 10f;
        float height = grenadeScript != null ? grenadeScript.height : 5f;
        Vector3 startPosition = grenade.transform.position;
        float journeyLength = Vector3.Distance(startPosition, targetPosition);
        float startTime = Time.time;

        while (grenade != null && Vector3.Distance(grenade.transform.position, targetPosition) > 0.1f)
        {
            float distCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distCovered / journeyLength;
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);
            currentPosition.y += height * Mathf.Sin(Mathf.PI * fractionOfJourney);

            grenade.transform.position = currentPosition;
            yield return null;
        }

        if (grenadeScript != null)
        {
            grenadeScript.Boom();
        }
    }

    //ATTAQUE DE L'EPEE//
    public void SwordAttack()
    {
        float portee = 3.0f;

        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = hit.point;
            Vector3 direction = (targetPosition - transform.position).normalized;
            Vector3 spawnPosition = transform.position + direction * portee;
            spawnPosition.y = transform.position.y;
            GameObject sword = Instantiate(weaponPrefabs[currentAttackIndex], spawnPosition, Quaternion.identity);
            Sword swordComponent = sword.GetComponent<Sword>();
            sword.transform.LookAt(new Vector3(targetPosition.x, sword.transform.position.y, targetPosition.z));
            sword.transform.Rotate(0, 90, 0);
            StartCoroutine(SwordSwing(sword, direction));
        }
    }

    private IEnumerator SwordSwing(GameObject sword, Vector3 direction)
    {
        float swingSpeed = 500.0f;
        float currentAngle = -45.0f;

        Vector3 rotationAxis = Vector3.up;

        sword.transform.RotateAround(transform.position, rotationAxis, currentAngle);

        while (currentAngle < 45.0f)
        {
            float stepAngle = swingSpeed * Time.deltaTime;
            float angleToRotate = Mathf.Min(stepAngle, 45.0f - currentAngle);
            sword.transform.RotateAround(transform.position, rotationAxis, angleToRotate);

            currentAngle += angleToRotate;
            yield return null;
        }
        Destroy(sword);
    }

    //ATTAQUE DU LANCER DE PARALYSIE//
    public void ThrowParalysed()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject Paralysed = Instantiate(weaponPrefabs[currentAttackIndex], transform.position, Quaternion.identity);
            Vector3 direction = (hit.point - transform.position).normalized;
            Paralysed.transform.LookAt(hit.point); // Ajuster l'orientation
            Paralysed.transform.Rotate(0, 90, 0); // Rotation suppl�mentaire de 90 degr�s
            StartCoroutine(MoveParalysed(Paralysed, direction));
        }
    }

    private IEnumerator MoveParalysed(GameObject Paralysed, Vector3 direction)
    {
        Paralysed ParalysedScript = Paralysed.GetComponent<Paralysed>();
        float speed = ParalysedScript != null ? ParalysedScript.speed : 5f;
        float fixedY = Paralysed.transform.position.y;

        while (Paralysed != null)
        {
            Vector3 newPosition = Paralysed.transform.position + direction * speed * Time.deltaTime;
            newPosition.y = fixedY;
            Paralysed.transform.position = newPosition;
            yield return null;
        }
    }

    //ATTAQUE DEPOT DE LA TOURELLE//
    public void DropTourelle()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 SpawnPosition = new Vector3(hit.point.x, 0.2f, hit.point.z);
            GameObject Tourelle = Instantiate(weaponPrefabs[currentAttackIndex], SpawnPosition, Quaternion.identity);
        }
    }
}
