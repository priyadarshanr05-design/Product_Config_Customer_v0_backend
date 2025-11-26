namespace Product_Config_Customer_v0.Services
{
    public static class User_02_Verification_EmailTemplates
    {
        public static string PasswordResetTemplate(string username, string otp)
        {
            return $@"
                <html>
                <body>
                    <h2>Password Reset Request</h2>
                    <p>Hello {username},</p>
                    <p>Your password reset code is:</p>

                    <h1 style=""color:#2d89ef;"">{otp}</h1>

                    <p>This code will expire in <b>15 minutes</b>.</p>
                    <p>If you did not request this, please ignore this email.</p>

                    <br />
                    <p>Thanks,<br/>Support Team</p>
                </body>
                </html>";
        }
    }
}
