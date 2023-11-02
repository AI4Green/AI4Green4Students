import { Route, Routes } from "react-router-dom";
import { ProtectedRoutes } from "layouts/ProtectedRoutes";
import { Experiment } from "pages/Experiment";
import { Overview } from "pages/Experiment/Plan/Overview";
import { IndividualExperimentPlan } from "pages/Experiment/IndividualExperimentPlan";
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
            user.permissions?.includes(
              EXPERIMENTS_PERMISSIONS.ViewOwnExperiments
            )
          }
        />
      }
    >
      <Route index element={<Experiment />} /> sdsd
    </Route>

    <Route
      path="project/:projectId/planoverview/:experimentId"
      element={
        <ProtectedRoutes
          isAuthorized={(user) =>
            user.permissions?.includes(
              EXPERIMENTS_PERMISSIONS.EditOwnExperiments,
              EXPERIMENTS_PERMISSIONS.ViewOwnExperiments
            )
          }
        />
      }
    >
      <Route index element={<Overview />} />
    </Route>

    <Route
      path="project/:projectId/experiment/:experimentId/plansection/:sectionId"
      element={
        <ProtectedRoutes
          isAuthorized={(user) =>
            user.permissions?.includes(
              EXPERIMENTS_PERMISSIONS.ViewOwnExperiments,
              EXPERIMENTS_PERMISSIONS.EditOwnExperiments
            )
          }
        />
      }
    >
      <Route index element={<Section />} />
    </Route>

    <Route
      path="edit/:experimentId"
      element={
        <ProtectedRoutes
          isAuthorized={(user) =>
            user.permissions?.includes(
              EXPERIMENTS_PERMISSIONS.EditOwnExperiments,
              EXPERIMENTS_PERMISSIONS.ViewOwnExperiments
            )
          }
        />
      }
    >
      <Route index element={<IndividualExperimentPlan />} />
    </Route>

    <Route path="*" element={<NotFound />} />
  </Routes>
);
