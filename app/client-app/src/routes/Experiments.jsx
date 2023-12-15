import { Route, Routes } from "react-router-dom";
import { ProtectedRoutes } from "layouts/ProtectedRoutes";
import { Experiment } from "pages/Experiment";
import { PlanOverview } from "pages/Experiment/Plan/Overview/PlanOverview";
import { ReactionOverview } from "pages/Experiment/Plan/Overview/ReactionOverview";
import { NotFound } from "pages/error/NotFound";
import { EXPERIMENTS_PERMISSIONS } from "constants/site-permissions";
import { Section } from "pages/Experiment/Plan/Section";

export const Experiments = () => (
  <Routes>
    <Route
      path="project/:projectId"
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
      path=":experimentId/plan-overview"
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
      path=":experimentId/reaction-overview/:reactionId"
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
      <Route index element={<ReactionOverview />} />
    </Route>

    <Route
      path=":experimentId/plansection/:sectionId"
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
      <Route index element={<Section />} />
    </Route>

    <Route path="*" element={<NotFound />} />
  </Routes>
);
