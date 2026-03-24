using System.IO;
using System.Collections;
using UnityEngine;

namespace ClawbearGames
{
    public class ShareManager : MonoBehaviour
    {
        [Header("Native Share Configuration")]
        [SerializeField] private string shareText = "Can you beat my score!!!";
        [SerializeField] private string shareSubject = "Share With";
        [SerializeField] private string appUrl = "https://play.google.com/store/apps/details?id=com.BrightFuture.KnifeShooter";

        public string AppUrl { get { return appUrl; } }
        public string ScreenshotFilePath { get { return Path.Combine(Application.temporaryCachePath, "Screenshot.png"); } }


        /// <summary>
        /// Create the screenshot
        /// </summary>
        public void CreateScreenshot()
        {
            StartCoroutine(CRTakeScreenshot());
        }

        /// <summary>
        /// Coroutine take a screenshot.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRTakeScreenshot()
        {
            Texture2D screenshot2D = null;
            yield return new WaitForEndOfFrame();
            screenshot2D = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            screenshot2D.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenshot2D.Apply();
            File.WriteAllBytes(ScreenshotFilePath, screenshot2D.EncodeToPNG());
            Destroy(screenshot2D);
        }


        /// <summary>
        /// Share screenshot with text
        /// </summary>
        public void NativeShare()
        {
            new NativeShare().AddFile(ScreenshotFilePath).SetSubject(shareSubject).SetText(shareText + " " + AppUrl).Share();
        }
    }

}