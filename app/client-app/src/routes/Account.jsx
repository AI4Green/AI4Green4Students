import { useEffect } from "react";
import { Route, Routes, useNavigate } from "react-router-dom";
import { useUser } from "contexts";
import { Confirm } from "pages/account/Confirm";
import { Login } from "pages/account/Login";
import { Register } from "pages/account/Register";
import { RequestPasswordReset } from "pages/account/RequestPasswordReset";
import { ResendConfirm } from "pages/account/ResendConfirm";
import { ResendPasswordReset } from "pages/account/ResendPasswordReset";
import { ResetPassword } from "pages/account/ResetPassword";
import { ConfirmEmailChange } from "pages/account/ConfirmEmailChange";
import { NotFound } from "pages/error";
import { ActivateAccount } from "pages/account/AccountActivate";

const Redirect = () => {
  const navigate = useNavigate();
  useEffect(() => {
    const targetPath = "/home";
    navigate(targetPath, { replace: true });
  }, []);
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
