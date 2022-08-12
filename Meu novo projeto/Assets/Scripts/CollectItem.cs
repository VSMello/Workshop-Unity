using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectItem : MonoBehaviour
{
    private int count = 0;
    [SerializeField] private Text countText;

    private void Start() {
        countText.text = "0";
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            count = int.Parse(countText.text);
            count++;
            countText.text = count.ToString();
            Destroy(this.gameObject);
        }
    }
}
