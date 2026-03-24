using System.Collections;
using UnityEngine;

namespace ClawbearGames
{
    public class CharacterInforController : MonoBehaviour
    {
        [Header("The name of this character. This field must be unique.")]
        [SerializeField] private string characterName = string.Empty;

        [Header("Price of the character.")]
        [SerializeField] private int characterPrice = 0;

        [Header("The material's color of this character when it locked.")]
        [SerializeField] private Color lockedColor = Color.black;

        [Header("The material's color of this character when it unlocked.")]
        [SerializeField] private Color unlockedColor = Color.white;

        [Header("Character References")]
        [SerializeField] private SpriteRenderer spriteRenderer = null;


        /// <summary>
        /// The material of this character.
        /// </summary>
        public Sprite HoleSprite => spriteRenderer.sprite;



        /// <summary>
        /// The name of this character.
        /// </summary>
        public string CharacterName => characterName;


        /// <summary>
        /// The sequence number of this character in CharacterContainer
        /// </summary>
        public int SequenceNumber { private set; get; }


        /// <summary>
        /// The price of this character.
        /// </summary>
        public int CharacterPrice => characterPrice;


        /// <summary>
        /// Is this character unlocked or not.
        /// </summary>
        public bool IsUnlocked => PlayerDataHandler.IsUnlockedCharacter(characterName);


        private ParticleSystem fireEffects = null;
        private void Awake()
        {
            //Get the fire effects
            if (fireEffects == null) { fireEffects = transform.GetChild(1).GetComponent<ParticleSystem>(); }

            //Set the sprite renderer's colors for this character and update the fire effects
            if (IsUnlocked)
            {
                spriteRenderer.color = unlockedColor;
                fireEffects.gameObject.SetActive(true);
                fireEffects.Play();
            }
            else
            {
                spriteRenderer.color = lockedColor;
                fireEffects.gameObject.SetActive(false);
            }
        }


        /// <summary>
        /// Set the sequence number of this character
        /// </summary>
        /// <param name="number"></param>
        public void SetSequenceNumber(int number)
        {
            SequenceNumber = number;
        }


        /// <summary>
        /// Unlock this character.
        /// </summary>
        /// <returns></returns>
        public void Unlock()
        {
            PlayerDataHandler.UpdateUnlockedCharacter(characterName);
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Unlock);
            ServicesManager.Instance.CoinManager.RemoveTotalCoins(characterPrice, 0.15f);

            //Set the sprite renderer's colors for this character and update the fire effects
            spriteRenderer.color = unlockedColor;
            fireEffects.gameObject.SetActive(true);
            fireEffects.Play();
        }



        /// <summary>
        /// Rotate this CharacterInforController object
        /// </summary>
        /// <param name="speed"></param>
        public void RotateThisCharacter(float speed)
        {
            StartCoroutine(CRRotate(speed));
        }


        /// <summary>
        /// Coroutine rotate this camera.
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        private IEnumerator CRRotate(float speed)
        {
            while (true)
            {
                transform.eulerAngles += Vector3.up * speed * Time.deltaTime;
                yield return null;
            }
        }



        /// <summary>
        /// Reset the rotation of this character.
        /// </summary>
        public void ResetRotation(float speed)
        {
            StopAllCoroutines();
            StartCoroutine(CRResetRotation(speed));
        }


        /// <summary>
        /// Coroutine reset rotation.
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        private IEnumerator CRResetRotation(float speed)
        {
            Vector3 startRotation = transform.rotation.eulerAngles;
            Vector3 endRotation = Vector3.zero;
            float timePast = 0;
            float rotateAngle = Mathf.Abs(endRotation.y - startRotation.y);
            float rotateTime = rotateAngle / speed;

            while (timePast < rotateTime)
            {
                timePast += Time.deltaTime;
                Vector3 rotation = Vector3.Lerp(startRotation, endRotation, timePast / rotateTime);
                transform.rotation = Quaternion.Euler(rotation);
                yield return null;
            }
        }


        /// <summary>
        /// Move this character by given vector.
        /// </summary>
        /// <param name="dir"></param>
        public void MoveByVector(Vector3 dir)
        {
            transform.position += dir;
        }
    }
}
