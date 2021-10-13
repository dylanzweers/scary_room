using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentController : MonoBehaviour
{
    GameObject target;

    NavMeshAgent agent;

    string loseMessage;


    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        setNewDestination();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void setNewDestination() {
        agent.SetDestination(target.transform.position);
        Invoke("setNewDestination", 1.0f);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            col.gameObject.SetActive(false);

            loseMessage = String.Format("You Lost");

        }
    }

    private void OnGUI()
    {
        if (loseMessage != null)
        {
            GUI.Label(new Rect(325, 150, 300, 30), loseMessage, "box");
        }
    }
}
