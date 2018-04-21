using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Nulah.LazyCommon.Core.UserManagement {
    public class TokenGenerator {
        /// <summary>
        /// Returns a token used for
        /// </summary>
        /// <returns></returns>
        public string GenerateRegistrationToken() {
            var guidToken = Guid.NewGuid();
            var sha512 = SHA512.Create();
            return HashToString(sha512.ComputeHash(guidToken.ToByteArray()));
        }

        private string HashToString(byte[] HashArray) {
            StringBuilder sb = new StringBuilder();
            foreach(byte b in HashArray) {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
