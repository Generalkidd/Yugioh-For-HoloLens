using System.Collections;
using UnityEngine;

public class CardCommand3 : MonoBehaviour
{
    Vector3 originalPosition;

    // Use this for initialization
    void Start()
    {
        // Grab the original local position of the sphere when the app starts.
        originalPosition = this.transform.localPosition;
    }

    // Called by GazeGestureManager when the user performs a Select gesture
    IEnumerator OnSelect()
    {
        this.transform.rotation = Quaternion.Euler(-90, 180, 0);
        this.transform.position = new Vector3(-0.5f, -1.5f, 6.5f);
        // If the sphere has no Rigidbody component, add one to enable physics.
        //if (!this.GetComponent<Rigidbody>())
        //{
        //    var rigidbody = this.gameObject.AddComponent<Rigidbody>();
        //    rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        //}

        yield return new WaitForSeconds(2);

        Quaternion rotation = transform.rotation;
        rotation.x = 0f;
        rotation.y = 0f;
        rotation.z = 0f;

        GameObject monster;
        monster = (GameObject)Instantiate(GameObject.Find("MysticalElf"), new Vector3(-0.5f, -0.5f, 6.5f), rotation);
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