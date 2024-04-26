import { Route, Routes } from "react-router-dom";
import { ProtectedRoutes } from "layouts/ProtectedRoutes";
import { PlanOverview } from "pages/experiment/overview/PlanOverview";
import { NotFound } from "pages/error/NotFound";
import { EXPERIMENTS_PERMISSIONS } from "constants/site-permissions";
import { PlanSection } from "pages/experiment/section/PlanSection";
import { ProjectGroupList } from "pages/project/ProjectGroupList";
import { StudentExperimentList } from "pages/experiment/summary/StudentExperimentList";
import { ProjectGroupExperimentList } from "pages/experiment/summary/ProjectGroupExperimentList";
import { LiteratureReviewOverview } from "pages/experiment/overview/LiteratureReviewOverview";
import { LiteratureReviewSection } from "pages/experiment/section/LiteratureReviewSection";
import { GroupProjectSummarySection } from "pages/experiment/section/GroupProjectSummarySection";
import { NoteOverview } from "pages/experiment/overview/NoteOverview";
import { NoteSection } from "pages/experiment/section/NoteSection";
import { useIsInstructor } from "components/experiment/useIsInstructor";

export const Project = () => {
  const isInstructor = useIsInstructor();
  return (
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
        <Route
          index
          element={
            isInstructor ? <ProjectGroupList /> : <StudentExperimentList />
          }
        />
      </Route>

      <Route
        path=":projectId/project-group/:projectGroupId"
        element={<ProtectedRoutes isAuthorized={() => isInstructor} />}
      >
        <Route index element={<ProjectGroupExperimentList />} />
      </Route>

      <Route
        path=":projectId/project-group/:projectGroupId/section-type/:sectionTypeId/literature-review/:literatureReviewId/overview"
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
        path=":projectId/project-group/:projectGroupId/section-type/:sectionTypeId/plan/:planId/overview"
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
        path=":projectId/project-group/:projectGroupId/section-type/:sectionTypeId/note/:noteId/overview"
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
        <Route index element={<NoteOverview />} />
      </Route>

      <Route
        path=":projectId/project-group/:projectGroupId/literature-review/:literatureReviewId/section/:sectionId"
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
        path=":projectId/project-group/:projectGroupId/plan/:planId/section/:sectionId"
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

      <Route
        path=":projectId/project-group/:projectGroupId/note/:noteId/section/:sectionId"
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
        <Route index element={<NoteSection />} />
      </Route>

      <Route
        path=":projectId/project-group/:projectGroupId/section-type/:sectionTypeId/activities"
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
        <Route index element={<GroupProjectSummarySection />} />
      </Route>

      <Route path="*" element={<NotFound />} />
    </Routes>
  );
};
