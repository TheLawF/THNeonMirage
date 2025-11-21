using System;

namespace Fictology.Registry
{
    [Serializable]
    public class RegistryKey: IEquatable<RegistryKey>
    {
        public string rootName;
        public string registryName;

        public RegistryKey(string rootName, string registryName)
        {
            this.rootName = rootName;
            this.registryName = registryName;
        }

        public string GetRegistryName() => registryName;

        public static RegistryKey Create(string rootName, string registryName) => new (rootName, registryName);

        public override string ToString()
        {
            return $"{rootName}/{registryName}";
        }

        // 重载 == 和 != 运算符
        public static bool operator ==(RegistryKey left, RegistryKey right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(RegistryKey left, RegistryKey right)
        {
            return !(left == right);
        }

        // 重写 GetHashCode 方法
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (rootName?.GetHashCode(StringComparison.Ordinal) ?? 0);
                hash = hash * 23 + (registryName?.GetHashCode(StringComparison.Ordinal) ?? 0);
                return hash;
            }
        }
    
        public bool Equals(RegistryKey other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
        
            return string.Equals(rootName, other.rootName, StringComparison.Ordinal) &&
                   string.Equals(registryName, other.registryName, StringComparison.Ordinal);
        }
    }
}