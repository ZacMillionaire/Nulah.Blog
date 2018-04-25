using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Nulah.LazyCommon.Core.UserManagement {
    public class PasswordManagement {
        private readonly TokenGenerator _tokenGenerator;

        public PasswordManagement(TokenGenerator tokenGenerator) {
            _tokenGenerator = tokenGenerator;
        }

        /// <summary>
        /// Creates a single use password and hash.
        /// </summary>
        /// <param name="OneTimePassword"></param>
        /// <returns></returns>
        public OneTimePassword GenerateOneTimePassword() {
            var OneTimePassword = _tokenGenerator.GenerateUniqueToken(12);
            var OneTimePasswordHash = HashOneTimePassword(OneTimePassword);
            return new OneTimePassword {
                Password = OneTimePassword,
                Hash = OneTimePasswordHash
            };
        }

        /// <summary>
        /// Returns the hash of a given one time password
        /// </summary>
        /// <param name="OneTimePassword"></param>
        /// <returns></returns>
        public string HashFromPassword(string OneTimePassword) {
            return HashOneTimePassword(OneTimePassword);
        }

        private string HashOneTimePassword(string OneTimePassword) {
            var sha512 = SHA512.Create();
            return _tokenGenerator.HashToString(sha512.ComputeHash(Encoding.ASCII.GetBytes(OneTimePassword)));
        }
    }

    // TODO: Move to seperate file
    public class OneTimePassword {
        public string Password { get; set; }
        public string Hash { get; set; }
    }
}
