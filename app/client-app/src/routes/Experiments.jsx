import { Route, Routes } from "react-router-dom";
import { ProtectedRoutes } from "layouts/ProtectedRoutes";
import { Experiment } from "pages/Experiment";
import { NotFound } from "pages/error/NotFound";
import { EXPERIMENTS_PERMISSIONS } from "constants/site-permissions";

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
      <Route index element={<Experiment />} />
    </Route>

    <Route path="*" element={<NotFound />} />
  </Routes>
);
