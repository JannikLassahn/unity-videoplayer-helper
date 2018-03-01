using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Unity.VideoHelper
{
    internal static class Extensions
    {
        /// <summary>
        /// Gets an existing component or adds one to the game object.
        /// </summary>
        /// <typeparam name="TComponent">The type of the component.</typeparam>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static TComponent GetOrAddComponent<TComponent>(this GameObject target) where TComponent : Component
        {
            if (target == null)
                throw new ArgumentNullException("Target gameobject must not be null.");

            var comp = target.GetComponent<TComponent>();
            if (comp == null)
                comp = target.AddComponent<TComponent>();

            return comp;
        }

        /// <summary>
        /// Sets the <see cref="GameObject.active"/> property of a components gameobject. 
        /// Works also if the component is null.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="isActive"></param>
        public static void SetGameObjectActive(this Component component, bool isActive)
        {
            // we should throw an exception here, but we want to keep the syntax short 
            // and the null-conditional operator (myVariable?.MyMethod()) is not yet supported in Untiy...
            if (component == null)
                return;

            component.gameObject.SetActive(isActive);
        }

        public static void OnClick(this Component component, UnityAction action)
        {
            if (component == null || action == null)
                return;

            var button = component.gameObject.GetComponentInParent<Button>();
            if (button != null)
                button.onClick.AddListener(action);
            else
                component.gameObject.GetOrAddComponent<ClickRouter>().OnClick.AddListener(action);
        }

        public static void OnDoubleClick(this Component component, UnityAction action)
        {
            var router = component.gameObject.GetOrAddComponent<ClickRouter>();
            router.OnDoubleClick.AddListener(action);
        }
    }
}
