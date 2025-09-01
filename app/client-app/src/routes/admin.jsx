import {
  PROJECT_TYPE_MANAGEMENT_PERMISSIONS,
  REGISTRATION_RULES_PERMISSIONS,
  USERMANAGEMENT_PERMISSIONS,
} from "constants";
import { ProtectedRoutes } from "layouts/protected-routes";
import { ProjectTypeManagement } from "pages/admin/project-type-management";
import { UserManagement } from "pages/admin/user-management";
import { NotFound } from "pages/error";
import { ProjectTypeView } from "pages/project-type/view";
import { RegistrationRule } from "pages/registration-rule";
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

    <Route
      path="registration-rule"
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

    <Route
      path="project-type-management"
      element={
        <ProtectedRoutes
          isAuthorized={(user) =>
            Object.values(PROJECT_TYPE_MANAGEMENT_PERMISSIONS).every(
              (permission) => user.permissions?.includes(permission)
            )
          }
        />
      }
    >
      <Route index element={<ProjectTypeManagement />} />
    </Route>

    <Route
      path="project-type-management/:projectTypeId"
      element={
        <ProtectedRoutes
          isAuthorized={(user) =>
            Object.values(PROJECT_TYPE_MANAGEMENT_PERMISSIONS).every(
              (permission) => user.permissions?.includes(permission)
            )
          }
        />
      }
    >
      <Route index element={<ProjectTypeView />} />
      <Route
        path="section-types/:sectionTypeId/sections"
        element={<ProjectTypeView />}
      />
      <Route
        path="section-types/:sectionTypeId/sections/:sectionId/fields"
        element={<ProjectTypeView />}
      />
    </Route>

    <Route path="*" element={<NotFound />} />
  </Routes>
);
