using UnityEngine;

namespace ClawbearGames
{
    public static class HoleVisualUtility
    {
        public const float BlackApertureDiameterRatio = 0.60f;

        private static readonly string[] holeEffectNames = { "Sparks", "FireRising", "Smoke" };
        private static Sprite cachedReferenceHoleSprite = null;

        public static Sprite GetReferenceHoleSprite()
        {
            if (cachedReferenceHoleSprite != null)
            {
                return cachedReferenceHoleSprite;
            }

            const int textureSize = 256;
            const float pixelsPerUnit = 100f;
            const float antiAlias = 1.5f;

            Color transparent = new Color(0f, 0f, 0f, 0f);
            Color holeBlack = new Color(0f, 0f, 0f, 1f);
            Color innerRingGreen = new Color(0.78f, 0.93f, 0.50f, 1f);
            Color outerRingGreen = new Color(0.34f, 0.82f, 0.23f, 1f);

            Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, false);
            texture.name = "Hole_Reference_Runtime";
            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;

            Color[] pixels = new Color[textureSize * textureSize];
            float center = (textureSize - 1f) * 0.5f;
            float outerRadius = center - 2f;
            float innerRingOuterRadius = outerRadius * 0.70f;
            float holeRadius = outerRadius * 0.60f;

            for (int y = 0; y < textureSize; y++)
            {
                for (int x = 0; x < textureSize; x++)
                {
                    float dx = x - center;
                    float dy = y - center;
                    float distance = Mathf.Sqrt(dx * dx + dy * dy);

                    Color pixelColor = transparent;

                    if (distance <= holeRadius)
                    {
                        pixelColor = holeBlack;
                    }
                    else if (distance <= innerRingOuterRadius)
                    {
                        pixelColor = innerRingGreen;
                    }
                    else if (distance <= outerRadius)
                    {
                        pixelColor = outerRingGreen;
                    }

                    if (distance > holeRadius && distance < (holeRadius + antiAlias))
                    {
                        float blend = Mathf.InverseLerp(holeRadius, holeRadius + antiAlias, distance);
                        pixelColor = Color.Lerp(holeBlack, pixelColor, blend);
                    }

                    if (distance > (outerRadius - antiAlias) && distance <= outerRadius)
                    {
                        float alpha = Mathf.InverseLerp(outerRadius, outerRadius - antiAlias, distance);
                        pixelColor.a *= alpha;
                    }

                    pixels[(y * textureSize) + x] = pixelColor;
                }
            }

            texture.SetPixels(pixels);
            texture.Apply(false, false);

            cachedReferenceHoleSprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, textureSize, textureSize),
                new Vector2(0.5f, 0.5f),
                pixelsPerUnit);
            cachedReferenceHoleSprite.name = "Hole_Reference_Sprite_Runtime";

            return cachedReferenceHoleSprite;
        }

        public static void ApplyReferenceSprite(SpriteRenderer holeSpriteRenderer)
        {
            if (holeSpriteRenderer == null)
            {
                return;
            }

            holeSpriteRenderer.sprite = GetReferenceHoleSprite();
            holeSpriteRenderer.color = Color.white;

            // Keep the rendering plane height but align sprite center with hole physics center.
            Transform spriteTransform = holeSpriteRenderer.transform;
            Vector3 localPosition = spriteTransform.localPosition;
            spriteTransform.localPosition = new Vector3(0f, localPosition.y, 0f);
        }

        /// <summary>
        /// Get black-aperture diameter in world units from the hole sprite bounds.
        /// </summary>
        /// <param name="holeSpriteRenderer"></param>
        /// <param name="fallbackOuterDiameter"></param>
        /// <returns></returns>
        public static float GetBlackApertureDiameterWorld(SpriteRenderer holeSpriteRenderer, float fallbackOuterDiameter)
        {
            if (holeSpriteRenderer != null)
            {
                float outerDiameter = holeSpriteRenderer.bounds.size.x;
                if (outerDiameter > Mathf.Epsilon)
                {
                    return outerDiameter * BlackApertureDiameterRatio;
                }
            }

            return Mathf.Max(fallbackOuterDiameter * BlackApertureDiameterRatio, Mathf.Epsilon);
        }

        public static void ApplyReferenceVisual(Transform root, SpriteRenderer holeSpriteRenderer = null)
        {
            if (holeSpriteRenderer == null && root != null)
            {
                holeSpriteRenderer = FindHoleSpriteRenderer(root);
            }

            ApplyReferenceSprite(holeSpriteRenderer);
            DisableHoleEffects(root);
        }

        public static void DisableHoleEffectsInScene()
        {
            ParticleSystem[] particles = Object.FindObjectsOfType<ParticleSystem>(true);
            for (int i = 0; i < particles.Length; i++)
            {
                ParticleSystem particle = particles[i];
                if (particle != null && IsHoleEffectName(particle.gameObject.name))
                {
                    DisableParticleHierarchy(particle.transform);
                }
            }
        }

        public static void DisableHoleEffects(Transform root, ParticleSystem[] holeEffects = null, Transform fireEffectRoot = null)
        {
            if (holeEffects != null)
            {
                for (int i = 0; i < holeEffects.Length; i++)
                {
                    ParticleSystem particle = holeEffects[i];
                    if (particle == null)
                    {
                        continue;
                    }

                    particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    particle.Clear(true);
                    particle.gameObject.SetActive(false);
                }
            }

            if (fireEffectRoot != null)
            {
                DisableParticleHierarchy(fireEffectRoot);
            }

            if (root == null)
            {
                return;
            }

            Transform[] transforms = root.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < transforms.Length; i++)
            {
                Transform tr = transforms[i];
                if (tr != null && IsHoleEffectName(tr.gameObject.name))
                {
                    DisableParticleHierarchy(tr);
                }
            }
        }

        public static SpriteRenderer FindHoleSpriteRenderer(Transform root)
        {
            if (root == null)
            {
                return null;
            }

            SpriteRenderer[] spriteRenderers = root.GetComponentsInChildren<SpriteRenderer>(true);
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                SpriteRenderer spriteRenderer = spriteRenderers[i];
                if (spriteRenderer != null && spriteRenderer.gameObject.name == "HoleSprite")
                {
                    return spriteRenderer;
                }
            }

            return null;
        }

        private static bool IsHoleEffectName(string name)
        {
            for (int i = 0; i < holeEffectNames.Length; i++)
            {
                if (name == holeEffectNames[i])
                {
                    return true;
                }
            }

            return false;
        }

        private static void DisableParticleHierarchy(Transform root)
        {
            if (root == null)
            {
                return;
            }

            ParticleSystem[] particles = root.GetComponentsInChildren<ParticleSystem>(true);
            for (int i = 0; i < particles.Length; i++)
            {
                ParticleSystem particle = particles[i];
                if (particle == null)
                {
                    continue;
                }

                particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                particle.Clear(true);
            }

            root.gameObject.SetActive(false);
        }
    }
}
