using System;
using System.Text.Json.Serialization;
using TestSysplan.Core.Util;

namespace TestSysplan.Core.Model
{
    public abstract class ModelBase : IEquatable<ModelBase>, IComparable<ModelBase>, IComparable
    {
        [JsonIgnore]
        public long Id { get; set; }

        public Guid Uuid { get; set; }

        #region IEquatable
        public sealed override int GetHashCode()
        {
            return this.GetHashCodeFromPublicFields();
        }

        public bool Equals(ModelBase other)
        {
            return other != null && other == this || this.CompareTo(other) == 0;
        }

        public bool EqualsId(ModelBase other)
        {
            return other != null && this.GetType() == other.GetType() && Objects.GetHashCode(Id, Uuid) == Objects.GetHashCode(other.Id, other.Uuid);
        }
        
        public bool EqualsAnyId(ModelBase other)
        {
            return other != null && this.GetType() == other.GetType() && (this.Id == other.Id || this.Uuid == other.Uuid);
        }
        #endregion

        #region IComparable
        public int CompareTo(ModelBase other)
        {
            return other is null ? 1 : this.GetHashCode().CompareTo(other.GetHashCode());
        }

        public int CompareTo(object obj)
        {
            return obj is ModelBase mb ? this.CompareTo(mb) : 1;
        }
        #endregion
    }
}
