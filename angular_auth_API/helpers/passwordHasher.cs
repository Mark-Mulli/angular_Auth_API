using System;
using System.Security.Cryptography;

namespace angular_auth_API.helpers
{
	public class passwordHasher
	{
		//crypto service provider
		private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

		private static readonly int SaltSize = 16;

        private static readonly int HashSize = 20;

        private static readonly int Iterations = 1000;

		//method to hash password

		public static string hashPassword( string password)
		{
			byte[] salt;

			rng.GetBytes(salt = new byte[SaltSize]);

			var key = new Rfc2898DeriveBytes(password,salt,Iterations);

			var hash = key.GetBytes(HashSize);

			var hashBytes = new byte[SaltSize + HashSize];

			Array.Copy(salt,0,hashBytes,0,SaltSize);

			Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

			var base64hash = Convert.ToBase64String(hashBytes);
			

			return base64hash;
		}


    }
}

