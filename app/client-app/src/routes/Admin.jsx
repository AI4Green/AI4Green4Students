import { Route, Routes } from "react-router-dom";
import { ProtectedRoutes } from "layouts/ProtectedRoutes";
import { permissions } from "constants/site-permissions";
import { UserManagement } from "pages/UserManagement";
import { RegistrationRule } from "pages/RegistrationRule";
import { NotFound } from "pages/error/NotFound";

export const Admin = () => (
  <Routes>
    <Route
      path="/"
      element={
        <ProtectedRoutes
          isAuthorized={(user) =>
            [permissions.ManageUsers].some((x) => user.permissions?.includes(x))
          }
        />
      }
    >
      <Route path="usermanagement" element={<UserManagement />} />
      <Route path="registrationrule" element={<RegistrationRule />} />
      <Route path="*" element={<NotFound />} />
    </Route>
  </Routes>
);
