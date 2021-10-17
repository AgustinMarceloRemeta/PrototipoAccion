using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform gunRoot;
    private Transform playerCamera;
    public float health = 100f;
    private bool alive;
    private Rigidbody rb;
    private FirstPersonAIO fpc;
    private AudioSource source;
    private AudioClip gunShot;
    private Manager manager;

    private void Start()
    {
        alive = true;
        rb = GetComponent<Rigidbody>();
        playerCamera = Camera.main.transform;
        manager = FindObjectOfType<Manager>();
        fpc = GetComponent<FirstPersonAIO>();
        source = gameObject.AddComponent<AudioSource>();
        source.clip = gunShot = Resources.Load<AudioClip>("effect_shot");
    }

    public void ItsAlive()
    {
        health = 100f;
        fpc.enabled = rb.isKinematic = alive = true;
    }

    private void Update()
    {
        if (!alive) return;
        gunRoot.rotation = playerCamera.rotation;

        if (Input.GetMouseButtonDown(0) && Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hit)
            && hit.collider.gameObject.CompareTag("Enemy"))
        {
            source.PlayOneShot(gunShot, 0.3f);
            bool headshot = hit.collider.gameObject.name.Contains("Head");
            Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
            enemy.Damage(headshot ? 50f : 20f);
            if(health <= 0) { Dead(); }
        
        }
    }

    public void Damage(float damage)
    {
        health -= damage;
        if (alive && health <= 0)
        {
            manager.PlaySound("player", "killed");
            manager.QueueSpawn(transform);
            fpc.enabled = rb.isKinematic = alive = false;
            Dead();
        }
    }
    void Dead() {
        manager.player.position = manager.Spawn.position + Vector3.up;
        health = 100;
        manager.Muertes++;
        manager.Score = manager.Score - 5;
        manager.Vida.text = "Vida: 100";
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("star") == true)
        {
            manager.Score = manager.Score + 50;
            manager.bonus = manager.bonus + 50;
            other.gameObject.SetActive(false);
        }
    }
}
