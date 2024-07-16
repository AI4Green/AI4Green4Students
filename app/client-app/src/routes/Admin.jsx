import { Route, Routes } from "react-router-dom";
import { ProtectedRoutes } from "layouts/ProtectedRoutes";
import { UserManagement } from "pages/admin/UserManagement";
import { RegistrationRule } from "pages/RegistrationRule";
import { NotFound } from "pages/error/NotFound";
import { USERMANAGEMENT_PERMISSIONS } from "constants/site-permissions";
import { REGISTRATION_RULES_PERMISSIONS } from "constants/site-permissions";

export const Admin = () => (
  <Routes>
    <Route
      path="usermanagement"
      element={
        <ProtectedRoutes
          isAuthorized={(user) =>
            // for now, only allow access if user has all of the User management permissions
            Object.values(USERMANAGEMENT_PERMISSIONS).every((permission) =>
              user.permissions?.includes(permission)
            )
          }
        />
      }
    >
      <Route index element={<UserManagement />} />
    </Route>

    <Route
      path="registrationrule"
      element={
        <ProtectedRoutes
          isAuthorized={(user) =>
            Object.values(REGISTRATION_RULES_PERMISSIONS).every((permission) =>
              user.permissions?.includes(permission)
            )
          }
        />
      }
    >
      <Route index element={<RegistrationRule />} />
    </Route>

    <Route path="*" element={<NotFound />} />
  </Routes>
);
