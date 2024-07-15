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
import { ReportOverview } from "pages/experiment/overview/ReportOverview";
import { ReportSection } from "pages/experiment/section/ReportSection";

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
        path=":projectId/literature-reviews/:literatureReviewId/overview"
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
        path=":projectId/plans/:planId/overview"
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
        path=":projectId/notes/:noteId/overview"
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
        path=":projectId/reports/:reportId/overview"
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
        <Route index element={<ReportOverview />} />
      </Route>

      <Route
        path=":projectId/literature-reviews/:literatureReviewId/sections/:sectionId"
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
        path=":projectId/plans/:planId/sections/:sectionId"
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
        path=":projectId/reports/:reportId/sections/:sectionId"
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
        <Route index element={<ReportSection />} />
      </Route>

      <Route
        path=":projectId/notes/:noteId/sections/:sectionId"
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
        path=":projectId/project-groups/:projectGroupId/activities"
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
