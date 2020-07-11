using System;
using UnityEngine;

namespace WooshiiAttributes
    {
    /// <summary>
    /// Contain a class in a little padded toggle view box
    /// </summary>
    [AttributeUsage (AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ContainedClassAttribute : PropertyAttribute
        {
        public readonly float classPadding;
        public readonly float classSpacing;

        /// <summary>
        /// Contain a class in a small padded box
        /// </summary>
        /// <param name="classPadding">Nothing, do not use</param>
        /// <param name="classSpacing">Nothing, do not use</param>
        public ContainedClassAttribute(float classPadding, float classSpacing)
            {
            this.classPadding = classPadding;
            this.classSpacing = classSpacing;
            }

        /// <summary>
        /// Contain a class in a small padded box
        /// </summary>
        public ContainedClassAttribute()
            {
            this.classPadding = 1;
            this.classSpacing = 1;
            }
        }
    }
