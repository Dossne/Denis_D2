using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ClawbearGames
{
    public class EffectManager : MonoBehaviour
    {

        public static EffectManager Instance { private set; get; }

        [SerializeField] private ParticleSystem targetObjectExplodePrefab = null;
        [SerializeField] private CollectCashEffectController cashEffectControllerPrefab = null;

        private List<ParticleSystem> listTargetObjectExplode = new List<ParticleSystem>();
        private List<CollectCashEffectController> listCashEffectController = new List<CollectCashEffectController>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                DestroyImmediate(Instance.gameObject);
                Instance = this;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }


        /// <summary>
        /// Play the given particle then disable it 
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        private IEnumerator CRPlayParticle(ParticleSystem par)
        {
            par.Play();
            yield return new WaitForSeconds(2f);
            par.gameObject.SetActive(false);
        }



        /// <summary>
        /// Create a target object explode with position and hole size.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="holeSize"></param>
        /// <returns></returns>
        public void CreateTargetObjectExplode(Vector3 pos, float holeSize)
        {
            //Find in the list
            ParticleSystem targetObjectExplode = listTargetObjectExplode.Where(a => !a.gameObject.activeSelf).FirstOrDefault();

            if (targetObjectExplode == null)
            {
                //Didn't find one -> create new one
                targetObjectExplode = Instantiate(targetObjectExplodePrefab, pos, Quaternion.identity);
                targetObjectExplode.gameObject.SetActive(false);
                listTargetObjectExplode.Add(targetObjectExplode);
            }

            //Update params based on hole size

            //Adjust speed and size
            var main = targetObjectExplode.main;
            main.startSpeed = new ParticleSystem.MinMaxCurve(holeSize * 4f, Mathf.Clamp(holeSize * 10f, 5f, 30f));
            main.startSize = new ParticleSystem.MinMaxCurve(holeSize / 8f, holeSize / 6f);

            //Adjust redius
            var shape = targetObjectExplode.shape;
            shape.radius = holeSize * 0.5f;

            targetObjectExplode.transform.position = pos;
            targetObjectExplode.gameObject.SetActive(true);
            StartCoroutine(CRPlayParticle(targetObjectExplode));
        }


        /// <summary>
        /// Create a cash effect at given position and amount.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="cashAmount"></param>
        public void CreateCashEffect(Vector3 pos, int cashAmount)
        {
            CollectCashEffectController cashEffect = GetCashEffect(pos);
            cashEffect.transform.position = pos;
            cashEffect.gameObject.SetActive(true);
            cashEffect.OnInit(cashAmount);
        }

        /// <summary>
        /// Create a score effect at given position and amount.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="scoreAmount"></param>
        public void CreateScoreEffect(Vector3 pos, int scoreAmount)
        {
            CollectCashEffectController scoreEffect = GetCashEffect(pos);
            scoreEffect.transform.position = pos;
            scoreEffect.gameObject.SetActive(true);
            scoreEffect.OnInitScore(scoreAmount);
        }

        private CollectCashEffectController GetCashEffect(Vector3 pos)
        {
            CollectCashEffectController cashEffect = listCashEffectController.Where(a => !a.gameObject.activeSelf).FirstOrDefault();

            if (cashEffect == null)
            {
                //Didn't find one -> create new one
                cashEffect = Instantiate(cashEffectControllerPrefab, pos, Quaternion.identity);
                cashEffect.gameObject.SetActive(false);
                listCashEffectController.Add(cashEffect);
            }

            return cashEffect;
        }
    }
}
