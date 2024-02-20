import { Route, Routes } from "react-router-dom";
import { ProtectedRoutes } from "layouts/ProtectedRoutes";
import { PlanOverview } from "pages/experiment/overview/PlanOverview";
import { NotFound } from "pages/error/NotFound";
import { EXPERIMENTS_PERMISSIONS } from "constants/site-permissions";
import { PlanSection } from "pages/experiment/section/PlanSection";
import { ProjectGroupList } from "pages/project/ProjectGroupList";
import { ProjectGroupExperimentList } from "pages/experiment/ProjectGroupExperimentList";
import { StudentExperimentList } from "pages/experiment/StudentExperimentList";
import { PROJECTMANAGEMENT_PERMISSIONS } from "constants/site-permissions";
import { LiteratureReviewOverview } from "pages/experiment/overview/LiteratureReviewOverview";
import { LiteratureReviewSection } from "pages/experiment/section/LiteratureReviewSection";

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
      <Route index element={<StudentExperimentList />} />
    </Route>

    <Route
      path=":projectId/project-group/:projectGroupId"
      element={
        <ProtectedRoutes
          isAuthorized={(user) =>
            [
              PROJECTMANAGEMENT_PERMISSIONS.ViewAllProjects,
              EXPERIMENTS_PERMISSIONS.ViewAllExperiments,
            ].every((permission) => user.permissions?.includes(permission))
          }
        />
      }
    >
      <Route index element={<ProjectGroupExperimentList />} />
    </Route>

    <Route
      path=":projectId/project-groups"
      element={
        <ProtectedRoutes
          isAuthorized={(user) =>
            user.permissions?.includes(
              PROJECTMANAGEMENT_PERMISSIONS.ViewAllProjects
            )
          }
        />
      }
    >
      <Route index element={<ProjectGroupList />} />
    </Route>

    <Route
      path=":sectionTypeId/literatureReview-overview/:literatureReviewId"
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
      <Route index element={<LiteratureReviewOverview />} />
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
      path="literatureReview-section/:literatureReviewId/:sectionId"
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
      <Route index element={<LiteratureReviewSection />} />
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
