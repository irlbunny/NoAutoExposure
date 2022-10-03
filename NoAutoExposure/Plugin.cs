using IPA;
using IPA.Utilities;
using System.Collections.Generic;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

namespace NoAutoExposure
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        private static readonly FieldAccessor<BloomPrePassBloomTextureEffectSO, PyramidBloomRendererSO.Pass>.Accessor _finalUpsamplePassAccessor
            = FieldAccessor<BloomPrePassBloomTextureEffectSO, PyramidBloomRendererSO.Pass>.GetAccessor("_finalUpsamplePass");

        internal static IPALogger Log { get; private set; }

        internal static Dictionary<BloomPrePassBloomTextureEffectSO, PyramidBloomRendererSO.Pass> FinalUpsamplePasses { get; private set; }

        [Init]
        public Plugin(IPALogger logger)
        {
            Log = logger;
        }

        [OnEnable]
        public void OnEnable()
        {
            if (FinalUpsamplePasses == null)
            {
                FinalUpsamplePasses = new Dictionary<BloomPrePassBloomTextureEffectSO, PyramidBloomRendererSO.Pass>();

                var textureEffects = Resources.FindObjectsOfTypeAll<BloomPrePassBloomTextureEffectSO>();
                for (var i = 0; i < textureEffects.Length; i++)
                {
                    var textureEffect = textureEffects[i];
                    var finalUpsamplePass = _finalUpsamplePassAccessor(ref textureEffect);
                    if (finalUpsamplePass == PyramidBloomRendererSO.Pass.UpsampleTentAndACESToneMappingGlobalIntensity)
                    {
                        FinalUpsamplePasses[textureEffect] = finalUpsamplePass;
                        _finalUpsamplePassAccessor(ref textureEffect) = PyramidBloomRendererSO.Pass.UpsampleTentAndACESToneMapping;
                        Log.Info($"Patched {textureEffect.name}.");
                    }
                }
            }
        }

        [OnDisable]
        public void OnDisable()
        {
            if (FinalUpsamplePasses != null)
            {
                foreach (var finalUpsamplePass in FinalUpsamplePasses)
                {
                    var textureEffect = finalUpsamplePass.Key;
                    _finalUpsamplePassAccessor(ref textureEffect) = finalUpsamplePass.Value;
                    Log.Info($"Unpatched {textureEffect.name}.");
                }
            }

            FinalUpsamplePasses = null;
        }
    }
}
