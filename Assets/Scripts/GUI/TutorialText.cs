using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialText : MonoBehaviour
{
    public GameObject treeText;
    public GameObject golemText;
    public GameObject elevatorText;
    public GameObject playerText;
    public GameObject hideText;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        treeText.gameObject.SetActive(false);
        golemText.gameObject.SetActive(false);
        elevatorText.gameObject.SetActive(false);
        playerText.gameObject.SetActive(true);
        hideText.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(treeText.transform.position, player.transform.position) < 3f) {
            treeText.gameObject.SetActive(true);
        } else {
            treeText.gameObject.SetActive(false);
        }
        if (Vector3.Distance(golemText.transform.position, player.transform.position) < 3f) {
            golemText.gameObject.SetActive(true);
        } else {
            golemText.gameObject.SetActive(false);
        }
        if (Vector3.Distance(elevatorText.transform.position, player.transform.position) < 3f) {
            elevatorText.gameObject.SetActive(true);
        } else {
            elevatorText.gameObject.SetActive(false);
        }
        if (Vector3.Distance(hideText.transform.position, player.transform.position) < 3f) {
            hideText.gameObject.SetActive(true);
        } else {
            hideText.gameObject.SetActive(false);
        }
        if (Input.GetKeyUp (KeyCode.Z) || Input.GetKeyUp (KeyCode.Z) || Input.GetKeyUp (KeyCode.Space)) {
            playerText.gameObject.SetActive(false);
        }
    }
}
