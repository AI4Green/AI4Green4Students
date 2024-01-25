import { Route, Routes } from "react-router-dom";
import { ProtectedRoutes } from "layouts/ProtectedRoutes";
import { Experiment } from "pages/experiment";
import { PlanOverview } from "pages/experiment/overview/PlanOverview";
import { NotFound } from "pages/error/NotFound";
import { EXPERIMENTS_PERMISSIONS } from "constants/site-permissions";
import { PlanSection } from "pages/experiment/section/PlanSection";

export const Project = () => (
  <Routes>
    <Route
      path=":projectId"
      element={
        <ProtectedRoutes
          isAuthorized={(user) =>
            [
              EXPERIMENTS_PERMISSIONS.ViewOwnExperiments,
              EXPERIMENTS_PERMISSIONS.ViewAllExperiments,
            ].some((permission) => user.permissions?.includes(permission))
          }
        />
      }
    >
      <Route index element={<Experiment />} />
    </Route>

    <Route
      path=":sectionTypeId/plan-overview/:planId"
      element={
        <ProtectedRoutes
          isAuthorized={(user) =>
            [
              EXPERIMENTS_PERMISSIONS.ViewOwnExperiments,
              EXPERIMENTS_PERMISSIONS.ViewAllExperiments,
            ].some((permission) => user.permissions?.includes(permission))
          }
        />
      }
    >
      <Route index element={<PlanOverview />} />
    </Route>

    <Route
      path="plan-section/:planId/:sectionId"
      element={
        <ProtectedRoutes
          isAuthorized={(user) =>
            [
              EXPERIMENTS_PERMISSIONS.ViewOwnExperiments,
              EXPERIMENTS_PERMISSIONS.ViewAllExperiments,
            ].some((permission) => user.permissions?.includes(permission))
          }
        />
      }
    >
      <Route index element={<PlanSection />} />
    </Route>

    <Route path="*" element={<NotFound />} />
  </Routes>
);
