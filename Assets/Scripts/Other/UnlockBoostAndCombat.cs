using UnityEngine;

namespace Other
{
    public class UnlockBoostAndCombat : MonoBehaviour, IUsable
    {
        private GameObject pickUpIndicator;

        private void Start() 
        {
            pickUpIndicator = transform.Find("PickUpIndicator").gameObject;
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
            Destroy(gameObject);
        }
    }
}
