using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private float health = 100f;
    private NavMeshAgent agent;
    private Transform target;
    private Rigidbody rb;
    private bool alive;
    private Manager manager;
    private float Cooldown;
    private Player player;
   
    private void Start()
    {
        alive = true;
        manager = FindObjectOfType<Manager>();
        rb = GetComponent<Rigidbody>();
        target = FindObjectOfType<Player>().transform;
        agent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<Player>();
    }

    public void ItsAlive()
    {
        health = 100f;
        agent.enabled = rb.isKinematic = alive = true;
    }

    private void Update()
    {
        if (alive && Vector3.Distance(transform.position, target.position) < 100f)
            agent.SetDestination(target.position);
        if (Cooldown > 0) { Cooldown -= Time.deltaTime;}
        if(Cooldown<= 0 && Vector3.Distance(transform.position, target.position) < 5.25f)
        {
            Ataque();
        }
    }

    public void Damage(float damage)
    {
        health -= damage;
        if (alive && health <= 0)
        {
            manager.QueueSpawn(transform);
            manager.PlaySound("player", "kills");
            agent.enabled = rb.isKinematic = alive = false;           
            rb.AddForce(-transform.forward * 250f);
            manager.Kills++;
            sumakills();
        }
    }
     void Ataque()
    {
        Cooldown = 3f;
        manager.life = player.health = player.health - 5f;        
    }
    void sumakills()
    {
        if (gameObject.CompareTag("Red"))
        {
            manager.KillsRed++;
            manager.Score = manager.Score + 7;
        }
        if (gameObject.CompareTag("White")){
            manager.KillsWhite++;
            manager.Score = manager.Score + 2;
        }
        if (gameObject.CompareTag("Orange"))
        {
            manager.KillsOrange++;
            manager.Score = manager.Score + 5;
        }
        if (gameObject.CompareTag("Black"))
        {
            manager.KillsBlack++;
            manager.Score = manager.Score + 10;
        }
    }
}
