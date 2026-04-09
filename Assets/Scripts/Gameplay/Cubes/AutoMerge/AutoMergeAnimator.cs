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
        private const int TrailParticlesTotal = 200;

        public async UniTask SpiralMergeAsync(
            CubeView cubeA, CubeView cubeB,
            Vector3 mergePoint, float spreadOffset,
            float duration, MergeVFXPlayer vfxPlayer, int po2Value,
            CancellationToken ct)
        {
            var startA = cubeA.transform.position;
            var startB = cubeB.transform.position;
            var midpoint = (startA + startB) * 0.5f;

            var startRadiusA = Vector3.Distance(
                new Vector3(startA.x, 0f, startA.z),
                new Vector3(midpoint.x, 0f, midpoint.z));
            var startRadiusB = Vector3.Distance(
                new Vector3(startB.x, 0f, startB.z),
                new Vector3(midpoint.x, 0f, midpoint.z));

            var maxRadiusA = startRadiusA + spreadOffset;
            var maxRadiusB = startRadiusB + spreadOffset;

            var angleA = Mathf.Atan2(startA.x - midpoint.x, startA.z - midpoint.z);
            var angleB = Mathf.Atan2(startB.x - midpoint.x, startB.z - midpoint.z);

            var totalAngle = Revolutions * Mathf.PI * 2f;

            var emitInterval = 1f / TrailParticlesTotal;
            var nextEmitProgress = emitInterval;

            var tween = DOVirtual.Float(0f, 1f, duration, progress =>
                {
                    var height = Mathf.Lerp(startA.y, mergePoint.y, EaseOutSine(progress));

                    if (progress < 0.5f)
                    {
                        var radiusT = progress * 2f;
                        var curRadiusA = Mathf.Lerp(startRadiusA, maxRadiusA, radiusT);
                        var curRadiusB = Mathf.Lerp(startRadiusB, maxRadiusB, radiusT);
                        SetPositions(cubeA, cubeB, midpoint, height,
                            angleA + totalAngle * progress, curRadiusA,
                            angleB + totalAngle * progress, curRadiusB);
                    }
                    else
                    {
                        var radiusT = (progress - 0.5f) * 2f;
                        var curRadiusA = Mathf.Lerp(maxRadiusA, 0f, radiusT);
                        var curRadiusB = Mathf.Lerp(maxRadiusB, 0f, radiusT);
                        SetPositions(cubeA, cubeB, midpoint, height,
                            angleA + totalAngle * progress, curRadiusA,
                            angleB + totalAngle * progress, curRadiusB);
                    }

                    cubeA.transform.rotation = Quaternion.identity;
                    cubeB.transform.rotation = Quaternion.identity;

                    if (progress >= nextEmitProgress)
                    {
                        vfxPlayer.EmitAt(cubeA.transform.position, po2Value, 1);
                        vfxPlayer.EmitAt(cubeB.transform.position, po2Value, 1);
                        nextEmitProgress += emitInterval;
                    }
                })
                .SetEase(Ease.Linear);

            await AwaitTween(tween, ct);
        }

        public async UniTask ScalePulseAsync(CubeView cube, float duration, CancellationToken ct)
        {
            var originalScale = cube.transform.localScale;

            var sequence = DOTween.Sequence()
                .Append(cube.transform.DOScale(originalScale * 1.3f, duration * 0.5f).SetEase(Ease.OutQuad))
                .Append(cube.transform.DOScale(originalScale, duration * 0.5f).SetEase(Ease.InQuad));

            await AwaitTween(sequence, ct);
        }

        private static void SetPositions(
            CubeView cubeA, CubeView cubeB,
            Vector3 center, float height,
            float angA, float radA,
            float angB, float radB)
        {
            cubeA.transform.position = new Vector3(
                center.x + Mathf.Sin(angA) * radA,
                height,
                center.z + Mathf.Cos(angA) * radA);

            cubeB.transform.position = new Vector3(
                center.x + Mathf.Sin(angB) * radB,
                height,
                center.z + Mathf.Cos(angB) * radB);
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
    }
}
