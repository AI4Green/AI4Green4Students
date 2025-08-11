import { Route, Routes } from "react-router-dom";
import { ProtectedRoutes } from "layouts/protected-routes";
import { UserManagement } from "pages/admin/user-management";
import { RegistrationRule } from "pages/registration-rule";
import { NotFound } from "pages/error";
import {
  USERMANAGEMENT_PERMISSIONS,
  REGISTRATION_RULES_PERMISSIONS,
} from "constants";

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
