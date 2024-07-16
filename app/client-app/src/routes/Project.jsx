import { Route, Routes, useNavigate } from "react-router-dom";
import { ProtectedRoutes } from "layouts/ProtectedRoutes";
import { PlanOverview } from "pages/experiment/overview/PlanOverview";
import { NotFound } from "pages/error/NotFound";
import { EXPERIMENTS_PERMISSIONS } from "constants/site-permissions";
import { PlanSection } from "pages/experiment/section/PlanSection";
import { ProjectGroupList } from "pages/project/ProjectGroupList";
import { StudentExperimentList } from "pages/experiment/summary/StudentExperimentList";
import { LiteratureReviewOverview } from "pages/experiment/overview/LiteratureReviewOverview";
import { LiteratureReviewSection } from "pages/experiment/section/LiteratureReviewSection";
import { GroupProjectSummarySection } from "pages/experiment/section/GroupProjectSummarySection";
import { NoteOverview } from "pages/experiment/overview/NoteOverview";
import { NoteSection } from "pages/experiment/section/NoteSection";
import { useCanManageProjects } from "pages/project/ProjectList";
import { ReportOverview } from "pages/experiment/overview/ReportOverview";
import { ReportSection } from "pages/experiment/section/ReportSection";
import { ProjectList } from "pages/project/ProjectList";
import { useEffect } from "react";

export const Project = () => {
  const canManageProjects = useCanManageProjects();
  return (
    <Routes>
      <Route path="/" element={<ProtectedRoutes />}>
        <Route index element={<ProjectList />} />
      </Route>

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
            canManageProjects ? (
              <RedirectToProjectGroups />
            ) : (
              <StudentExperimentList />
            )
          }
        />
      </Route>

      <Route
        path=":projectId/project-groups"
        element={<ProtectedRoutes isAuthorized={() => canManageProjects} />}
      >
        <Route index element={<ProjectGroupList />} />
      </Route>

      <Route
        path=":projectId/students/:studentId"
        element={<ProtectedRoutes isAuthorized={() => canManageProjects} />}
      >
        <Route index element={<StudentExperimentList />} />
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

/**
 * Redirect to project groups page.
 */
const RedirectToProjectGroups = () => {
  const navigate = useNavigate();
  useEffect(() => {
    const nextPath = "project-groups";
    navigate(nextPath, { replace: true });
  }, []);
  return null;
};
