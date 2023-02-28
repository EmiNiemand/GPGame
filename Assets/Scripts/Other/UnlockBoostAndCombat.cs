using UnityEngine;

namespace Other
{
    public class UnlockBoostAndCombat : MonoBehaviour, IUsable
    {
        // Sticks used in animation
        [SerializeField] private GameObject[] sticks;
        private GameObject pickUpIndicator;

        private void Start() 
        {
            pickUpIndicator = transform.Find("PickUpIndicator").gameObject;
            foreach(var stick in sticks)
            {
                stick.GetComponent<SpriteRenderer>().enabled = false;
            }
        }

        public void OnEnter(GameObject user)
        {
            pickUpIndicator.SetActive(true);
        }

        public void OnExit(GameObject user)
        {
            pickUpIndicator.SetActive(false);
        }

        public void Use(GameObject user)
        {
            user.GetComponent<Player.PlayerCombat>().UnlockCombat();
            user.GetComponent<Player.PlayerMovement>().UnlockBoost();
            foreach(var stick in sticks)
            {
                stick.GetComponent<SpriteRenderer>().enabled = true;
            }
            Destroy(gameObject);
        }
    }
}
