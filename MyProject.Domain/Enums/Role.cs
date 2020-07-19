using MyProject.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyProject.Domain.Enums
{
    public sealed class Role : TypeSafeEnum
    {
        public char Code { get; }

        private static readonly Dictionary<int, Role> Instance = new Dictionary<int, Role>();
        public static readonly Role Anonymous = new Role(0, "Anonymous", 'X');
        public static readonly Role User = new Role(1, "User", 'U');
        public static readonly Role Admin = new Role(2, "Admin", 'A');
        public static readonly Role Developer = new Role(3, "Developer", 'D');

        private Role(int value, string name, char code) : base(value, name)
        {
            this.Code = code;
            Instance[value] = this;
        }

        public static explicit operator Role(int value)
        {
            if (Instance.TryGetValue(value, out var result))
                return result;
            else
                throw new InvalidCastException();
        }

        public static explicit operator Role(char value)
        {
            var result = Instance.Select(x => x.Value).FirstOrDefault(x => x.Code == value);
            if (result != null)
                return result;
            else
                throw new InvalidCastException();
        }

        public static explicit operator int(Role v)
        {
            return v.Value;
        }

        public static IList<(string, string)> ToTupleList()
        {
            return Instance.Select(x => (x.Value.ToString(), x.Key.ToString())).ToList();
        }
    }
}