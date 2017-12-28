using System;
using System.Reflection;

namespace Smart.Core.Domain.Base
{

    public abstract partial class BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        public string BusinessEntityId { get; set; }
       


        public override bool Equals(object obj)
        {
            return Equals(obj as BaseEntity);
        }
        private static bool IsTransient(BaseEntity obj)
        {
            return obj != null && Equals(obj.BusinessEntityId, default(string));
        }
        private Type GetUnproxiedType()
        {
            return GetType();
        }
        public virtual bool Equals(BaseEntity other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;


            if (!IsTransient(this) &&
                !IsTransient(other) &&
                Equals(BusinessEntityId, other.BusinessEntityId))
            {
                var otherType = other.GetUnproxiedType();
                var thisType = GetUnproxiedType();
                return thisType.IsAssignableFrom(otherType) ||
                        otherType.IsAssignableFrom(thisType);
            }

            return false;
        }
        public override int GetHashCode()
        {
            if (Equals(BusinessEntityId, default(int)))
                return base.GetHashCode();
            return BusinessEntityId.GetHashCode();
        }
        public static bool operator ==(BaseEntity x, BaseEntity y)
        {
            return Equals(x, y);
        }
        public static bool operator !=(BaseEntity x, BaseEntity y)
        {
            return !(x == y);
        }
    }
}
