using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game2048.Gameplay.Cubes.Merge;
using UnityEngine;

namespace Game2048.Gameplay.Cubes.AutoMerge
{
    public class AutoMergeAnimator
    {
        private const float Revolutions = 2f;
        private const int TrailParticlesTotal = 400;
        private const float PulseScale = 1.3f;

        public async UniTask SpiralMergeAsync(
            CubeView cubeA, CubeView cubeB,
            Vector3 mergePoint, float spreadOffset,
            float duration, MergeVFXPlayer vfxPlayer, int po2Value,
            CancellationToken ct)
        {
            var config = BuildSpiralConfig(cubeA, cubeB, mergePoint, spreadOffset);
            var emitInterval = 1f / TrailParticlesTotal;
            var nextEmit = emitInterval;

            var tween = DOVirtual.Float(0f, 1f, duration, progress =>
                {
                    UpdateSpiralPositions(cubeA, cubeB, config, progress, mergePoint.y);
                    ResetRotations(cubeA, cubeB);
                    nextEmit = TryEmitParticles(cubeA, cubeB, vfxPlayer, po2Value, progress, nextEmit, emitInterval);
                })
                .SetEase(Ease.Linear);

            await AwaitTween(tween, ct);
        }

        public async UniTask ScalePulseAsync(CubeView cube, float duration, CancellationToken ct)
        {
            var originalScale = cube.transform.localScale;

            var sequence = DOTween.Sequence()
                .Append(cube.transform.DOScale(originalScale * PulseScale, duration * 0.5f).SetEase(Ease.OutQuad))
                .Append(cube.transform.DOScale(originalScale, duration * 0.5f).SetEase(Ease.InQuad));

            await AwaitTween(sequence, ct);
        }

        private static SpiralConfig BuildSpiralConfig(
            CubeView cubeA, CubeView cubeB,
            Vector3 mergePoint, float spreadOffset)
        {
            var startA = cubeA.transform.position;
            var startB = cubeB.transform.position;
            var midpoint = (startA + startB) * 0.5f;

            var startRadiusA = HorizontalDistance(startA, midpoint);
            var startRadiusB = HorizontalDistance(startB, midpoint);

            return new SpiralConfig
            {
                Midpoint = midpoint,
                StartHeight = startA.y,
                StartRadiusA = startRadiusA,
                StartRadiusB = startRadiusB,
                MaxRadiusA = startRadiusA + spreadOffset,
                MaxRadiusB = startRadiusB + spreadOffset,
                AngleA = Mathf.Atan2(startA.x - midpoint.x, startA.z - midpoint.z),
                AngleB = Mathf.Atan2(startB.x - midpoint.x, startB.z - midpoint.z),
                TotalAngle = Revolutions * Mathf.PI * 2f
            };
        }

        private static void UpdateSpiralPositions(
            CubeView cubeA, CubeView cubeB,
            SpiralConfig config, float progress, float targetHeight)
        {
            var height = Mathf.Lerp(config.StartHeight, targetHeight, EaseOutSine(progress));
            var currentAngleA = config.AngleA + config.TotalAngle * progress;
            var currentAngleB = config.AngleB + config.TotalAngle * progress;

            float radiusA, radiusB;

            if (progress < 0.5f)
            {
                var t = progress * 2f;
                radiusA = Mathf.Lerp(config.StartRadiusA, config.MaxRadiusA, t);
                radiusB = Mathf.Lerp(config.StartRadiusB, config.MaxRadiusB, t);
            }
            else
            {
                var t = (progress - 0.5f) * 2f;
                radiusA = Mathf.Lerp(config.MaxRadiusA, 0f, t);
                radiusB = Mathf.Lerp(config.MaxRadiusB, 0f, t);
            }

            SetPositionOnCircle(cubeA, config.Midpoint, height, currentAngleA, radiusA);
            SetPositionOnCircle(cubeB, config.Midpoint, height, currentAngleB, radiusB);
        }

        private static void SetPositionOnCircle(
            CubeView cube, Vector3 center,
            float height, float angle, float radius)
        {
            cube.transform.position = new Vector3(
                center.x + Mathf.Sin(angle) * radius,
                height,
                center.z + Mathf.Cos(angle) * radius);
        }

        private static void ResetRotations(CubeView cubeA, CubeView cubeB)
        {
            cubeA.transform.rotation = Quaternion.identity;
            cubeB.transform.rotation = Quaternion.identity;
        }

        private static float TryEmitParticles(
            CubeView cubeA, CubeView cubeB,
            MergeVFXPlayer vfxPlayer, int po2Value,
            float progress, float nextEmit, float interval)
        {
            if (progress < nextEmit)
                return nextEmit;

            vfxPlayer.EmitAt(cubeA.transform.position, po2Value, 1);
            vfxPlayer.EmitAt(cubeB.transform.position, po2Value, 1);

            return nextEmit + interval;
        }

        private static float HorizontalDistance(Vector3 a, Vector3 b)
        {
            return Vector3.Distance(
                new Vector3(a.x, 0f, a.z),
                new Vector3(b.x, 0f, b.z));
        }

        private static float EaseOutSine(float t)
        {
            return Mathf.Sin(t * Mathf.PI * 0.5f);
        }

        private static async UniTask AwaitTween(Tween tween, CancellationToken ct)
        {
            while (tween.IsActive() && !tween.IsComplete())
            {
                ct.ThrowIfCancellationRequested();
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
        }

        private struct SpiralConfig
        {
            public Vector3 Midpoint;
            public float StartHeight;
            public float StartRadiusA;
            public float StartRadiusB;
            public float MaxRadiusA;
            public float MaxRadiusB;
            public float AngleA;
            public float AngleB;
            public float TotalAngle;
        }
    }
}
