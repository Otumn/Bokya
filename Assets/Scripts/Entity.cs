using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Otumn.Bokya
{
    public class Entity : MonoBehaviour
    {
        #region MonoBehaviour calls

        protected virtual void Start()
        {
            
        }

        protected virtual void Update()
        {

        }

        protected virtual void OnEnable()
        {
            GameManager.instance.AddEntity(this);
        }

        protected virtual void OnDisable()
        {
            GameManager.instance.RemoveEntity(this);
        }

        #endregion

        #region Entity calls

        public virtual void OnDatasInitialized()
        {

        }

        public virtual void OnWordEntered()
        {

        }

        public virtual void OnWordRight()
        {

        }

        public virtual void OnWordWrong()
        {

        }

        public virtual void OnFlashcardAdded()
        {

        }

        public virtual void OnFlashcardDeleted()
        {

        }

        public virtual void OnChoicePopUpRequested(string title, string info, string positiveText, Action positiveAction, string negativeText, Action negativeAction)
        {
            
        }

        public virtual void OnInfoPopUpRequested(string title, string info, string button)
        {

        }

        public virtual void OnMenuChanged(MenuType type)
        {

        }

        public virtual void OnFocusOnCard(Flashcard focusedCard)
        {

        }

        public virtual void OnRequestSound(SoundRequest type, float volume)
        {

        }

        public virtual void OnRequestSound(AudioClip clip, float volume)
        {

        }

        public virtual void OnRequestShopOpen()
        {

        }

        public virtual void OnFullVersionBought()
        {

        }

        public virtual void ChangeMenuTitle(string title)
        {

        }

        #endregion 
    }
}
