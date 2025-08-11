import { useEffect } from "react";
import { Route, Routes, useNavigate } from "react-router-dom";
import { useUser } from "contexts";
import { Confirm } from "pages/account/confirm";
import { Login } from "pages/account/login";
import { Register } from "pages/account/register";
import { RequestPasswordReset } from "pages/account/request-password-reset";
import { ResendConfirm } from "pages/account/resend-confirm";
import { ResendPasswordReset } from "pages/account/resend-password-reset";
import { ResetPassword } from "pages/account/reset-password";
import { ConfirmEmailChange } from "pages/account/confirm-email-change";
import { NotFound } from "pages/error";
import { ActivateAccount } from "pages/account/activate";

const Redirect = () => {
  const navigate = useNavigate();
  useEffect(() => {
    const targetPath = "/";
    navigate(targetPath, { replace: true });
  }, [navigate]);
  return null;
};

export const Account = () => {
  const { user } = useUser();
  return (
    <Routes>
      <Route path="login" element={user ? <Redirect /> : <Login />} />
      <Route path="register" element={user ? <Redirect /> : <Register />} />
      <Route path="confirm" element={<Confirm />} />
      <Route path="confirmEmailChange" element={<ConfirmEmailChange />} />
      <Route path="confirm/resend" element={<ResendConfirm />} />
      <Route path="password/reset" element={<RequestPasswordReset />} />
      <Route path="password/resend" element={<ResendPasswordReset />} />
      <Route path="password" element={<ResetPassword />} />
      <Route path="activate" element={<ActivateAccount />} />
      <Route path="*" element={<NotFound />} />
    </Routes>
  );
};
