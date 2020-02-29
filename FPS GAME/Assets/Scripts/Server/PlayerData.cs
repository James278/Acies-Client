using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace GameServer
{
    public class PlayerData : MonoBehaviour
    {
        public Vector3 position = Vector3.zero;
        public Quaternion rotation = Quaternion.identity;

        public static GameObject playerPrefab;

        public float speed = 5f;
        public float jump_Force = 10f;
        private CharacterController character_Controller;
        private Vector3 distance;
        private float gravity = 20f;
        private float vertical_Velocity;
        public int id = 0;
        public Client client;
        MouseLook_FPS mouseLook;

        public bool localPlayer = false;

        public int pingIndex;

        private void Awake()
        {
            client = FindObjectOfType<Client>();
            mouseLook = GetComponentInChildren<MouseLook_FPS>();
            character_Controller = GetComponent<CharacterController>();
            playerPrefab = Resources.Load<GameObject>("Prefabs/PlayerVariants/PlayerAvatar");
        }

        public void SetName()
        {
            GetComponentInChildren<TextMesh>().text = $"Player: {id}";
        }

        public void MoveThePlayer()
        {
            float _Hor = Input.GetAxis("Horizontal");
            float _Vert = Input.GetAxis("Vertical");
            Quaternion _Rotation = transform.rotation;

            Vector3 moveDirectionForward = transform.forward * _Vert;
            Vector3 moveDirectionSide = transform.right * _Hor;

            Vector3 direction = (moveDirectionForward + moveDirectionSide).normalized;
            distance = direction * speed * Time.deltaTime;

            character_Controller.Move(distance);
            character_Controller.Move(distance);
            distance = Vector3.zero;
        }

        private void FixedUpdate()
        {
            //Appls movement to the server if the localplayer is moving
            if (localPlayer)
            {
                MoveThePlayer();
                ApplyGravity();
                ClientSend.SendMovement(transform.position, transform.rotation);
            }
        }

        public void ApplyGravity()
        {
            // If we are on the ground.
            if (character_Controller.isGrounded)
            {
                // Calculating gravity and allow to jump.
                vertical_Velocity -= gravity * Time.deltaTime;
            }
            else
            {
                // Calculating gravity.
                vertical_Velocity -= gravity * Time.deltaTime;
            }

            // Applying gravity.
            distance.y = vertical_Velocity * Time.deltaTime;
        }

        public void PlayerJump()
        {
            // If we are on the ground and press the space key.
            if (character_Controller.isGrounded && Input.GetKeyDown("Space"))
            {
                // Calculate jump force.
                vertical_Velocity = jump_Force;
            }
        }
    }
}
