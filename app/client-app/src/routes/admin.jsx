import {
  PROJECT_TYPE_MANAGEMENT_PERMISSIONS,
  USERMANAGEMENT_PERMISSIONS,
} from "constants";
import { ProtectedRoutes } from "layouts/protected-routes";
import { UserManagement } from "pages/admin/user-management";
import { NotFound } from "pages/error";
import { ProjectTypeCanvas } from "pages/project-type/canvas";
import { ProjectTypeList } from "pages/project-type/list";
import { Route, Routes } from "react-router-dom";

export const Admin = () => (
  <Routes>
    <Route
      path="user-management"
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

    <Route path="*" element={<NotFound />} />
  </Routes>
);
