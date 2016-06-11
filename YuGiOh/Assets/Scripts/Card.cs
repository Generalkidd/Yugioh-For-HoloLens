using System.Collections;
using UnityEngine;

public class Card : MonoBehaviour
{
    Vector3 originalPosition;
    public string CardName = "";
    public string CardType = "";
    private Vector3 spawnPos = new Vector3(6.5f, -1.5f, 6.5f);
    public int attack = 0;
    public int defense = 0;
    public int level = 0;
    public Quaternion rotation = new Quaternion(0f, 90f, 0f, 0f);
    
    // Use this for initialization
    void Start()
    {
        // Grab the original local position of the sphere when the app starts.
        originalPosition = this.transform.localPosition;
    }

    // Called by GazeGestureManager when the user performs a Select gesture
    IEnumerator OnSelect()
    {
    SpawnCheck:
        if (CardType == "Monster")
        {
            if (!Physics.CheckSphere(spawnPos, 0.35f))
            {
                this.transform.rotation = Quaternion.Euler(-90, 180, 0);
                this.transform.position = spawnPos;
                yield return new WaitForSeconds(2);

                GameObject monster;
                monster = (GameObject)Instantiate(GameObject.Find(CardName), new Vector3(this.transform.position.x, -0.5f, 6.5f), rotation);
            }
            else
            {
                float temp = spawnPos.x;
                if (temp - 0.5f >= -3f)
                {
                    spawnPos = new Vector3(temp - 0.5f, -1.5f, 6.5f);
                    goto SpawnCheck;
                }
            }
        }
        else if(CardType == "Spell")
        {
            if (!Physics.CheckSphere(spawnPos, 0.35f))
            {
                this.transform.rotation = Quaternion.Euler(-90, 180, 0);
                this.transform.position = spawnPos;
            }
            else
            {
                float temp = spawnPos.x;
                if (temp - 0.5f >= -3f)
                {
                    spawnPos = new Vector3(temp - 0.5f, -1.5f, 5.5f);
                    goto SpawnCheck;
                }
            }
        }

        // If the sphere has no Rigidbody component, add one to enable physics.
        //if (!this.GetComponent<Rigidbody>())
        //{
        //    var rigidbody = this.gameObject.AddComponent<Rigidbody>();
        //    rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        //}

    }

    // Called by SpeechManager when the user says the "Reset world" command
    void OnReset()
    {
        // If the sphere has a Rigidbody component, remove it to disable physics.
        var rigidbody = this.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            DestroyImmediate(rigidbody);
        }

        // Put the sphere back into its original local position.
        this.transform.localPosition = originalPosition;
    }

    // Called by SpeechManager when the user says the "Drop sphere" command
    void OnDrop()
    {
        // Just do the same logic as a Select gesture.
        OnSelect();
    }
}