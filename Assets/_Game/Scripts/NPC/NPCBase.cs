﻿using UnityEngine;

namespace NPC {

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(AudioSource))]
    public class NPCBase : MonoBehaviour, IInteractable
    {
        [Header("NPC Details")]
        [SerializeField]
        private string _name = string.Empty;
        [SerializeField] private int _age;
        [Tooltip("i.e. younger sister or grandmother")]
        [SerializeField]
        private string _placeInFamily = string.Empty;

        [Header("Animations")]
        [SerializeField]
        private AnimationClip _angryAnimation = null;
        [SerializeField] private AnimationClip _happyAnimation = null;
        [SerializeField] private AnimationClip _idleAnimation = null;
        [SerializeField] private AnimationClip _sadAnimation = null;
        [SerializeField] private AnimationClip _surprisedAnimation = null;

        [Header("Animation Override Controller")]
        [Header("Designers DO NOT TOUCH FOR NOW")]
        [SerializeField]
        private AnimatorOverrideController _animOverrideController;

        private Animator _animator;

        public AnimationClip IdleAnimation => _idleAnimation;
        public AnimationClip SurprisedAnimation => _surprisedAnimation;
        public AnimationClip HappyAnimation => _happyAnimation;
        public AnimationClip SadAnimation => _sadAnimation;
        public AnimationClip AngryAnimation => _angryAnimation;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        #region Interaction

        //This is when the mouse first hovers over the object.
        public void OnHoverEnter()
        {
            Debug.Log("Hovering over " + gameObject.name);
            _animator.SetTrigger("angry");
        }

        //This is when the mouse leaves the shape of the object.
        public void OnHoverExit()
        {
            Debug.Log("No Longer Hovering over " + gameObject.name);
            _animator.SetTrigger("sad");
        }

        //This is when the mouse left clicks on the object.
        public void OnLeftClick()
        {
            Debug.Log("Left Clicked On" + gameObject.name);
            _animator.SetTrigger("happy");
        }

        //This is when the mouse right clicks on an object.
        public void OnRightClick()
        {
            Debug.Log("Right Clicked On" + gameObject.name);
            _animator.SetTrigger("surprised");
        }


        #endregion

        #region Satisfying Interface

        public void OnLeftClick(Vector3 mousePoint)
        {
            
        }

        //This is when the mouse right clicks on an object.
        public void OnRightClick(Vector3 mousePoint)
        {
           
        }

        #endregion


    }
}
