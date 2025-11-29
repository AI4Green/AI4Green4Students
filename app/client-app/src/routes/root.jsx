import {
  PROJECT_TYPE_MANAGEMENT_PERMISSIONS,
  PROJECTMANAGEMENT_PERMISSIONS,
  REGISTRATION_RULES_PERMISSIONS,
} from "constants";
import { DefaultLayout } from "layouts/default";
import { ProtectedRoutes } from "layouts/protected-routes";
import { ContentPage } from "pages/content";
import { NotFound } from "pages/error";
import GreenMetrics from "pages/green-metrics";
import { Home } from "pages/home";
import ReactionPredictions from "pages/reactions-predictions";
import { RegistrationRule } from "pages/registration-rule";
import { Route, Routes } from "react-router-dom";

import { Account } from "./account";
import { Admin } from "./admin";
import { Project } from "./project";
import { ProjectType } from "./project-type";

export const Root = () => {
  return (
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/*" element={<DefaultLayout />}>
        <Route path="metrics" element={<GreenMetrics />} />
        <Route path="reaction-predictions" element={<ReactionPredictions />} />
        <Route
          path="greenchemistry"
          element={<ContentPage contentKey={"greenchemistry"} />}
        />
        <Route path="about" element={<ContentPage contentKey={"about"} />} />

        <Route
          path="documentation"
          element={<ContentPage contentKey={"documentation"} />}
        />

        <Route
          path="projects/*"
          element={
            <ProtectedRoutes
              isAuthorized={(user) =>
                user.permissions?.includes(
                  PROJECTMANAGEMENT_PERMISSIONS.ViewProjects
                )
              }
            />
          }
        >
          <Route path="*" element={<Project />} />
        </Route>

        <Route
          path="registration-rule"
          element={
            <ProtectedRoutes
              isAuthorized={(user) =>
                user.permissions?.includes(
                  REGISTRATION_RULES_PERMISSIONS.ViewRegistrationRules
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
                user.permissions?.includes(
                  PROJECT_TYPE_MANAGEMENT_PERMISSIONS.ViewProjectTypes
                )
              }
            />
          }
        >
          <Route path="*" element={<ProjectType />} />
        </Route>

        <Route path="account/*" element={<Account />} />

        <Route path="admin/*" element={<Admin />} />

        <Route path="*" element={<NotFound />} />
      </Route>
    </Routes>
  );
};
