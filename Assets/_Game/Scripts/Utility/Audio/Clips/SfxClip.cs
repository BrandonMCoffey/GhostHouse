﻿using UnityEngine;
using UnityEngine.Audio;
using Utility.Audio.Clips.Base;
using Utility.Audio.Helper;

namespace Utility.Audio.Clips
{
    [CreateAssetMenu(menuName = "Sound System/Sfx Clip")]
    public class SfxClip : SfxBase
    {
        [Header("Audio Clip Settings")]
        [SerializeField] private SfxReference _clip = new SfxReference(false);
        [SerializeField] private AudioMixerGroup _mixerGroup = null;
        [SerializeField] private SfxPriorityLevel _priority = SfxDefaults.Priority;

        [Header("Volume Settings")]
        [SerializeField, Range(SfxDefaults.VolumeMin, SfxDefaults.VolumeMax)]
        private float _volume = SfxDefaults.Volume;
        [SerializeField, Range(SfxDefaults.PitchMin, SfxDefaults.PitchMax)]
        private float _pitch = SfxDefaults.Pitch;
        [SerializeField, Range(SfxDefaults.StereoPanMin, SfxDefaults.StereoPanMax), Tooltip("Pans a playing sound in a stereo way (left or right). This only applies to sounds that are Mono or Stereo")]
        private float _stereoPan = SfxDefaults.StereoPan;
        [SerializeField, Range(SfxDefaults.ReverbZoneMixMin, SfxDefaults.ReverbZoneMixMax), Tooltip("The amount by which the signal from the AudioSource will be mixed into the global reverb associated with the Reverb Zones")]
        private float _reverbZoneMix = SfxDefaults.ReverbZoneMix;

        [Header("Spatial Settings")]
        [SerializeField] private bool _overrideSpatialSettings = false;
        [SerializeField, Range(SfxDefaults.SpatialBlendMin, SfxDefaults.SpatialBlendMax),
         Tooltip("Sets how much this AudioSource is affected by 3D spatialisation calculations (attenuation, doppler etc). 0.0 makes the sound full 2D, 1.0 makes it full 3D")]
        private float _spatialBlend = SfxDefaults.SpatialBlend;
        [SerializeField, Tooltip("Sets/Gets how the AudioSource attenuates over distance")]
        private AudioRolloffMode _rolloffMode = SfxDefaults.RolloffMode;
        [SerializeField, Range(SfxDefaults.MinDistanceMin, SfxDefaults.MinDistanceMax), Tooltip("Within the Min distance the AudioSource will cease to grow louder in volume")]
        private float _minDistance = SfxDefaults.MinDistance;
        [SerializeField, Range(SfxDefaults.MaxDistanceMin, SfxDefaults.MaxDistanceMax), Tooltip("(Logarithmic rolloff) MaxDistance is the distance a sound stops attenuating at")]
        private float _maxDistance = SfxDefaults.MaxDistance;
        [SerializeField, Range(SfxDefaults.SpreadMin, SfxDefaults.SpreadMax), Tooltip("Sets the spread angle (in degrees) of a 3d stereo or multichannel sound in speaker space")]
        private int _spread = SfxDefaults.Spread;
        [SerializeField, Range(SfxDefaults.DopplerLevelMin, SfxDefaults.DopplerLevelMax), Tooltip("Sets the Doppler scale for this AudioSource")]
        private float _dopplerLevel = SfxDefaults.DopplerLevel;

        /*
        [Header("Ignores")]
        [SerializeField, Tooltip("Bypass effects (Applied from filter components or global listener filters)")]
        private bool _bypassEffects = false;
        [SerializeField, Tooltip("When set global effects on the AudioListener will not be applied to the audio signal generated by the AudioSource. Does not apply if the AudioSource is playing into a mixer group")]
        private bool _bypassListenerEffects = false;
        [SerializeField, Tooltip("When set doesn't route the signal from an AudioSource into the global reverb associated with reverb zones")]
        private bool _bypassReverbZones = false;
        [SerializeField, Tooltip("This makes the audio source not take into account the volume of the audio listener")]
        private bool _ignoreListenerVolume = false;
        [SerializeField, Tooltip("Allows AudioSource to play even though AudioListener.pause is set to true. This is useful for the menu element sounds or background music in pause menus")]
        private bool _ignoreListenerPause = false;
        */

        public override SfxProperties GetSourceProperties() {
            // Ensure clip is not null or the same (prevent recursion)
            if (_clip.NullTest() || _clip.TestSame(this)) return new SfxProperties(true);

            // Find Reference Source Properties
            var referenceProperties = _clip.GetSourceProperties();

            // Create Current Source Properties
            var myProperties = new SfxProperties(_mixerGroup, (int)_priority, _volume, _pitch, _stereoPan, _reverbZoneMix, Vector3.zero, _overrideSpatialSettings, _spatialBlend, _rolloffMode, _minDistance, _maxDistance, _spread,
                _dopplerLevel);

            // Add properties together and return
            return myProperties.AddProperties(referenceProperties);
        }

        /*
        public void ApplyTo(AudioSource audioSource) {
            audioSource.outputAudioMixerGroup = _mixerGroup;
            audioSource.bypassEffects = _bypassEffects;
            audioSource.bypassListenerEffects = _bypassListenerEffects;
            audioSource.bypassReverbZones = _bypassReverbZones;
            audioSource.priority = (int)_priority;
            audioSource.volume = _volume;
            audioSource.pitch = _pitch;
            audioSource.panStereo = _stereoPan;
            audioSource.spatialBlend = _spatialBlend;
            audioSource.reverbZoneMix = _reverbZoneMix;
            audioSource.dopplerLevel = _dopplerLevel;
            audioSource.spread = _spread;
            audioSource.rolloffMode = _rolloffMode;
            audioSource.minDistance = _minDistance;
            audioSource.maxDistance = _maxDistance;
            audioSource.ignoreListenerVolume = _ignoreListenerVolume;
            audioSource.ignoreListenerPause = _ignoreListenerPause;
        }
        */
    }
}