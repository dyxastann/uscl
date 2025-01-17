using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private int count;
    private float movementX;
    private float movementY;
    // Start is called before the first frame update
    [SerializeField] float speed = 10;
    [SerializeField] float jump = 5;
    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    [SerializeField] Transform cam;
    InputAction moveAction;

    public Animator animator;

    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        winTextObject.SetActive(false);
        moveAction = InputSystem.actions.FindAction("Move");
    }

    private void Update() 
    {
        Vector2 movementVector = moveAction.ReadValue<Vector2>(); 
        movementX = movementVector.x; 
        movementY = movementVector.y; 

        Vector3 camForward = cam.forward;
        Vector3 camUp = cam.up;
        Vector3 camRight = cam.right;

        camForward.y = 0;
        camRight.y = 0;

        Vector3 forwardRelative = movementY * new Vector3(camForward.x + camUp.x, 0f, camForward.z + camUp.z);
        Vector3 rightRelative = movementX * camRight;

        Vector3 movement = forwardRelative + rightRelative;

        rb.velocity = new Vector3(movement.x * speed, rb.velocity.y, movement.z * speed);
        if(Input.GetButtonDown("Jump") && Physics.Raycast(transform.position, Vector3.down, 0.65f)) {
            animator.SetTrigger("Jump");
            rb.velocity = new Vector3(rb.velocity.x, jump, rb.velocity.z);
        }
        if(!Mathf.Approximately(rb.velocity.x, 0) && !Mathf.Approximately(rb.velocity.z, 0)) transform.forward = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        
        // animator.SetFloat("Speed", rb.velocity.magnitude);
    }

    void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.CompareTag("PickUp")) 
        {
            other.gameObject.SetActive(false);
            count = count + 1;
            SetCountText();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Destroy the current object
            Destroy(gameObject); 
            // Update the winText to display "You Lose!"
            winTextObject.gameObject.SetActive(true);
            winTextObject.GetComponent<TextMeshProUGUI>().text = "You Lose!";
        }
    }

    private void OnCollisionStay(Collision other) {
        if(other.gameObject.CompareTag("Grass")) speed = 10f;
        if(other.gameObject.CompareTag("Dirt")) speed = 7.5f;
        if(other.gameObject.CompareTag("Swamp")) speed = 5f;
    }

    void SetCountText() 
    {
        countText.text =  "Count: " + count.ToString();
        if (count >= 8)
        {
            winTextObject.SetActive(true);
            Destroy(GameObject.FindGameObjectWithTag("Enemy"));
        }
    }
}
