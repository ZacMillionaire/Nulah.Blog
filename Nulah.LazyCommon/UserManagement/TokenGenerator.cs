using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Nulah.LazyCommon.Core.UserManagement {
    public class TokenGenerator {

        /// <summary>
        /// Returns a token used for registrations
        /// </summary>
        /// <returns></returns>
        public string GenerateRegistrationToken() {
            var guidToken = Guid.NewGuid();
            var sha512 = SHA512.Create();
            return HashToString(sha512.ComputeHash(guidToken.ToByteArray()));
        }

        /// <summary>
        /// Returns a token used for unique codes, capped at a fixed length
        /// </summary>
        /// <returns></returns>
        public string GenerateUniqueToken(int MaxLength) {

            if(MaxLength == 0) {
                throw new ArgumentException("MaxLength must be greater than 0");
            }

            var guidToken = Guid.NewGuid();
            var sha512 = SHA512.Create();
            return HashToString(sha512.ComputeHash(guidToken.ToByteArray())).Substring(0, MaxLength);
        }

        public string HashToString(byte[] HashArray) {
            StringBuilder sb = new StringBuilder();
            foreach(byte b in HashArray) {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
