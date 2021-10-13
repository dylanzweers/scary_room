using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentController : MonoBehaviour
{
    Transform target;

    NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        setNewDestination();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void setNewDestination() {
        agent.SetDestination(target.position);
        Invoke("setNewDestination", 1.0f);
    }

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("humping");
        Vector3 resetPos = new Vector3(0,0,0);
        if (col.CompareTag("Player")) {
            Debug.Log("trying");
            target.position = resetPos;
        }
    }
}
