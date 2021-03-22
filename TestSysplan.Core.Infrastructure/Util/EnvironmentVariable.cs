using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace TestSysplan.Core.Infrastructure.Util
{
    /// <summary>
    /// <para>Environment Variable</para> 
    /// <para>
    /// System Environment variable abstraction how structure.
    /// </para>
    /// <para>    
    /// <strong>Allow to load environment variable by your name:</strong>
    /// </para>
    /// <para>
    /// <i>● By constructor; </i> <br/><br/>
    /// <i>● [Or] The static method <see cref="EnvironmentVariable.GetFromProcess(string, object)"/>;</i><br/><br/>
    /// <i>● [Or] The static method <see cref="EnvironmentVariable.GetFromUser(string, object)"/>;</i> <br/><br/>
    /// <i>● [Or] The static method <see cref="EnvironmentVariable.GetFromMachine(string, object)"/>;</i> <br/><br/>
    /// <i>● [Or] Implicit operator through a simple String text.</i>
    /// </para>
    /// </summary>
    /// <author>Carlos Matos</author>
    /// <date>2021-01-19</date>
    public struct EnvironmentVariable : IComparable, IComparable<EnvironmentVariable>, IConvertible, IEquatable<EnvironmentVariable>
    {
        private delegate bool TryParse<T>(string value, out T result);

        private readonly string name;
        private readonly object defaultValue;
        private readonly EnvironmentVariableTarget target;
        
        private object SyncRoot => string.Intern($"{nameof(EnvironmentVariable)}#{target}_{name}");

        /// <summary>
        /// Variable name
        /// </summary>
        public string Name => name;

        /// <summary>
        /// Current variable value
        /// </summary>
        public string Value
        {
            get
            {
                lock (SyncRoot)
                {
                    return Environment.GetEnvironmentVariable(name, target) ?? defaultValue?.ToString() ?? string.Empty;
                }
            }
        }

        /// <summary>
        /// Indicates whether the current environment variable value is null or empty.
        /// </summary>
        public bool IsNullOrEmpty => string.IsNullOrEmpty(Value);

        /// <summary>
        /// Indicates whether current environment variable has a value.
        /// </summary>
        public bool HasValue => !IsNullOrEmpty;

        /// <summary>
        /// Load system environment variable by name
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="defaultValue">default value to be used whether the variable not found or current value is empty</param>
        private EnvironmentVariable(string name, object defaultValue = null, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
        {
            this.name           = name ?? throw new ArgumentNullException(nameof(name));
            this.defaultValue   = defaultValue;
            this.target         = target;
        }

        /// <summary>
        /// Try get environment variable value whether non null and non empty.
        /// </summary>
        /// <param name="value">recovered non null and non empty value</param>
        /// <returns></returns>
        public bool TryGetValue(out string value)
        {
            lock (SyncRoot)
            {
                value = Environment.GetEnvironmentVariable(name, target) ?? defaultValue?.ToString() ?? string.Empty;
                return !string.IsNullOrEmpty(value);
            }
        }

        /// <summary>
        /// Get environment variable value or default value whether current variable not set.
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string GetValueOrDefault(object defaultValue)
        {
            string aux = Value;
            return string.IsNullOrWhiteSpace(aux) ? defaultValue.ToString() : aux;
        }

        /// <summary>
        /// Creates, modifies, or deletes an environment variable stored in the current process
        /// or in the Windows operating system registry key reserved for the current user
        /// or local machine.
        /// </summary>
        /// <param name="value">
        /// A value to assign to variable
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// variable contains a zero-length string, an initial hexadecimal zero character
        /// (0x00), or an equal sign ("="). -or- The length of variable or value is greater
        /// than or equal to 32,767 characters. -or- An error occurred during the execution
        /// of this operation.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The caller does not have the required permission to perform this operation.
        /// </exception>
        public void SetValue(string value)
        {
            lock (SyncRoot)
            {
                Environment.SetEnvironmentVariable(name, value, target);
            }
        }

        private T ParseOrDefault<T>(TryParse<T> tryParse) => tryParse.Invoke(Value, out T result) ? result : default(T);

        public override string ToString() => Value;

        #region IComparable
        public int CompareTo(object obj)
        {
            return obj is EnvironmentVariable otherEnv ? this.CompareTo(otherEnv) : -1;
        }

        public int CompareTo(EnvironmentVariable otherEnv)
        {
            int i;
            return (this == otherEnv ? 0 :
                (i = this.Name?.CompareTo(otherEnv.Name) ?? -1) == 0 ?
                (this.Value?.CompareTo(otherEnv.Value) ?? -1) : i);
        }
        #endregion

        #region IConvertible
        public TypeCode GetTypeCode() => TypeCode.String;

        public bool ToBoolean(IFormatProvider provider = null) => this;

        public byte ToByte(IFormatProvider provider = null) => this;

        public char ToChar(IFormatProvider provider = null) => this;

        public DateTime ToDateTime(IFormatProvider provider = null) => this;

        public decimal ToDecimal(IFormatProvider provider = null) => this;

        public double ToDouble(IFormatProvider provider = null) => this;

        public short ToInt16(IFormatProvider provider = null) => this;

        public int ToInt32(IFormatProvider provider = null) => this;

        public long ToInt64(IFormatProvider provider = null) => this;

        public sbyte ToSByte(IFormatProvider provider = null) => this;

        public float ToSingle(IFormatProvider provider = null) => this;

        public string ToString(IFormatProvider provider) => this.Value;

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            throw new InvalidCastException($"Can not convert EnvironmentVariable type to target type {conversionType}!");
        }

        public ushort ToUInt16(IFormatProvider provider = null) => this;

        public uint ToUInt32(IFormatProvider provider = null) => this;

        public ulong ToUInt64(IFormatProvider provider = null) => this;
        #endregion

        #region IEquatable
        public bool Equals(EnvironmentVariable other)
        {
            return this.CompareTo(other) == 0;
        }

        public override bool Equals(object obj)
        {
            return this.CompareTo(obj) == 0;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Operators
        public static bool operator ==(EnvironmentVariable env0, EnvironmentVariable env1)
        {
            return env0.GetHashCode() == env1.GetHashCode();
        }

        public static bool operator !=(EnvironmentVariable env0, EnvironmentVariable env1)
        {
            return env0.GetHashCode() != env1.GetHashCode();
        }
                
        public static implicit operator EnvironmentVariable(string variableName) => new EnvironmentVariable(variableName);

        public static implicit operator EnvironmentVariable((string, object) pair) => new EnvironmentVariable(pair.Item1, pair.Item2);

        public static implicit operator EnvironmentVariable((string, object, EnvironmentVariableTarget) pair) => new EnvironmentVariable(pair.Item1, pair.Item2, pair.Item3);

        public static implicit operator EnvironmentVariable((string, EnvironmentVariableTarget) pair) => new EnvironmentVariable(pair.Item1, target: pair.Item2);

        public static implicit operator EnvironmentVariable(Tuple<string> tuple) => new EnvironmentVariable(tuple.Item1);

        public static implicit operator EnvironmentVariable(Tuple<string, object> tuple) => new EnvironmentVariable(tuple.Item1, tuple.Item2);

        public static implicit operator EnvironmentVariable(Tuple<string, object, EnvironmentVariableTarget> tuple) => new EnvironmentVariable(tuple.Item1, tuple.Item2, tuple.Item3);

        public static implicit operator EnvironmentVariable(Tuple<string, EnvironmentVariableTarget> tuple) => new EnvironmentVariable(tuple.Item1, target: tuple.Item2);

        public static implicit operator EnvironmentVariable(KeyValuePair<string, object> tuple) => new EnvironmentVariable(tuple.Key, tuple.Value);

        public static implicit operator EnvironmentVariable(KeyValuePair<string, EnvironmentVariableTarget> tuple) => new EnvironmentVariable(tuple.Key, target: tuple.Value);

        public static implicit operator string(EnvironmentVariable env) => env.Value;

        public static implicit operator byte(EnvironmentVariable env) => env.ParseOrDefault<byte>(byte.TryParse);

        public static implicit operator sbyte(EnvironmentVariable env) => env.ParseOrDefault<sbyte>(sbyte.TryParse);

        public static implicit operator bool(EnvironmentVariable env) => env.ParseOrDefault<bool>(bool.TryParse) || bool.TrueString.Equals(env.Value, StringComparison.InvariantCultureIgnoreCase) || env == 1;

        public static implicit operator char(EnvironmentVariable env) => env.ParseOrDefault<char>(char.TryParse);

        public static implicit operator short(EnvironmentVariable env) => env.ParseOrDefault<short>(short.TryParse);

        public static implicit operator ushort(EnvironmentVariable env) => env.ParseOrDefault<ushort>(ushort.TryParse);

        public static implicit operator int(EnvironmentVariable env) => env.ParseOrDefault<int>(int.TryParse);

        public static implicit operator uint(EnvironmentVariable env) => env.ParseOrDefault<uint>(uint.TryParse);

        public static implicit operator long(EnvironmentVariable env) => env.ParseOrDefault<long>(long.TryParse);

        public static implicit operator ulong(EnvironmentVariable env) => env.ParseOrDefault<ulong>(ulong.TryParse);

        public static implicit operator float(EnvironmentVariable env) => env.ParseOrDefault<float>(float.TryParse);

        public static implicit operator double(EnvironmentVariable env) => env.ParseOrDefault<double>(double.TryParse);

        public static implicit operator decimal(EnvironmentVariable env) => env.ParseOrDefault<decimal>(decimal.TryParse);

        public static implicit operator DateTime(EnvironmentVariable env) => env.ParseOrDefault<DateTime>(DateTime.TryParse);
        #endregion

        #region Get
        /// <summary>
        /// Load a environment variable by your name from current process.
        /// </summary>
        /// <param name="variableName">variable name</param>
        /// <param name="defaultValue">default value to be used whether the variable not found or current value is empty</param>
        /// <returns></returns>
        public static EnvironmentVariable GetFromProcess([CallerMemberName] string variableName = null, object defaultValue = null)
        {
            return new EnvironmentVariable(variableName, defaultValue);
        }

        /// <summary>
        /// Load a environment variable by your name from current user.
        /// </summary>
        /// <param name="variableName">variable name</param>
        /// <param name="defaultValue">default value to be used whether the variable not found or current value is empty</param>
        /// <returns></returns>
        public static EnvironmentVariable GetFromUser([CallerMemberName] string variableName = null, object defaultValue = null)
        {
            return new EnvironmentVariable(variableName, defaultValue, EnvironmentVariableTarget.User);
        }

        /// <summary>
        /// Load a environment variable by your name from local machine.
        /// </summary>
        /// <param name="variableName">variable name</param>
        /// <param name="defaultValue">default value to be used whether the variable not found or current value is empty</param>
        /// <returns></returns>
        public static EnvironmentVariable GetFromMachine([CallerMemberName] string variableName = null, object defaultValue = null)
        {
            return new EnvironmentVariable(variableName, defaultValue, EnvironmentVariableTarget.Machine);
        }
        #endregion
    }
}
