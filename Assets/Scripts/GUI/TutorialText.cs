using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialText : MonoBehaviour
{
    public GameObject treeText;
    public GameObject golemText;
    public GameObject elevatorText;
    public GameObject playerText;
    public GameObject player;
    public GetHealth golem;
    private CharacterInputController cinput;
    // Start is called before the first frame update
    void Start()
    {
        treeText.gameObject.SetActive(false);
        golemText.gameObject.SetActive(false);
        elevatorText.gameObject.SetActive(false);
        playerText.gameObject.SetActive(true);
        cinput = player.GetComponent<CharacterInputController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(treeText.transform.position, player.transform.position) < 3f) {
            treeText.gameObject.SetActive(true);
        } else {
            treeText.gameObject.SetActive(false);
        }
        if (golem.hp > 0f && Vector3.Distance(golemText.transform.position, player.transform.position) < 3f) {
            golemText.gameObject.SetActive(true);
        } else {
            golemText.gameObject.SetActive(false);
        }
        if (Vector3.Distance(elevatorText.transform.position, player.transform.position) < 3f) {
            elevatorText.gameObject.SetActive(true);
        } else {
            elevatorText.gameObject.SetActive(false);
        }
        if (cinput.Jump || cinput.Forward != 0f || cinput.Turn != 0f) {
            playerText.gameObject.SetActive(false);
        }
    }
}
