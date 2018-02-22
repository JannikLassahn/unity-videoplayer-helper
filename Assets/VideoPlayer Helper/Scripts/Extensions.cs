using System;
using UnityEngine;

namespace Unity.VideoHelper
{
    internal static class Extensions
    {
        public static TComponent GetOrAddComponent<TComponent>(this GameObject target) where TComponent : Component
        {
            if (target == null)
                throw new ArgumentNullException("Target gameobject must not be null.");

            var comp = target.GetComponent<TComponent>();
            if (comp == null)
                comp = target.AddComponent<TComponent>();

            return comp;
        }
    }
}
