﻿using System.Collections;
using UnityEngine;
using Utility.Audio.Clips;
using Utility.Audio.Controllers.Base;
using Utility.Audio.Managers;

namespace Utility.Audio.Controllers
{
    public class MusicSourceController : ASC_Base
    {
        private MusicTrack _current;
        private bool _routineActive;

        public void ResetSource() {
            Source.outputAudioMixerGroup = SoundManager.MusicGroup;
        }

        public float PlayMusic(MusicTrack track, float delay) {
            _current = track;
            SetSourceProperties(track.GetSourceProperties());
            StartCoroutine(FadeRoutine(track.FadeInTime, true, delay));
            Play();
            return Time.time + track.TrackLength - track.FadeOutTime;
        }

        /*
        public void PlayMusic(MusicTrack track, float time) {
            if (time < 0 || time >= track.TrackLength) return;
            _current = track;
            _active = true;
            SetSourceProperties(track.GetSourceProperties());
            if (time < track.FadeInTime) {
                StartCoroutine(FadeRoutine(track.FadeInTime, true, time));
            }
            float offsetFromEnd = track.TrackLength - time;
            if (track.FadeOutTime > offsetFromEnd) {
                StartCoroutine(FadeRoutine(track.FadeOutTime, false, track.FadeOutTime - offsetFromEnd));
                _active = false;
            }
            else {
                _timeToStop = Time.time + track.TrackLength - time - Mathf.Max(track.FadeOutTime, track.DelayNextSong);
            }
            Play();
        }
        */

        public void StopMusic() {
            StartCoroutine(FadeRoutine(_current.FadeOutTime, false));
        }

        private IEnumerator FadeRoutine(float fadeLength, bool fadeIn, float delay = 0, float startTime = 0) {
            if (_routineActive) {
                yield return null;
            }
            for (float t = 0; t < delay; t += Time.deltaTime) {
                yield return null;
            }
            _routineActive = true;
            for (float t = startTime; t < fadeLength; t += Time.time) {
                float delta = t / fadeLength;
                SetCustomVolume(_current.Evaluate(delta, fadeIn));
                yield return null;
            }
            _routineActive = false;
            if (!fadeIn) {
                Stop();
            }
        }

        public override void Stop() {
            base.Stop();
            SoundManager.MusicManager.ReturnController(this);
        }
    }
}