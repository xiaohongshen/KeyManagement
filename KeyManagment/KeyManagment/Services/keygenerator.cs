using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace KeyManagment.Services
{
    /* the basic ideal is from internet 
     * 
     * https://codeshare.co.uk/blog/how-to-create-a-random-password-generator-in-c/ 
     * 
     */
    public static class KeyGenerator
    {

        public static string GeneratePassword()
        {
            const int PASSWORD_LENGTH = 10;
            const string CHARACTER_SET = "abcdefghijklmnopqrstuvwxyz" +
                                         "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                                         "0123456789" +
                                         @"!#$%&*@\";

            char[] password_char = new char[PASSWORD_LENGTH];
            int characterSetLength = CHARACTER_SET.Length;
            String password;
            bool pwisntvalid = true;
            do
            {
                System.Random random = new System.Random();
                for (int characterPosition = 0; characterPosition < PASSWORD_LENGTH; characterPosition++)
                {
                    password_char[characterPosition] = CHARACTER_SET[random.Next(characterSetLength - 1)];
                }
                password = string.Join(null, password_char);
                pwisntvalid = !(Regex.IsMatch(password, @"[A-Z]") 
                            && Regex.IsMatch(password, @"[a-z]")
                            && Regex.IsMatch(password, @"[\d]")
                            && Regex.IsMatch(password, @"([!#$%&*@\\])"));

            } while (pwisntvalid);

            return password;
        }
    }
}
