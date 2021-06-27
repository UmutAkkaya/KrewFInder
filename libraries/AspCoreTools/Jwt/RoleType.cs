using System;
using System.Collections.Generic;
using System.Text;
using DatabaseLayer.Models;

namespace AspCoreTools.Jwt
{
    /// <summary>
    /// Built-in roles for KrewFindr. Can be implicitly cast to string for convenience.
    /// </summary>
    public sealed class RoleType
    {
        public static readonly RoleType UserIdClaimType = new RoleType("uid");

        /// <summary>
        /// Used for logging out (if present token is ignored by auth middleware)
        /// </summary>
        public static readonly RoleType VoidToken = new RoleType("void");

        private readonly string _value;

        private RoleType(string val)
        {
            _value = val;
        }

        public static implicit operator string(RoleType ct)
        {
            return ct._value;
        }

        public override string ToString()
        {
            return _value;
        }
    }
}
